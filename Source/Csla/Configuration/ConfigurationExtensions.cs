#if NET462_OR_GREATER || NETSTANDARD2_0 || NET6_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for base .NET configuration</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
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
      services.AddScoped((p) => cslaOptions);

      // ApplicationContext defaults
      services.AddScoped<ApplicationContext>();
      RegisterContextManager(services);

      // Runtime Info defaults
      services.TryAddScoped(typeof(IRuntimeInfo), typeof(RuntimeInfo));

      cslaOptions.AddRequiredDataPortalServices();

      // Default to using LocalProxy and local data portal
      var proxyInit = services.Where(i => i.ServiceType.Equals(typeof(IDataPortalProxy))).Any();
      if (!proxyInit)
      {
        cslaOptions.DataPortal((options) => options.UseLocalProxy());
      }

      return services;
    }

    private static void RegisterContextManager(IServiceCollection services)
    {
      services.AddScoped<Core.ApplicationContextAccessor>();
      services.TryAddScoped(typeof(Core.IContextManagerLocal), typeof(Core.ApplicationContextManagerAsyncLocal));

      var contextManagerType = typeof(Core.IContextManager);

      var managerInit = services.Where(i => i.ServiceType.Equals(contextManagerType)).Any();
      if (managerInit) return;

      if (LoadContextManager(services, "Csla.Blazor.WebAssembly.ApplicationContextManager, Csla.Blazor.WebAssembly")) return;
      if (LoadContextManager(services, "Csla.Xaml.ApplicationContextManager, Csla.Xaml")) return;
      if (LoadContextManager(services, "Csla.Web.Mvc.ApplicationContextManager, Csla.Web.Mvc")) return;
      if (LoadContextManager(services, "Csla.Web.ApplicationContextManager, Csla.Web")) return;
      if (LoadContextManager(services, "Csla.Windows.Forms.ApplicationContextManager, Csla.Windows.Forms")) return;

      // default to AsyncLocal context manager
      services.AddScoped(contextManagerType, typeof(Core.ApplicationContextManager));
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