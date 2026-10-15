//-----------------------------------------------------------------------
// <copyright file="AzureTableStorageConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Configuration extensions for the Azure Table Storage session store</summary>
//-----------------------------------------------------------------------

using Azure.Data.Tables;
using Csla.Blazor.State.AzureTableStorage;
using Csla.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods to register the Azure Table Storage
  /// session store with the CSLA configuration system.
  /// </summary>
  public static class AzureTableStorageConfigurationExtensions
  {
    /// <summary>
    /// Configures CSLA to use the Azure Table Storage
    /// <see cref="ISessionStore"/> implementation.
    /// </summary>
    /// <param name="config">The CslaOptions instance.</param>
    /// <param name="options">An optional delegate to configure the store.</param>
    /// <returns>The CslaOptions instance.</returns>
    /// <remarks>
    /// Call this after <c>AddServerSideBlazor</c>, which registers the default
    /// in-memory session store; the last registration wins. The session store
    /// is only used when <c>UseInMemoryApplicationContextManager</c> is
    /// <see langword="false"/>. Call <c>ISessionManager.PurgeSessions</c>
    /// periodically: besides removing expired sessions, it saves changes made
    /// to live sessions to the table.
    /// <para>
    /// The <see cref="TableServiceClient"/> comes from
    /// <see cref="AzureTableStorageOptions.ConnectionString"/> when it is set,
    /// otherwise from the keyed service named by
    /// <see cref="AzureTableStorageOptions.ServiceKey"/>, otherwise from the
    /// default <see cref="TableServiceClient"/> registered in the container.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions UseAzureTableStorageSessionStore(this CslaOptions config, Action<AzureTableStorageOptions>? options = null)
    {
      ArgumentNullException.ThrowIfNull(config);

      var storeOptions = new AzureTableStorageOptions();
      options?.Invoke(storeOptions);

      // Singletons, because the singleton SessionManager depends on ISessionStore.
      config.Services.AddSingleton(storeOptions);
      config.Services.TryAddSingleton(TimeProvider.System);

      // The client is resolved privately rather than registered, so the store
      // never changes which TableServiceClient the rest of the app receives.
      config.Services.AddSingleton<ISessionStore>(sp =>
        ActivatorUtilities.CreateInstance<AzureTableStorageSessionStore>(
          sp, GetTableServiceClient(sp, sp.GetRequiredService<AzureTableStorageOptions>())));
      return config;
    }

    internal static TableServiceClient GetTableServiceClient(IServiceProvider serviceProvider, AzureTableStorageOptions options)
    {
      if (!string.IsNullOrEmpty(options.ConnectionString))
        return new TableServiceClient(options.ConnectionString);
      if (options.ServiceKey is not null)
        return serviceProvider.GetRequiredKeyedService<TableServiceClient>(options.ServiceKey);
      return serviceProvider.GetRequiredService<TableServiceClient>();
    }
  }
}
