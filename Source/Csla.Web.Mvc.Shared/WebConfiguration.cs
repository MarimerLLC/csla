#if NETSTANDARD2_0
//-----------------------------------------------------------------------
// <copyright file="WebConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Extension methods for ASP.NET Core configuration</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Csla.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Web
{
  /// <summary>
  /// Extension methods for ASP.NET Core configuration.
  /// </summary>
  public static class WebConfiguration
  {
    /// <summary>
    /// Configure CSLA .NET options for ASP.NET Core.
    /// </summary>
    /// <param name="services">ASP.NET services</param>
    /// <param name="setupAction">Setup action</param>
    /// <returns></returns>
    public static IServiceCollection ConfigureCsla(this IServiceCollection services, Action<CslaOptions, IServiceProvider> setupAction = null)
    {
      services.AddSingleton<CslaOptions>((sp) =>
      {
        var options = new CslaOptions();
        setupAction?.Invoke(options, sp);
        return options;
      });

      return services;
    }

    /// <summary>
    /// CSLA .NET application builder.
    /// </summary>
    /// <param name="appBuilder">Application builder</param>
    /// <returns>Application builder</returns>
    public static IApplicationBuilder UseCsla(this IApplicationBuilder appBuilder)
    {
      // grab the options
      var options = appBuilder.ApplicationServices.GetRequiredService<CslaOptions>();

      // configure csla according to options.
      Csla.Server.FactoryDataPortal.FactoryLoader = options.ObjectFactoryLoader;
      Csla.ApplicationContext.WebContextManager = options.WebContextManager ?? new ApplicationContextMananger(appBuilder.ApplicationServices);

      return appBuilder;
    }

    /// <summary>
    /// CSLA .NET configuration options.
    /// </summary>
    public class CslaOptions
    {
      /// <summary>
      /// Gets or sets the object factory loader.
      /// </summary>
      public IObjectFactoryLoader ObjectFactoryLoader { get; set; }

      /// <summary>
      /// Gets or sets the web application context manager.
      /// </summary>
      public IContextManager WebContextManager { get; set; }

    }
  }
}
#endif