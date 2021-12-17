//-----------------------------------------------------------------------
// <copyright file="SecurityOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for security</summary>
//-----------------------------------------------------------------------
namespace Csla.Configuration
{
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
      Security.PrincipalCache.MaxCacheSize = size;
      return this;
    }
  }
}
