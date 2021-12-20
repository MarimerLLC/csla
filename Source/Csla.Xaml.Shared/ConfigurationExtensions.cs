#if !XAMARIN && !NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="XamlConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Csla.Xaml;
using Microsoft.Extensions.Hosting;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for Xaml
  /// </summary>
  public static class XamlConfigurationExtensions
  {
    /// <summary>
    /// Registers services necessary for Xaml-based
    /// environments.
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <returns></returns>
    public static CslaOptions AddXaml(this CslaOptions config)
    {
      return AddXaml(config, null);
    }

    /// <summary>
    /// Registers services necessary for Xaml-based
    /// environments.
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <param name="options">XamlOptions action</param>
    /// <returns></returns>
    public static CslaOptions AddXaml(this CslaOptions config, Action<XamlOptions> options)
    {
      var xamlOptions = new XamlOptions();
      options?.Invoke(xamlOptions);

      // use correct mode for raising PropertyChanged events
      ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;

      config.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      return config;
    }

    /// <summary>
    /// Initializes CSLA for use by Xaml apps.
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static IHost UseCsla(this IHost host)
    {
      // create instance of ApplicationContext so the
      // Csla.Xaml.ApplicationContextManager gets a static
      // reference for use by UI helpers.
#pragma warning disable IDE0059 // Unnecessary assignment of a value
      var context = host.Services.GetService(typeof(ApplicationContext));
#pragma warning restore IDE0059 // Unnecessary assignment of a value
      return host;
    }
  }

  /// <summary>
  /// Configuration options for AddXaml method
  /// </summary>
  public class XamlOptions
  {

  }
}
#endif