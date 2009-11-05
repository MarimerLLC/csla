using System;
using System.EnterpriseServices;
using System.Runtime.InteropServices;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side Serviced 
  /// DataPortal described in Chapter 4.
  /// </summary>
  [Transaction(TransactionOption.Required)]
  [EventTrackingEnabled(true)]
  [ComVisible(true)]
  public class ServicedDataPortal : ServicedComponent, IDataPortalServer
  {
    /// <summary>
    /// Wraps a Create call in a ServicedComponent.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a COM+ transaction
    /// to provide transactional support.
    /// </remarks>
    /// <param name="objectType">A <see cref="Type">Type</see> object
    /// indicating the type of business object to be created.</param>
    /// <param name="criteria">A custom criteria object providing any
    /// extra information that may be required to properly create
    /// the object.</param>
    /// <param name="context">Context data from the client.</param>
    /// <returns>A populated business object.</returns>
    [AutoComplete(true)]
    public DataPortalResult Create(
      Type objectType, object criteria, DataPortalContext context)
    {
      var portal = new DataPortalSelector();
      return portal.Create(objectType, criteria, context);
    }

    /// <summary>
    /// Wraps a Fetch call in a ServicedComponent.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a COM+ transaction
    /// to provide transactional support.
    /// </remarks>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Object containing context data from client.</param>
    /// <returns>A populated business object.</returns>
    [AutoComplete(true)]
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      var portal = new DataPortalSelector();
      return portal.Fetch(objectType, criteria, context);
    }

    /// <summary>
    /// Wraps an Update call in a ServicedComponent.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a COM+ transaction
    /// to provide transactional support.
    /// </remarks>
    /// <param name="obj">A reference to the object being updated.</param>
    /// <param name="context">Context data from the client.</param>
    /// <returns>A reference to the newly updated object.</returns>
    [AutoComplete(true)]
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      var portal = new DataPortalSelector();
      return portal.Update(obj, context);
    }

    /// <summary>
    /// Wraps a Delete call in a ServicedComponent.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a COM+ transaction
    /// to provide transactional support.
    /// </remarks>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Context data from the client.</param>
    [AutoComplete(true)]
    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      var portal = new DataPortalSelector();
      return portal.Delete(objectType, criteria, context);
    }
  }
}