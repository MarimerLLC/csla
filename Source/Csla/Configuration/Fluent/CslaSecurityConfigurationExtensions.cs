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
  /// Extension method for CslaSecurityConfiguration
  /// </summary>
  public static class CslaSecurityConfigurationExtensions
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

  /// <summary>
  /// Use this type to configure the settings for security.
  /// </summary>
  public class SecurityOptions
  {
    /// <summary>
    /// Sets the max size of the PrincipalCache cache.
    /// </summary>
    /// <param name="size">Max cache size</param>
    public SecurityOptions PrincipalCacheMaxCacheSize(int size)
    {
      ConfigurationManager.AppSettings["CslaPrincipalCacheSize"] = size.ToString();
      return this;
    }
  }
}
