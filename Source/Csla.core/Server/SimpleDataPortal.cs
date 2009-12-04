using System;
using Csla.Reflection;
using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side DataPortal as discussed
  /// in Chapter 4.
  /// </summary>
  public class SimpleDataPortal : IDataPortalServer
  {
    #region Data Access

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public DataPortalResult Create(
      Type objectType, object criteria, DataPortalContext context)
    {
      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, DataPortalOperations.Create);
      try
      {
        // create an instance of the business object.
        obj = new LateBoundObject(objectType);

        target = obj.Instance as IDataPortalTarget;

        if (target != null)
        {
          target.DataPortal_OnDataPortalInvoke(eventArgs);
          target.MarkNew();
        }
        else
        {
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs);
          obj.CallMethodIfImplemented("MarkNew");
        }

        // tell the business object to create its data
        if (criteria is int)
          obj.CallMethod("DataPortal_Create");
        else
          obj.CallMethod("DataPortal_Create", criteria);

        if (target != null)
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
        else
          obj.CallMethodIfImplemented(
            "DataPortal_OnDataPortalInvokeComplete", eventArgs);

        // return the populated business object as a result
        return new DataPortalResult(obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.DataPortal_OnDataPortalException(eventArgs, ex);
          else
            obj.CallMethodIfImplemented(
              "DataPortal_OnDataPortalException",
              eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw new DataPortalException(
          "DataPortal.Create " + Resources.FailedOnServer, 
          ex, new DataPortalResult(outval));
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, DataPortalOperations.Fetch);
      try
      {
        // create an instance of the business object.
        obj = new LateBoundObject(objectType);

        target = obj.Instance as IDataPortalTarget;

        if (target != null)
        {
          target.DataPortal_OnDataPortalInvoke(eventArgs);
          target.MarkOld();
        }
        else
        {
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs);
          obj.CallMethodIfImplemented("MarkOld");
        }

        // tell the business object to fetch its data
        if (criteria is int)
          obj.CallMethod("DataPortal_Fetch");
        else
          obj.CallMethod("DataPortal_Fetch", criteria);

        if (target != null)
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
        else
          obj.CallMethodIfImplemented(
            "DataPortal_OnDataPortalInvokeComplete",
            eventArgs);

        // return the populated business object as a result
        return new DataPortalResult(obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.DataPortal_OnDataPortalException(eventArgs, ex);
          else
            obj.CallMethodIfImplemented(
              "DataPortal_OnDataPortalException",
              eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw new DataPortalException(
          "DataPortal.Fetch " + Resources.FailedOnServer, 
          ex, new DataPortalResult(outval));
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      DataPortalOperations operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      var target = obj as IDataPortalTarget;
      LateBoundObject lb = new LateBoundObject(obj);
      try
      {
        if (target != null)
          target.DataPortal_OnDataPortalInvoke(
            new DataPortalEventArgs(context, objectType, operation));
        else
          lb.CallMethodIfImplemented(
            "DataPortal_OnDataPortalInvoke",
            new DataPortalEventArgs(context, objectType, operation));

        // tell the business object to update itself
        var busObj = obj as Core.BusinessBase;
        if (busObj != null)
        {
          if (busObj.IsDeleted)
          {
            if (!busObj.IsNew)
            {
              // tell the object to delete itself
              lb.CallMethod("DataPortal_DeleteSelf");
            }
            if (target != null)
              target.MarkNew();
            else
              lb.CallMethodIfImplemented("MarkNew");
          }
          else
          {
            if (busObj.IsNew)
            {
              // tell the object to insert itself
              lb.CallMethod("DataPortal_Insert");
            }
            else
            {
              // tell the object to update itself
              lb.CallMethod("DataPortal_Update");
            }
            if (target != null)
              target.MarkOld();
            else
              lb.CallMethodIfImplemented("MarkOld");
          }
        }
        else if (obj is CommandBase)
        {
          operation = DataPortalOperations.Execute;
          // tell the object to update itself
          lb.CallMethod("DataPortal_Execute");
        }
        else
        {
          // this is an updatable collection or some other
          // non-BusinessBase type of object
          // tell the object to update itself
          lb.CallMethod("DataPortal_Update");
          if (target != null)
            target.MarkOld();
          else
            lb.CallMethodIfImplemented("MarkOld");
        }

        if (target != null)
          target.DataPortal_OnDataPortalInvokeComplete(
            new DataPortalEventArgs(context, objectType, operation));
        else
          lb.CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete",
            new DataPortalEventArgs(context, objectType, operation));

        return new DataPortalResult(obj);
      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.DataPortal_OnDataPortalException(
              new DataPortalEventArgs(context, objectType, operation), ex);
          else
            lb.CallMethodIfImplemented(
              "DataPortal_OnDataPortalException",
              new DataPortalEventArgs(context, objectType, operation), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new DataPortalException(
          "DataPortal.Update " + Resources.FailedOnServer, 
          ex, new DataPortalResult(obj));
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, DataPortalOperations.Delete);
      try
      {
        // create an instance of the business objet
        obj = new LateBoundObject(objectType);

        target = obj.Instance as IDataPortalTarget;

        if (target != null)
          target.DataPortal_OnDataPortalInvoke(eventArgs);
        else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs);

        // tell the business object to delete itself
        obj.CallMethod("DataPortal_Delete", criteria);

        if (target != null)
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
        else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete", eventArgs);

        return new DataPortalResult();
      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.DataPortal_OnDataPortalException(eventArgs, ex);
          else
            obj.CallMethodIfImplemented(
              "DataPortal_OnDataPortalException",
              eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new DataPortalException(
          "DataPortal.Delete " + Resources.FailedOnServer, 
          ex, new DataPortalResult());
      }
    }

    #endregion
  }
}