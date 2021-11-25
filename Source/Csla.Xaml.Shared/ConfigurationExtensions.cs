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
    public static ICslaConfiguration AddXaml(this ICslaConfiguration config)
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
    public static ICslaConfiguration AddXaml(this ICslaConfiguration config, Action<XamlOptions> options)
    {
      var xamlOptions = new XamlOptions();
      options?.Invoke(xamlOptions);

      // use correct mode for raising PropertyChanged events
      ConfigurationManager.AppSettings["CslaPropertyChangedMode"] = Csla.ApplicationContext.PropertyChangedModes.Xaml.ToString();

      config.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      return config;
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