//-----------------------------------------------------------------------
// <copyright file="AzureTableStorageSessionStore.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Azure Table Storage implementation of ISessionStore</summary>
//-----------------------------------------------------------------------

using Azure;
using Azure.Data.Tables;
using Csla.Serialization;
using Csla.State;
using Microsoft.Extensions.Logging;

namespace Csla.Blazor.State.AzureTableStorage;

/// <summary>
/// Azure Table Storage implementation of <see cref="ISessionStore"/>.
/// </summary>
/// <remarks>
/// Read and write failures propagate to the caller. A failed read must not
/// look like a missing session, because the session manager would then
/// replace the stored session with a new, empty one.
/// </remarks>
public class AzureTableStorageSessionStore : ISessionStore
{
  private readonly string _applicationDiscriminator;
  private readonly ISerializationFormatter _formatter;
  private readonly ILogger<AzureTableStorageSessionStore> _logger;
  private readonly TableClient _tableClient;

  /// <summary>
  /// Initializes a new instance of the
  /// <see cref="AzureTableStorageSessionStore"/> class, creating the
  /// storage table if it does not exist.
  /// </summary>
  /// <param name="tableServiceClient">The Azure table service client.</param>
  /// <param name="formatter">The CSLA serialization formatter.</param>
  /// <param name="options">The session store options.</param>
  /// <param name="logger">The logger.</param>
  /// <exception cref="ArgumentNullException">When any argument is <see langword="null"/>.</exception>
  public AzureTableStorageSessionStore(
    TableServiceClient tableServiceClient,
    ISerializationFormatter formatter,
    AzureTableStorageOptions options,
    ILogger<AzureTableStorageSessionStore> logger)
  {
    ArgumentNullException.ThrowIfNull(tableServiceClient);
    ArgumentNullException.ThrowIfNull(options);

    _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _applicationDiscriminator = options.ApplicationDiscriminator;

    _tableClient = tableServiceClient.GetTableClient(options.TableName);
    _tableClient.CreateIfNotExists();
  }

  /// <inheritdoc/>
  public async ValueTask<Session?> GetSessionAsync(string key)
  {
    ArgumentNullException.ThrowIfNull(key);

    _logger.LogDebug("Retrieving session {Key} from table {Table}", key, _tableClient.Name);

    var response = await _tableClient.GetEntityIfExistsAsync<SessionEntity>(key, _applicationDiscriminator).ConfigureAwait(false);
    if (!response.HasValue || response.Value is null)
      return null;
    return (Session?)_formatter.Deserialize(response.Value.Value);
  }

  /// <inheritdoc/>
  public async ValueTask AddOrUpdateSessionAsync(string key, Session session)
  {
    ArgumentNullException.ThrowIfNull(key);
    ArgumentNullException.ThrowIfNull(session);

    _logger.LogDebug("Saving session {Key} to table {Table}", key, _tableClient.Name);

    var entity = new SessionEntity
    {
      PartitionKey = key,
      RowKey = _applicationDiscriminator,
      Value = _formatter.Serialize(session),
      LastTouched = session.LastTouched,
    };
    await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace).ConfigureAwait(false);
  }

  /// <inheritdoc/>
  public async ValueTask PurgeExpiredSessionsAsync(TimeSpan expiration)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(expiration.Ticks, nameof(expiration));

    var cutoff = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (long)expiration.TotalSeconds;
    var discriminator = _applicationDiscriminator;

    var expired = _tableClient.QueryAsync<SessionEntity>(
      x => x.RowKey == discriminator && x.LastTouched < cutoff,
      select: [nameof(SessionEntity.PartitionKey), nameof(SessionEntity.RowKey)]);

    await foreach (var entity in expired.ConfigureAwait(false))
    {
      // Purging is best effort: another instance may already have removed
      // the session, and one failure should not stop the rest of the sweep.
      try
      {
        await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey).ConfigureAwait(false);
      }
      catch (RequestFailedException ex)
      {
        _logger.LogWarning(ex, "Failed to purge expired session {Key} from table {Table}", entity.PartitionKey, _tableClient.Name);
      }
    }
  }
}
