using System;
using System.Transactions;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side Serviced 
  /// DataPortal described in Chapter 4.
  /// </summary>
  public class TransactionalDataPortal : IDataPortalServer
  {
    /// <summary>
    /// Wraps a Create call in a TransactionScope
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a
    /// <see cref="TransactionScope">TransactionScope</see>
    /// to provide transactional support via
    /// System.Transactions.
    /// </remarks>
    /// <param name="objectType">A <see cref="Type">Type</see> object
    /// indicating the type of business object to be created.</param>
    /// <param name="criteria">A custom criteria object providing any
    /// extra information that may be required to properly create
    /// the object.</param>
    /// <param name="context">Context data from the client.</param>
    /// <returns>A populated business object.</returns>
    public DataPortalResult Create(
      System.Type objectType, object criteria, DataPortalContext context)
    {
      DataPortalResult result;
      using (TransactionScope tr = new TransactionScope())
      {
        var portal = new DataPortalSelector();
        result = portal.Create(objectType, criteria, context);
        tr.Complete();
      }
      return result;
    }

    /// <summary>
    /// Called by the client-side DataProtal to retrieve an object.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a
    /// <see cref="TransactionScope">TransactionScope</see>
    /// to provide transactional support via
    /// System.Transactions.
    /// </remarks>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Object containing context data from client.</param>
    /// <returns>A populated business object.</returns>
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      DataPortalResult result;
      using (TransactionScope tr = new TransactionScope())
      {
        var portal = new DataPortalSelector();
        result = portal.Fetch(objectType, criteria, context);
        tr.Complete();
      }
      return result;
    }

    /// <summary>
    /// Called by the client-side DataPortal to update an object.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a
    /// <see cref="TransactionScope">TransactionScope</see>
    /// to provide transactional support via
    /// System.Transactions.
    /// </remarks>
    /// <param name="obj">A reference to the object being updated.</param>
    /// <param name="context">Context data from the client.</param>
    /// <returns>A reference to the newly updated object.</returns>
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      DataPortalResult result;
      using (TransactionScope tr = new TransactionScope())
      {
        var portal = new DataPortalSelector();
        result = portal.Update(obj, context);
        tr.Complete();
      }
      return result;
    }

    /// <summary>
    /// Called by the client-side DataPortal to delete an object.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a
    /// <see cref="TransactionScope">TransactionScope</see>
    /// to provide transactional support via
    /// System.Transactions.
    /// </remarks>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Context data from the client.</param>
    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      DataPortalResult result;
      using (TransactionScope tr = new TransactionScope())
      {
        var portal = new DataPortalSelector();
        result = portal.Delete(objectType, criteria, context);
        tr.Complete();
      }
      return result;
    }
  }
}