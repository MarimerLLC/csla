//-----------------------------------------------------------------------
// <copyright file="IDataPortalCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Null implementation of a client-side cache service</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;
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
    /// Gets a semaphore used by the data portal to only allow a single
    /// consumer/thread to get/add an item to the cache at a time.
    /// </summary>
    /// <remarks>
    /// This semaphore must be a `new SemaphoreSlim(1)`
    /// </remarks>
    public SemaphoreSlim Semaphore => throw new NotImplementedException();

    /// <summary>
    /// Always returns success, does not cache values.
    /// </summary>
    /// <param name="objectType">Type of domain object to add</param>
    /// <param name="criteria">Criteria for domain type being added</param>
    /// <param name="operation">Data portal operation</param>
    /// <param name="result">Data portal result to cache</param>
    public Task AddDataPortalResultAsync(Type objectType, object criteria, DataPortalOperations operation, DataPortalResult result) 
      => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether the domain type 
    /// can be cached.
    /// </summary>
    /// <param name="objectType">Type of domain object to add</param>
    /// <param name="criteria">Criteria for domain type being added</param>
    /// <param name="operation">Data portal operation</param>
    /// <returns></returns>
    public bool IsCacheable(Type objectType, object criteria, DataPortalOperations operation) 
      => false;

    /// <summary>
    /// Always returns false, does not retrieve values from cache.
    /// </summary>
    /// <param name="objectType">Type of domain object to retrieve</param>
    /// <param name="criteria">Criteria for domain type being retrieved</param>
    /// <param name="operation">Data portal operation</param>
    /// <returns>true if success, false if object isn't returned</returns>
    public Task<DataPortalResult> GetDataPortalResultAsync(Type objectType, object criteria, DataPortalOperations operation) 
      => throw new NotImplementedException();
  }
}
