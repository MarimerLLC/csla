//-----------------------------------------------------------------------
// <copyright file="IDataPortalCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Null implementation of a client-side cache service</summary>
//-----------------------------------------------------------------------

using Csla.Server;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Null implementation of a client-side cache service.
  /// </summary>
  public class DataPortalNoCache : IDataPortalCache
  {
    /// <summary>
    /// Always invokes the data portal delegate with
    /// no caching.
    /// </summary>
    /// <param name="objectType">Type of domain object to retrieve</param>
    /// <param name="criteria">Criteria for domain type being retrieved</param>
    /// <param name="operation">Data portal operation</param>
    /// <param name="portal">Data portal delegate</param>
    public async Task<DataPortalResult> GetDataPortalResultAsync(Type objectType, object criteria, DataPortalOperations operation, Func<Task<DataPortalResult>> portal) 
      => await portal();
  }
}
