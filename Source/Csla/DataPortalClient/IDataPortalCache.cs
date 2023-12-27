//-----------------------------------------------------------------------
// <copyright file="IDataPortalCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines interface for a client-side cache service</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Csla.DataPortalClient
{
  /// <summary>
  /// Defines interface for a client-side cache service
  /// used by the create and fetch operations in the
  /// client-side data portal.
  /// </summary>
  public interface IDataPortalCache
  {
    /// <summary>
    /// Try to get result from cache.
    /// </summary>
    /// <param name="objectType">Type of domain object to retrieve</param>
    /// <param name="criteria">Criteria for domain type being retrieved</param>
    /// <param name="operation">Data portal operation</param>
    /// <returns>null if not in cache</returns>
    Task<Server.DataPortalResult?> GetDataPortalResultAsync(Type objectType, object criteria, DataPortalOperations operation);
    /// <summary>
    /// Add object to cache.
    /// </summary>
    /// <param name="objectType">Type of domain object to add</param>
    /// <param name="criteria">Criteria for domain type being added</param>
    /// <param name="operation">Data portal operation</param>
    /// <param name="result">Data portal result to cache</param>
    /// <returns></returns>
    Task AddDataPortalResultAsync(Type objectType, object criteria, DataPortalOperations operation, Server.DataPortalResult result);
    /// <summary>
    /// Gets a value indicating whether the domain type 
    /// can be cached.
    /// </summary>
    /// <param name="objectType">Type of domain object to add</param>
    /// <param name="criteria">Criteria for domain type being added</param>
    /// <param name="operation">Data portal operation</param>
    /// <returns></returns>
    bool IsCacheable(Type objectType, object criteria, DataPortalOperations operation);
    /// <summary>
    /// Gets a semaphore used by the data portal to only allow a single
    /// consumer/thread to get/add an item to the cache at a time.
    /// </summary>
    /// <remarks>
    /// This semaphore must be a `new SemaphoreSlim(1)`
    /// </remarks>
    SemaphoreSlim Semaphore { get; }
  }
}
#nullable disable