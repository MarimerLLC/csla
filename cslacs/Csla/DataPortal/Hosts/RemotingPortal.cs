using System;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through .NET Remoting.
  /// </summary>
  public class RemotingPortal : MarshalByRefObject, Server.IDataPortalServer
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Create(
      Type objectType, object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new DataPortal();
      return portal.Create(objectType, criteria, context);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new DataPortal();
      return portal.Fetch(objectType, criteria, context);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      Server.DataPortal portal = new DataPortal();
      return portal.Update(obj, context);
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new DataPortal();
      return portal.Delete(objectType, criteria, context);
    }
  }
}