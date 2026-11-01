//-----------------------------------------------------------------------
// <copyright file="AzureTableStorageSessionStore.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Azure Table Storage implementation of ISessionStore</summary>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Net;
using Azure;
using Azure.Data.Tables;
using Csla.Properties;
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
/// Live sessions are kept in memory and returned by reference, so callers
/// can change a session in place. The table is read only when a session is
/// not in memory, such as after a restart.
/// </para>
/// <para>
/// Session data is written to the table only by
/// <see cref="AddOrUpdateSessionAsync"/>. <see cref="PurgeExpiredSessionsAsync"/>
/// refreshes only the stored last-touched time of live sessions, so it never
/// serializes a session that a request may be changing.
/// </para>
/// <para>
/// Read and write failures propagate to the caller, so a failed read is never
/// mistaken for a missing session.
/// </para>
/// </remarks>
internal sealed partial class AzureTableStorageSessionStore : ISessionStore, IDisposable
{
  // Azure Tables limits a binary property to 64 KiB, so session data is split
  // across several columns of this size.
  internal const int ChunkSize = 64 * 1024;
  // The 1 MiB entity limit can be measured on the request, where binary
  // values are base64 encoded (Azurite measures it that way), so 11 chunks
  // (704 KiB) is the most that fits however it is measured.
  internal const int MaxChunks = 11;
  private const string ValueColumn = "Value";
  private const string LastTouchedColumn = nameof(Session.LastTouched);

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

    LogLoadingSession(key, _tableClient.Name);
    var response = await _tableClient.GetEntityIfExistsAsync<TableEntity>(key, _applicationDiscriminator).ConfigureAwait(false);
    if (!response.HasValue || response.Value is null)
      return null;

    // Callers that load the same key at the same time all get the instance
    // that is cached first, so they all change the same session in place.
    var entity = response.Value;
    return _sessions.GetOrAdd(key, _ => Deserialize(ReadData(entity)));
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
      LogSavingSession(key, _tableClient.Name);
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
        LogRefreshFailed(ex, item.Key, _tableClient.Name);
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
      catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.PreconditionFailed)
      {
        // Expected: the session was saved after the query, so it is still in use.
        LogPurgeSkipped(entity.PartitionKey, _tableClient.Name);
      }
      catch (RequestFailedException ex)
      {
        LogPurgeFailed(ex, entity.PartitionKey, _tableClient.Name);
      }
    }
  }

  /// <summary>
  /// Releases the locks used to coordinate writes.
  /// </summary>
  public void Dispose()
  {
    foreach (var keyLock in _keyLocks)
      keyLock.Dispose();
  }

  // Writes only the timestamp, so the stored session data is never replaced
  // by a copy serialized while a request was changing it.
  private async Task RefreshLastTouchedAsync(string key)
  {
    var keyLock = GetKeyLock(key);
    await keyLock.WaitAsync().ConfigureAwait(false);
    try
    {
      // Read the session under the lock, so the timestamp written is that of
      // the session the last AddOrUpdateSessionAsync stored.
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
        string.Format(Resources.SessionStoreSessionTooLarge, key, data.Length, MaxChunks * ChunkSize));

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
  private partial void LogLoadingSession(string key, string table);

  [LoggerMessage(Level = LogLevel.Debug, Message = "Saving session {Key} to table {Table}")]
  private partial void LogSavingSession(string key, string table);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to refresh the last-touched time of session {Key} in table {Table}")]
  private partial void LogRefreshFailed(Exception exception, string key, string table);

  [LoggerMessage(Level = LogLevel.Debug, Message = "Kept expired session {Key} in table {Table}, because it was saved after the purge query")]
  private partial void LogPurgeSkipped(string key, string table);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to purge expired session {Key} from table {Table}")]
  private partial void LogPurgeFailed(Exception exception, string key, string table);
}
