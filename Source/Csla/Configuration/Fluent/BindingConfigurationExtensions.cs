//-----------------------------------------------------------------------
// <copyright file="CslaDataConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Configure data binding settings</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for data binding
  /// </summary>
  public static class BindingConfigurationExtensions
  {
    /// <summary>
    /// Extension method for CslaDataConfiguration
    /// </summary>
    public static CslaOptions Binding(this CslaOptions config, Action<BindingOptions> options)
    {
      options?.Invoke(config.BindingOptions);
      return config;
    }
  }
}
