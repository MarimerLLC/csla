#if NET462_OR_GREATER || NETSTANDARD2_0 || NET6_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for base .NET configuration</summary>
//-----------------------------------------------------------------------
using Csla.Core;
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
      // Custom configuration
      var cslaOptions = new CslaOptions(services);
      options?.Invoke(cslaOptions);

      // capture options object
      services.AddScoped(_ => cslaOptions);
      services.AddScoped(_ => cslaOptions.DataPortalOptions);
      services.AddScoped(_ => cslaOptions.SecurityOptions);

      // ApplicationContext defaults
      services.AddScoped<ApplicationContext>();
      RegisterContextManager(services, cslaOptions.ContextManagerType);
      if (cslaOptions.ContextManagerType != null)
        services.AddScoped(typeof(Csla.Core.IContextManager), cslaOptions.ContextManagerType);

      // Runtime Info defaults
      services.TryAddScoped(typeof(IRuntimeInfo), typeof(RuntimeInfo));

      services.AddScoped(typeof(IDataPortalCache), cslaOptions.DataPortalOptions.DataPortalClientOptions.DataPortalCacheType);
      cslaOptions.AddRequiredDataPortalServices(services);

      // Default to using LocalProxy and local data portal
      var proxyInit = services.Any(i => i.ServiceType.Equals(typeof(IDataPortalProxy)));
      if (!proxyInit)
      {
        cslaOptions.DataPortal((options) => options.DataPortalClientOptions.UseLocalProxy());
      }

      return services;
    }

    private static void RegisterContextManager(IServiceCollection services, Type contextManagerType)
    {
      services.AddScoped<Core.ApplicationContextAccessor>();
      services.TryAddScoped(typeof(Core.IContextManagerLocal), typeof(Core.ApplicationContextManagerAsyncLocal));

      var managerInit = services.Any(i => i.ServiceType.Equals(typeof(IContextManager)));
      if (managerInit) return;

      if (contextManagerType != null)
      {
        services.AddScoped(typeof(Core.IContextManager), contextManagerType);
      }
      else
      {
        if (LoadContextManager(services, "Csla.Blazor.WebAssembly.ApplicationContextManager, Csla.Blazor.WebAssembly")) return;
        if (LoadContextManager(services, "Csla.Xaml.ApplicationContextManager, Csla.Xaml")) return;
        if (LoadContextManager(services, "Csla.Web.Mvc.ApplicationContextManager, Csla.Web.Mvc")) return;
        if (LoadContextManager(services, "Csla.Web.ApplicationContextManager, Csla.Web")) return;
        if (LoadContextManager(services, "Csla.Windows.Forms.ApplicationContextManager, Csla.Windows.Forms")) return;

        // default to AsyncLocal context manager
        services.AddScoped(typeof(Core.IContextManager), typeof(Core.ApplicationContextManager));
      }
    }

    private static bool LoadContextManager(IServiceCollection services, string managerTypeName)
    {
      var managerType = Type.GetType(managerTypeName, false);
      if (managerType != null)
      {
        services.AddScoped(typeof(Core.IContextManager), managerType);
        return true;
      }
      return false;
    }
  }
}
#endif