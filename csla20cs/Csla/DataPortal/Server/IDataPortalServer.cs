using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Server
{
  /// <summary>
  /// Interface implemented by server-side data portal
  /// components.
  /// </summary>
  public interface IDataPortalServer
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    DataPortalResult Create(Type objectType, object criteria, DataPortalContext context);
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    DataPortalResult Fetch(object criteria, DataPortalContext context);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    DataPortalResult Update(object obj, DataPortalContext context);
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    DataPortalResult Delete(object criteria, DataPortalContext context);
  }
}
