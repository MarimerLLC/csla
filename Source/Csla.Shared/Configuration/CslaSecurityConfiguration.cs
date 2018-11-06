//-----------------------------------------------------------------------
// <copyright file="CslaSecurityConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Use this type to configure the settings for security</summary>
//-----------------------------------------------------------------------
namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for CslaSecurityConfiguration
  /// </summary>
  public static class CslaSecurityConfigurationExtension
  {
    /// <summary>
    /// Extension method for CslaSecurityConfiguration
    /// </summary>
    public static CslaSecurityConfiguration Security(this ICslaConfiguration config)
    {
      return new CslaSecurityConfiguration(config);
    }
  }

  /// <summary>
  /// Use this type to configure the settings for security.
  /// </summary>
  public class CslaSecurityConfiguration
  {
    private ICslaConfiguration RootConfiguration { get; set; }

    internal CslaSecurityConfiguration(ICslaConfiguration root)
    {
      RootConfiguration = root;
    }

    /// <summary>
    /// Sets the max size of the PrincipalCache cache.
    /// </summary>
    /// <param name="size">Max cache size</param>
    public ICslaConfiguration PrincipalCacheMaxCacheSize(int size)
    {
      Security.PrincipalCache.MaxCacheSize = size;
      return RootConfiguration;
    }
  }
}
