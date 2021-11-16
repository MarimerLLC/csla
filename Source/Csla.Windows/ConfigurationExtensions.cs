//-----------------------------------------------------------------------
// <copyright file="WindowsConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for Windows Forms
  /// </summary>
  public static class WindowsConfigurationExtensions
  {
    /// <summary>
    /// Registers services necessary for Windows Forms
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <returns></returns>
    public static ICslaConfiguration WithWindowsForms(this ICslaConfiguration config)
    {
      config.Services.TryAddScoped(typeof(Csla.Core.IContextManager), typeof(Csla.Windows.ApplicationContextManager));
      return config;
    }

    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="builder">IHostBuilder instance</param>
    public static IHostBuilder UseCsla(this IHostBuilder builder)
    {
      return builder;
    }
  }
}
