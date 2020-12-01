//-----------------------------------------------------------------------
// <copyright file="PrincipalCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides a cache for a limited number of</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Csla.Security
{
  /// <summary>
  /// Provides a cache for a limited number of
  /// principal objects at the AppDomain level.
  /// </summary>
  public static class PrincipalCache
  {
    private static List<IPrincipal> _cache = new List<IPrincipal>();

    private static int _maxCacheSize;

    /// <summary>
    /// Gets the maximum cache size
    /// </summary>
    public static int MaxCacheSize
    {
      get
      {
        if (_maxCacheSize == 0)
        {
          string tmp = Csla.Configuration.ConfigurationManager.AppSettings["CslaPrincipalCacheSize"];
          if (string.IsNullOrEmpty(tmp))
            _maxCacheSize = 10;
          else
            _maxCacheSize = Convert.ToInt32(tmp);
        }
        return _maxCacheSize;
      }
      internal set
      {
        _maxCacheSize = value;
      }
    }

    /// <summary>
    /// Gets a principal from the cache based on
    /// the identity name. If no match is found null
    /// is returned.
    /// </summary>
    /// <param name="name">
    /// The identity name associated with the principal.
    /// </param>
    public static IPrincipal GetPrincipal(string name)
    {
      lock (_cache)
      {
        foreach (IPrincipal item in _cache)
          if (item.Identity.Name == name)
            return item;
        return null;
      }
    }

    /// <summary>
    /// Adds a principal to the cache.
    /// </summary>
    /// <param name="principal">
    /// IPrincipal object to be added.
    /// </param>
    public static void AddPrincipal(IPrincipal principal)
    {
      lock (_cache)
      {
        if (!_cache.Contains(principal))
        {
          _cache.Add(principal);
          if (_cache.Count > MaxCacheSize)
            _cache.RemoveAt(0);
        }
      }
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public static void Clear()
    {
      lock (_cache)
        _cache.Clear();
    }
  }
}
