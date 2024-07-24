//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for base .NET configuration</summary>
//-----------------------------------------------------------------------
using Csla.DataPortalClient;
using Csla.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for base .NET configuration
  /// </summary>
  public static class ConfigurationExtensions
  {
    /// <summary>
    /// Add CSLA .NET services for use by the application.
    /// </summary>
    /// <param name="services">ServiceCollection object</param>
    public static IServiceCollection AddCsla(this IServiceCollection services)
    {
      return AddCsla(services, null);
    }

    /// <summary>
    /// Add CSLA .NET services for use by the application.
    /// </summary>
    /// <param name="services">ServiceCollection object</param>
    /// <param name="options">Options for configuring CSLA .NET</param>
    public static IServiceCollection AddCsla(this IServiceCollection services, Action<CslaOptions> options)
    {
      // ApplicationContext defaults
      services.AddScoped<Core.ApplicationContextAccessor>();
      services.AddScoped(typeof(Core.IContextManagerLocal), typeof(Core.ApplicationContextManagerAsyncLocal));
      services.AddScoped<ApplicationContext>();

      // Custom configuration
      var cslaOptions = new CslaOptions(services);
      options?.Invoke(cslaOptions);

      // capture options objects
      services.AddScoped(_ => cslaOptions);
      services.AddScoped(_ => cslaOptions.DataPortalOptions);
      services.AddScoped(_ => cslaOptions.SecurityOptions);

      // Runtime Info defaults
      services.TryAddScoped(typeof(IRuntimeInfo), typeof(RuntimeInfo));

      services.AddScoped(typeof(IDataPortalCache), cslaOptions.DataPortalOptions.DataPortalClientOptions.DataPortalCacheType);
      cslaOptions.AddRequiredDataPortalServices(services);

      // Default to using LocalProxy and local data portal
      var proxyInit = services.Any(i => i.ServiceType.Equals(typeof(IDataPortalProxy)));
      if (!proxyInit)
      {
        cslaOptions.DataPortal(options => options.DataPortalClientOptions.UseLocalProxy());
      }

      // Default to using MobileFormatter
      if (!services.Any(_ => _.ServiceType == typeof(Serialization.ISerializationFormatter)))
        cslaOptions.Serialization(o => o.UseMobileFormatter());

      return services;
    }
  }
}
