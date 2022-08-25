//-----------------------------------------------------------------------
// <copyright file="CslaDataConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for CslaDataConfiguration
  /// </summary>
  public static class DataConfigurationExtensions
  {
    /// <summary>
    /// Extension method for CslaDataConfiguration
    /// </summary>
    public static CslaOptions Data(this CslaOptions config, Action<DataOptions> options)
    {
      options?.Invoke(config.DataOptions);
      return config;
    }
  }
}
