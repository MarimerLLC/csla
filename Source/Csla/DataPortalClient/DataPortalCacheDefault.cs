//-----------------------------------------------------------------------
// <copyright file="IDataPortalCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Null implementation of a client-side cache service</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Csla.Server;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Null implementation of a client-side cache service.
  /// </summary>
  public class DataPortalCacheDefault : IDataPortalCache
  {
    /// <summary>
    /// Always returns success, does not cache values.
    /// </summary>
    /// <param name="objectType">Type of domain object to add</param>
    /// <param name="criteria">Criteria for domain type being added</param>
    /// <param name="operation">Data portal operation</param>
    /// <param name="result">Data portal result to cache</param>
    /// <returns></returns>
    public Task AddObject(Type objectType, object criteria, DataPortalOperations operation, DataPortalResult result)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a value indicating whether the domain type 
    /// can be cached.
    /// </summary>
    /// <param name="objectType">Type of domain object to add</param>
    /// <param name="criteria">Criteria for domain type being added</param>
    /// <param name="operation">Data portal operation</param>
    /// <returns></returns>
    public bool IsCacheable(Type objectType, object criteria, DataPortalOperations operation) => false;

    /// <summary>
    /// Always returns false, does not retrieve values from cache.
    /// </summary>
    /// <param name="objectType">Type of domain object to retrieve</param>
    /// <param name="criteria">Criteria for domain type being retrieved</param>
    /// <param name="operation">Data portal operation</param>
    /// <param name="result">Cached data portal result</param>
    /// <returns>true if success, false if object isn't returned</returns>
    public Task<bool> TryGetObject(Type objectType, object criteria, DataPortalOperations operation, out DataPortalResult result)
    { 
      result = null;
      return Task.FromResult(false);
    }
  }
}
