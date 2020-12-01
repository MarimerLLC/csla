//-----------------------------------------------------------------------
// <copyright file="CslaSecurityConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
      return new CslaSecurityConfiguration();
    }
  }

  /// <summary>
  /// Use this type to configure the settings for security.
  /// </summary>
  public class CslaSecurityConfiguration
  {
    /// <summary>
    /// Sets the max size of the PrincipalCache cache.
    /// </summary>
    /// <param name="size">Max cache size</param>
    public CslaSecurityConfiguration PrincipalCacheMaxCacheSize(int size)
    {
      ConfigurationManager.AppSettings["CslaPrincipalCacheSize"] = size.ToString();
      return this;
    }
  }
}
