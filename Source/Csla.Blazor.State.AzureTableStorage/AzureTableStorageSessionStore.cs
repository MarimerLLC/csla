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
/// Sessions are written to the table by <see cref="AddOrUpdateSessionAsync"/>,
/// and every live session is written again by
/// <see cref="PurgeExpiredSessionsAsync"/>, which saves changes made in place
/// and refreshes the stored last-touched time.
/// </para>
/// <para>
/// Read and write failures propagate to the caller. A failed read must not
/// look like a missing session, because the session manager would then
/// replace the stored session with a new, empty one.
/// </para>
/// </remarks>
internal sealed partial class AzureTableStorageSessionStore : ISessionStore
{
  private readonly ConcurrentDictionary<string, Session> _sessions = [];
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
    var response = await _tableClient.GetEntityIfExistsAsync<SessionEntity>(key, _applicationDiscriminator).ConfigureAwait(false);
    if (!response.HasValue || response.Value is null)
      return null;

    // Concurrent first reads of one key must share a single instance,
    // or changes made to the losing copy would be lost.
    return _sessions.GetOrAdd(key, Deserialize(response.Value.Value));
  }

  /// <inheritdoc/>
  public async ValueTask AddOrUpdateSessionAsync(string key, Session session)
  {
    ArgumentNullException.ThrowIfNull(key);
    ArgumentNullException.ThrowIfNull(session);

    _sessions[key] = session;
    await SaveAsync(key, session).ConfigureAwait(false);
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
        await SaveAsync(item.Key, item.Value).ConfigureAwait(false);
      }
      catch (Exception ex) when (ex is RequestFailedException or InvalidOperationException)
      {
        // InvalidOperationException: a request changed the session while it was being serialized.
        LogSaveFailed(_logger, ex, item.Key, _tableClient.Name);
      }
    }

    var discriminator = _applicationDiscriminator;
    var expired = _tableClient.QueryAsync<SessionEntity>(
      x => x.RowKey == discriminator && x.LastTouched < cutoff,
      select: [nameof(SessionEntity.PartitionKey), nameof(SessionEntity.RowKey)]);

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

  private async Task SaveAsync(string key, Session session)
  {
    LogSavingSession(_logger, key, _tableClient.Name);
    var entity = new SessionEntity
    {
      PartitionKey = key,
      RowKey = _applicationDiscriminator,
      Value = Serialize(session),
      LastTouched = session.LastTouched,
    };
    await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace).ConfigureAwait(false);
  }

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

  [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to save session {Key} to table {Table} during purge")]
  private static partial void LogSaveFailed(ILogger logger, Exception exception, string key, string table);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to purge expired session {Key} from table {Table}")]
  private static partial void LogPurgeFailed(ILogger logger, Exception exception, string key, string table);
}
