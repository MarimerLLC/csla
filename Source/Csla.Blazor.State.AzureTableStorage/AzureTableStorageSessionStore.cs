//-----------------------------------------------------------------------
// <copyright file="AzureTableStorageSessionStore.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Azure Table Storage implementation of ISessionStore</summary>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;
using Azure;
using Azure.Data.Tables;
using Csla.Serialization;
using Csla.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Csla.Blazor.State.AzureTableStorage;

/// <summary>
/// Azure Table Storage implementation of <see cref="ISessionStore"/>.
/// </summary>
/// <remarks>
/// <para>
/// Live sessions are kept in memory and returned by reference, as
/// <c>InMemorySessionStore</c> does, because the session manager and the
/// Blazor context manager change the returned session in place. The table
/// is read only when a session is not in memory, such as after a restart.
/// </para>
/// <para>
/// Session data is written to the table only by
/// <see cref="AddOrUpdateSessionAsync"/>, which serializes the session before
/// publishing it to the cache. <see cref="PurgeExpiredSessionsAsync"/> refreshes
/// only the stored last-touched time of live sessions and never serializes
/// them, because a change a request makes during serialization can produce
/// data that cannot be deserialized.
/// </para>
/// <para>
/// Each row holds one session in the <c>LastTouched</c> column and up to
/// <see cref="MaxChunks"/> binary columns of <see cref="ChunkSize"/> bytes,
/// because Azure Tables limits a binary property to 64 KiB and an entity
/// to 1 MiB.
/// </para>
/// <para>
/// Read and write failures propagate to the caller. A failed read must not
/// look like a missing session, because the session manager would then
/// replace the stored session with a new, empty one.
/// </para>
/// </remarks>
internal sealed partial class AzureTableStorageSessionStore : ISessionStore
{
  internal const int ChunkSize = 64 * 1024;
  // The 1 MiB entity limit can be measured on the request, where binary
  // values are base64 encoded (Azurite measures it that way), so 11 chunks
  // (704 KiB) is the most that fits however it is measured.
  internal const int MaxChunks = 11;
  private const string ValueColumn = "Value";
  private const string LastTouchedColumn = "LastTouched";

  private readonly ConcurrentDictionary<string, Session> _sessions = [];
  private readonly SemaphoreSlim[] _keyLocks = [.. Enumerable.Range(0, 32).Select(_ => new SemaphoreSlim(1, 1))];
  private readonly string _applicationDiscriminator;
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly TimeProvider _timeProvider;
  private readonly ILogger<AzureTableStorageSessionStore> _logger;
  private readonly TableClient _tableClient;

  /// <summary>
  /// Initializes a new instance of the
  /// <see cref="AzureTableStorageSessionStore"/> class, creating the
  /// storage table if it does not exist.
  /// </summary>
  /// <param name="tableServiceClient">The Azure table service client.</param>
  /// <param name="options">The session store options.</param>
  /// <param name="scopeFactory">Creates the scopes the serialization formatter is resolved in.</param>
  /// <param name="timeProvider">The time provider used to find expired sessions.</param>
  /// <param name="logger">The logger.</param>
  /// <exception cref="ArgumentNullException">When any argument is <see langword="null"/>.</exception>
  public AzureTableStorageSessionStore(
    TableServiceClient tableServiceClient,
    AzureTableStorageOptions options,
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<AzureTableStorageSessionStore> logger)
  {
    ArgumentNullException.ThrowIfNull(tableServiceClient);
    ArgumentNullException.ThrowIfNull(options);

    _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _applicationDiscriminator = options.ApplicationDiscriminator;

    _tableClient = tableServiceClient.GetTableClient(options.TableName);
    _tableClient.CreateIfNotExists();
  }

  /// <inheritdoc/>
  public async ValueTask<Session?> GetSessionAsync(string key)
  {
    ArgumentNullException.ThrowIfNull(key);

    if (_sessions.TryGetValue(key, out var session))
      return session;

    LogLoadingSession(_logger, key, _tableClient.Name);
    var response = await _tableClient.GetEntityIfExistsAsync<TableEntity>(key, _applicationDiscriminator).ConfigureAwait(false);
    if (!response.HasValue || response.Value is null)
      return null;

    // Concurrent first reads of one key must share a single instance,
    // or changes made to the losing copy would be lost.
    return _sessions.GetOrAdd(key, Deserialize(ReadData(response.Value)));
  }

  /// <inheritdoc/>
  /// <exception cref="InvalidOperationException">The session serializes to more than the store can hold.</exception>
  public async ValueTask AddOrUpdateSessionAsync(string key, Session session)
  {
    ArgumentNullException.ThrowIfNull(key);
    ArgumentNullException.ThrowIfNull(session);

    // The lock keeps the table and the cache in the same order for one key.
    var keyLock = GetKeyLock(key);
    await keyLock.WaitAsync().ConfigureAwait(false);
    try
    {
      LogSavingSession(_logger, key, _tableClient.Name);
      await _tableClient.UpsertEntityAsync(CreateEntity(key, session), TableUpdateMode.Replace).ConfigureAwait(false);
      _sessions[key] = session;
    }
    finally
    {
      keyLock.Release();
    }
  }

  /// <inheritdoc/>
  public async ValueTask PurgeExpiredSessionsAsync(TimeSpan expiration)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(expiration.Ticks, nameof(expiration));

    var cutoff = _timeProvider.GetUtcNow().ToUnixTimeSeconds() - (long)expiration.TotalSeconds;

    // Purging is best effort: one failure should not stop the rest of the sweep.
    foreach (var item in _sessions)
    {
      if (item.Value.LastTouched < cutoff)
      {
        _sessions.TryRemove(item);
        continue;
      }
      try
      {
        await RefreshLastTouchedAsync(item.Key).ConfigureAwait(false);
      }
      catch (RequestFailedException ex)
      {
        LogRefreshFailed(_logger, ex, item.Key, _tableClient.Name);
      }
    }

    var filter = TableClient.CreateQueryFilter($"RowKey eq {_applicationDiscriminator} and LastTouched lt {cutoff}");
    var expired = _tableClient.QueryAsync<TableEntity>(filter, select: [nameof(TableEntity.PartitionKey), nameof(TableEntity.RowKey)]);

    await foreach (var entity in expired.ConfigureAwait(false))
    {
      if (_sessions.ContainsKey(entity.PartitionKey))
        continue;
      try
      {
        // The ETag makes the delete fail if the session was saved after the query.
        await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, entity.ETag).ConfigureAwait(false);
      }
      catch (RequestFailedException ex)
      {
        LogPurgeFailed(_logger, ex, entity.PartitionKey, _tableClient.Name);
      }
    }
  }

  // Writes only the timestamp, so the stored session data is never replaced
  // by a copy serialized while a request was changing it.
  private async Task RefreshLastTouchedAsync(string key)
  {
    var keyLock = GetKeyLock(key);
    await keyLock.WaitAsync().ConfigureAwait(false);
    try
    {
      if (!_sessions.TryGetValue(key, out var session))
        return;
      var entity = new TableEntity(key, _applicationDiscriminator) { [LastTouchedColumn] = session.LastTouched };
      await _tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Merge).ConfigureAwait(false);
    }
    finally
    {
      keyLock.Release();
    }
  }

  private SemaphoreSlim GetKeyLock(string key) =>
    _keyLocks[(uint)StringComparer.Ordinal.GetHashCode(key) % (uint)_keyLocks.Length];

  private TableEntity CreateEntity(string key, Session session)
  {
    var data = Serialize(session);
    if (data.Length > MaxChunks * ChunkSize)
      throw new InvalidOperationException(
        $"Session {key} serializes to {data.Length} bytes, more than the {MaxChunks * ChunkSize} bytes the Azure Table Storage session store can hold.");

    var entity = new TableEntity(key, _applicationDiscriminator) { [LastTouchedColumn] = session.LastTouched };
    var chunkCount = Math.Max(1, (data.Length + ChunkSize - 1) / ChunkSize);
    for (var index = 0; index < chunkCount; index++)
    {
      var offset = index * ChunkSize;
      entity[GetValueColumn(index)] = data.AsSpan(offset, Math.Min(ChunkSize, data.Length - offset)).ToArray();
    }
    return entity;
  }

  private static byte[] ReadData(TableEntity entity)
  {
    var chunks = new List<byte[]>();
    for (var index = 0; index < MaxChunks && entity.GetBinary(GetValueColumn(index)) is { } chunk; index++)
      chunks.Add(chunk);
    return chunks.Count == 1 ? chunks[0] : [.. chunks.SelectMany(c => c)];
  }

  // The first column keeps the name "Value", so single-column rows still read.
  private static string GetValueColumn(int index) => index == 0 ? ValueColumn : ValueColumn + index;

  // The formatter depends on the scoped ApplicationContext and keeps
  // per-call state, so each operation resolves its own in a new scope.
  private byte[] Serialize(Session session)
  {
    using var scope = _scopeFactory.CreateScope();
    return scope.ServiceProvider.GetRequiredService<ISerializationFormatter>().Serialize(session);
  }

  private Session Deserialize(byte[] data)
  {
    using var scope = _scopeFactory.CreateScope();
    return (Session)scope.ServiceProvider.GetRequiredService<ISerializationFormatter>().Deserialize(data)!;
  }

  [LoggerMessage(Level = LogLevel.Debug, Message = "Loading session {Key} from table {Table}")]
  private static partial void LogLoadingSession(ILogger logger, string key, string table);

  [LoggerMessage(Level = LogLevel.Debug, Message = "Saving session {Key} to table {Table}")]
  private static partial void LogSavingSession(ILogger logger, string key, string table);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to refresh the last-touched time of session {Key} in table {Table}")]
  private static partial void LogRefreshFailed(ILogger logger, Exception exception, string key, string table);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to purge expired session {Key} from table {Table}")]
  private static partial void LogPurgeFailed(ILogger logger, Exception exception, string key, string table);
}
