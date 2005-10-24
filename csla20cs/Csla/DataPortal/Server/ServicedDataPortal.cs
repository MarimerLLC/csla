using System;
using System.EnterpriseServices;
using System.Runtime.InteropServices;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side Serviced 
  /// DataPortal described in Chapter 5.
  /// </summary>
  [Transaction(TransactionOption.Required)]
  [EventTrackingEnabled(true)]
  [ComVisible(true)]
  public class ServicedDataPortal : ServicedComponent, IDataPortalServer
  {

    /// <summary>
    /// Called by the client-side DataPortal to create a new object.
    /// </summary>
    /// <remarks>
    /// This method runs in a distributed transactional context
    /// within Enterprise Services. To indicate failure and trigger
    /// a rollback your code must throw an exception.
    /// </remarks>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Context data from the client.</param>
    /// <returns>A populated business object.</returns>
    [AutoComplete(true)]
    public DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
    {
      SimpleDataPortal portal = new SimpleDataPortal();
      return portal.Create(objectType, criteria, context);
    }

    /// <summary>
    /// Called by the client-side DataProtal to retrieve an object.
    /// </summary>
    /// <remarks>
    /// This method runs in a distributed transactional context
    /// within Enterprise Services. To indicate failure and trigger
    /// a rollback your code must throw an exception.
    /// </remarks>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Object containing context data from client.</param>
    /// <returns>A populated business object.</returns>
    [AutoComplete(true)]
    public DataPortalResult Fetch(object criteria, DataPortalContext context)
    {
      SimpleDataPortal portal = new SimpleDataPortal();
      return portal.Fetch(criteria, context);
    }

    /// <summary>
    /// Called by the client-side DataPortal to update an object.
    /// </summary>
    /// <remarks>
    /// This method runs in a distributed transactional context
    /// within Enterprise Services. To indicate failure and trigger
    /// a rollback your code must throw an exception.
    /// </remarks>
    /// <param name="obj">A reference to the object being updated.</param>
    /// <param name="context">Context data from the client.</param>
    /// <returns>A reference to the newly updated object.</returns>
    [AutoComplete(true)]
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      SimpleDataPortal portal = new SimpleDataPortal();
      return portal.Update(obj, context);
    }

    /// <summary>
    /// Called by the client-side DataPortal to delete an object.
    /// </summary>
    /// <remarks>
    /// This method runs in a distributed transactional context
    /// within Enterprise Services. To indicate failure and trigger
    /// a rollback your code must throw an exception.
    /// </remarks>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Context data from the client.</param>
    [AutoComplete(true)]
    public DataPortalResult Delete(object criteria, DataPortalContext context)
    {
      SimpleDataPortal portal = new SimpleDataPortal();
      return portal.Delete(criteria, context);
    }
  }
}