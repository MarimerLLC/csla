//-----------------------------------------------------------------------
// <copyright file="CslaSecurityConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for security</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for Csla Security Configuration
  /// </summary>
  public static class SecurityConfigurationExtensions
  {
    /// <summary>
    /// Extension method for CslaSecurityConfiguration
    /// </summary>
    public static CslaOptions Security(this CslaOptions config, Action<SecurityOptions> options)
    {
      options?.Invoke(config.SecurityOptions);
      return config;
    }
  }
}
