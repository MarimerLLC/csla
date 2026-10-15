//-----------------------------------------------------------------------
// <copyright file="SessionEntity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Azure Table Storage entity for a stored session</summary>
//-----------------------------------------------------------------------

using Azure;
using Azure.Data.Tables;

namespace Csla.Blazor.State.AzureTableStorage;

/// <summary>
/// Azure Table Storage entity representing a single stored
/// <see cref="Csla.State.Session"/>.
/// </summary>
/// <remarks>
/// The <see cref="PartitionKey"/> holds the session key and the
/// <see cref="RowKey"/> holds the application discriminator, so that
/// multiple applications can safely share a single storage table.
/// </remarks>
internal sealed class SessionEntity : ITableEntity
{
  /// <summary>
  /// Gets or sets the partition key (the session key).
  /// </summary>
  public required string PartitionKey { get; set; }

  /// <summary>
  /// Gets or sets the row key (the application discriminator).
  /// </summary>
  public required string RowKey { get; set; }

  /// <summary>
  /// Gets or sets the entity timestamp (managed by Azure).
  /// </summary>
  public DateTimeOffset? Timestamp { get; set; }

  /// <summary>
  /// Gets or sets the entity ETag (managed by Azure).
  /// </summary>
  public ETag ETag { get; set; } = ETag.All;

  /// <summary>
  /// Gets or sets the serialized session data.
  /// </summary>
  public required byte[] Value { get; set; }

  /// <summary>
  /// Gets or sets the Unix-seconds timestamp of the last time
  /// the session was touched (used for expiration purging).
  /// </summary>
  public required long LastTouched { get; set; }
}
