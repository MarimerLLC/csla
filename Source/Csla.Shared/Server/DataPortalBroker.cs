//-----------------------------------------------------------------------
// <copyright file="DataPortalBroker.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Allows interception of DataPortal call</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;

namespace Csla.Server
{
  /// <summary>
  /// Allows the Data Portal call to be intercepted by
  /// a custom IDataPortalServer implementation.
  /// </summary>
  public class DataPortalBroker : IDataPortalServer
  {
    /// <summary>
    /// Gets or sets a reference to a implementation of
    /// IDataPortalServer to be used.
    /// </summary>
    public static IDataPortalServer DataPortalServer { get; set; }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (DataPortalServer != null)
      {
        return DataPortalServer.Create(objectType, criteria, context, isSync);
      }
      else
      {
        var dp = new DataPortalSelector();
        return dp.Create(objectType, criteria, context, isSync);
      }      
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (DataPortalServer != null)
      {
        return DataPortalServer.Fetch(objectType, criteria, context, isSync);
      }
      else
      {
        var dp = new DataPortalSelector();
        return dp.Fetch(objectType, criteria, context, isSync);
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (DataPortalServer != null)
      {
        return DataPortalServer.Update(obj, context, isSync);
      }
      else
      {
        var dp = new DataPortalSelector();
        return dp.Update(obj, context, isSync);
      }
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (DataPortalServer != null)
      {
        return DataPortalServer.Delete(objectType, criteria, context, isSync);
      }
      else
      {
        var dp = new DataPortalSelector();
        return dp.Delete(objectType, criteria, context, isSync);
      }
    }
  }
}