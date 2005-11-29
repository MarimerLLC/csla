using System;
using System.Reflection;
using System.Security.Principal;
using System.Collections.Specialized;
using Csla.Properties;

namespace Csla.Server
{
  public class SimpleDataPortal : IDataPortalServer
  {

    #region Data Access

    /// <summary>
    /// Called by the client-side DataPortal to create a new object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Context data from the client.</param>
    /// <returns>A populated business object.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public DataPortalResult Create(
      Type objectType, object criteria, DataPortalContext context)
    {
      object obj = null;

      try
      {
        // create an instance of the business object.
        obj = Activator.CreateInstance(objectType, true);

        // tell the business object we're about to make a DataPortal_xyz call
        MethodCaller.CallMethodIfImplemented(
          obj, "DataPortal_OnDataPortalInvoke", new DataPortalEventArgs(context));

        // tell the business object to fetch its data
        MethodCaller.CallMethod(
          obj, "DataPortal_Create", criteria);

        // mark the object as new
        MethodCaller.CallMethodIfImplemented(
          obj, "MarkNew");

        // tell the business object the DataPortal_xyz call is complete
        MethodCaller.CallMethodIfImplemented(
          obj, "DataPortal_OnDataPortalInvokeComplete", 
          new DataPortalEventArgs(context));

        // return the populated business object as a result
        return new DataPortalResult(obj);
      }
      catch (Exception ex)
      {
        try
        {
          // tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented(
            obj, "DataPortal_OnDataPortalException", 
            new DataPortalEventArgs(context), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new DataPortalException(
          "DataPortal.Create " + Resources.FailedOnServer, 
          ex, new DataPortalResult(obj));
      }

    }

    /// <summary>
    /// Called by the client-side DataProtal to retrieve an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Object containing context data from client.</param>
    /// <returns>A populated business object.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public DataPortalResult Fetch(object criteria, DataPortalContext context)
    {
      object obj = null;
      try
      {
        // create an instance of the business object
        obj = CreateBusinessObject(criteria);

        // tell the business object we're about to make a DataPortal_xyz call
        MethodCaller.CallMethodIfImplemented(
          obj, "DataPortal_OnDataPortalInvoke", 
          new DataPortalEventArgs(context));

        // tell the business object to fetch its data
        MethodCaller.CallMethod(
          obj, "DataPortal_Fetch", criteria);

        // mark the object as old
        MethodCaller.CallMethodIfImplemented(
          obj, "MarkOld");

        // tell the business object the DataPortal_xyz call is complete
        MethodCaller.CallMethodIfImplemented(
          obj, "DataPortal_OnDataPortalInvokeComplete", 
          new DataPortalEventArgs(context));

        // return the populated business object as a result
        return new DataPortalResult(obj);
      }
      catch (Exception ex)
      {
        try
        {
          // tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented(
            obj, "DataPortal_OnDataPortalException", 
            new DataPortalEventArgs(context), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new DataPortalException(
          "DataPortal.Fetch " + Resources.FailedOnServer, 
          ex, new DataPortalResult(obj));
      }
    }

    /// <summary>
    /// Called by the client-side DataPortal to update an object.
    /// </summary>
    /// <param name="obj">A reference to the object being updated.</param>
    /// <param name="context">Context data from the client.</param>
    /// <returns>A reference to the newly updated object.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      try
      {
        // tell the business object we're about to make a DataPortal_xyz call
        MethodCaller.CallMethodIfImplemented(
          obj, "DataPortal_OnDataPortalInvoke", 
          new DataPortalEventArgs(context));

        // tell the business object to update itself
        if (obj is Core.BusinessBase)
        {
          Core.BusinessBase busObj = obj as Core.BusinessBase;
          if (busObj.IsDeleted)
          {
            if (!busObj.IsNew)
            {
              // tell the object to delete itself
              MethodCaller.CallMethod(
                busObj, "DataPortal_DeleteSelf");
            }
            // mark the object as new
            MethodCaller.CallMethodIfImplemented(
              busObj, "MarkNew");
          }
          else
          {
            if (busObj.IsNew)
            {
              // tell the object to insert itself
              MethodCaller.CallMethod(
                busObj, "DataPortal_Insert");
            }
            else
            {
              // tell the object to update itself
              MethodCaller.CallMethod(
                busObj, "DataPortal_Update");
            }
            // mark the object as old
            MethodCaller.CallMethodIfImplemented(
              busObj, "MarkOld");
          }
        }
        else if (obj is CommandBase)
        {
          // tell the object to update itself
          MethodCaller.CallMethod(
            obj, "DataPortal_Execute");
        }
        else
        {
          // this is an updatable collection or some other
          // non-BusinessBase type of object
          // tell the object to update itself
          MethodCaller.CallMethod(
            obj, "DataPortal_Update");
          // mark the object as old
          MethodCaller.CallMethodIfImplemented(
            obj, "MarkOld");
        }

        // tell the business object the DataPortal_xyz is complete
        MethodCaller.CallMethodIfImplemented(
          obj, "DataPortal_OnDataPortalInvokeComplete", 
          new DataPortalEventArgs(context));

        return new DataPortalResult(obj);
      }
      catch (Exception ex)
      {
        try
        {
          // tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented(
            obj, "DataPortal_OnDataPortalException",
            new DataPortalEventArgs(context), ex);
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
    /// Called by the client-side DataPortal to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Context data from the client.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    public DataPortalResult Delete(object criteria, DataPortalContext context)
    {
      object obj = null;
      try
      {
        // create an instance of the business objet
        obj = CreateBusinessObject(criteria);

        // tell the business object we're about to make a DataPortal_xyz call
        MethodCaller.CallMethodIfImplemented(
          obj, "DataPortal_OnDataPortalInvoke", 
          new DataPortalEventArgs(context));

        // tell the business object to delete itself
        MethodCaller.CallMethod(
          obj, "DataPortal_Delete", criteria);

        // tell the business object the DataPortal_xyz call is complete
        MethodCaller.CallMethodIfImplemented(
          obj, "DataPortal_OnDataPortalInvokeComplete", 
          new DataPortalEventArgs(context));

        return new DataPortalResult();
      }
      catch (Exception ex)
      {
        try
        {
          // tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented(
            obj, "DataPortal_OnDataPortalException",
            new DataPortalEventArgs(context), ex);
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

    #region Creating the business object

    private static object CreateBusinessObject(object criteria)
    {
      Type businessType;
      if (criteria.GetType().IsSubclassOf(typeof(CriteriaBase)))
      {
        // get the type of the actual business object
        // from CriteriaBase (using the new scheme)
        businessType = ((CriteriaBase)criteria).ObjectType;
      }
      else
      {
        // get the type of the actual business object
        // based on the nested class scheme in the book
        businessType = criteria.GetType().DeclaringType;
      }

      // create an instance of the business object
      return Activator.CreateInstance(businessType, true);
    }

    #endregion

  }
}