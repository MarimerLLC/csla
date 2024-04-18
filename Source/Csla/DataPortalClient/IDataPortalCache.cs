//-----------------------------------------------------------------------
// <copyright file="IDataPortalCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines interface for a client-side cache service</summary>
//-----------------------------------------------------------------------

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
    /// Get result from cache or data portal.
    /// </summary>
    /// <param name="objectType">Type of domain object to retrieve</param>
    /// <param name="criteria">Criteria for domain type being retrieved</param>
    /// <param name="operation">Data portal operation</param>
    /// <param name="portal">Data portal delegate</param>
    /// <remarks>
    /// The data portal invokes this method for each operation. The cache
    /// implementation may choose to return a result from the cache, 
    /// or return a result by invoking the data portal delegate.
    /// </remarks>
    Task<Server.DataPortalResult> GetDataPortalResultAsync(
      Type objectType, 
      object criteria, 
      DataPortalOperations operation, 
      Func<Task<Server.DataPortalResult>> portal);
  }
}
#nullable disable