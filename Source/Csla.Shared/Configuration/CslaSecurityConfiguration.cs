//-----------------------------------------------------------------------
// <copyright file="CslaSecurityConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Use this type to configure the settings for security</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for security.
  /// </summary>
  public class CslaSecurityConfiguration
  {
    private CslaConfiguration RootConfiguration { get; set; }

    internal CslaSecurityConfiguration(CslaConfiguration root)
    {
      RootConfiguration = root;
    }

    /// <summary>
    /// Sets the max size of the PrincipalCache cache.
    /// </summary>
    /// <param name="size">Max cache size</param>
    public CslaConfiguration PrincipalCacheMaxCacheSize(int size)
    {
      Security.PrincipalCache.MaxCacheSize = size;
      return RootConfiguration;
    }
  }
}
