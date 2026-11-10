//-----------------------------------------------------------------------
// <copyright file="AzureTableStorageOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for the Azure Table Storage session store</summary>
//-----------------------------------------------------------------------

namespace Csla.Blazor.State.AzureTableStorage;

/// <summary>
/// Options for configuring the Azure Table Storage
/// <see cref="Csla.State.ISessionStore"/> implementation.
/// </summary>
public class AzureTableStorageOptions
{
  /// <summary>
  /// Gets or sets the Azure Storage connection string used to
  /// create the <see cref="Azure.Data.Tables.TableServiceClient"/>.
  /// </summary>
  /// <remarks>
  /// Leave <see langword="null"/> or empty to use a
  /// <see cref="Azure.Data.Tables.TableServiceClient"/> that is already
  /// registered in the dependency injection container (for example, one
  /// registered by .NET Aspire or by managed-identity wiring).
  /// </remarks>
  public string? ConnectionString { get; set; }

  /// <summary>
  /// Gets or sets the dependency injection service key used to resolve a
  /// keyed <see cref="Azure.Data.Tables.TableServiceClient"/>.
  /// </summary>
  /// <remarks>
  /// Set this when the host registered the client as a keyed service (for
  /// example <c>builder.AddKeyedAzureTableClient("state")</c>). Ignored when
  /// <see cref="ConnectionString"/> is supplied.
  /// </remarks>
  public object? ServiceKey { get; set; }

  /// <summary>
  /// Gets or sets the name of the Azure Storage table used to store
  /// sessions. Defaults to <c>CslaStateSessionManager</c>.
  /// </summary>
  public string TableName { get; set; } = "CslaStateSessionManager";

  /// <summary>
  /// Gets or sets a value used to discriminate sessions belonging to this
  /// application when multiple applications share the same storage table.
  /// Defaults to an empty string.
  /// </summary>
  public string ApplicationDiscriminator { get; set; } = string.Empty;
}
