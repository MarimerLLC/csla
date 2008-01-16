using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Principal;
using System.Collections.Specialized;

namespace Csla.Server
{
  /// <summary>
  /// Invoke data portal methods on child
  /// objects.
  /// </summary>
  public class ChildDataPortal
  {

    #region  Data Access

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public object Create(System.Type objectType, params object[] parameters)
    {

      object obj = null;

      try
      {
        // create an instance of the business object
        obj = Activator.CreateInstance(objectType, true);

        // mark the object as a child
        MethodCaller.CallMethodIfImplemented(obj, "MarkAsChild");

        // tell the business object we're about to make a Child_xyz call
        MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalInvoke", 
          new DataPortalEventArgs(null, objectType, DataPortalOperations.Create));

        // tell the business object to fetch its data
        MethodInfo method = MethodCaller.GetMethod(objectType, "Child_Create", parameters);
        MethodCaller.CallMethod(obj, method, parameters);

        // mark the object as new
        MethodCaller.CallMethodIfImplemented(obj, "MarkNew");

        // tell the business object the Child_xyz call is complete
        MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalInvokeComplete",
          new DataPortalEventArgs(null, objectType, DataPortalOperations.Create));

        // return the populated business object as a result
        return obj;

      }
      catch (Exception ex)
      {
        try
        {
          // tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalException",
            new DataPortalEventArgs(null, objectType, DataPortalOperations.Create), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException("ChildDataPortal.Create " + Properties.Resources.FailedOnServer, ex, obj);
      }

    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public object Fetch(Type objectType, params object[] parameters)
    {

      object obj = null;

      try
      {
        // create an instance of the business object
        obj = Activator.CreateInstance(objectType, true);

        // mark the object as a child
        MethodCaller.CallMethodIfImplemented(obj, "MarkAsChild");

        // tell the business object we're about to make a Child_xyz call
        MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalInvoke",
          new DataPortalEventArgs(null, objectType, DataPortalOperations.Fetch));

        // mark the object as old
        MethodCaller.CallMethodIfImplemented(obj, "MarkOld");

        // tell the business object to fetch its data
        MethodInfo method = MethodCaller.GetMethod(objectType, "Child_Fetch", parameters);
        MethodCaller.CallMethod(obj, method, parameters);

        // tell the business object the Child_xyz call is complete
        MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalInvokeComplete",
          new DataPortalEventArgs(null, objectType, DataPortalOperations.Fetch));

        // return the populated business object as a result
        return obj;

      }
      catch (Exception ex)
      {
        try
        {
          // tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalException",
            new DataPortalEventArgs(null, objectType, DataPortalOperations.Fetch), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException("ChildDataPortal.Fetch " + Properties.Resources.FailedOnServer, ex, obj);
      }

    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    public void Update(object obj, params object[] parameters)
    {

      var operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      try
      {
        // tell the business object we're about to make a Child_xyz call
        MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalInvoke",
          new DataPortalEventArgs(null, objectType, operation));

        // tell the business object to update itself
        if (obj is Core.BusinessBase)
        {
          Core.BusinessBase busObj = (Core.BusinessBase)obj;
          if (busObj.IsDeleted)
          {
            if (!busObj.IsNew)
            {
              // tell the object to delete itself
              MethodCaller.CallMethod(busObj, "Child_DeleteSelf", parameters);
            }
            // mark the object as new
            MethodCaller.CallMethodIfImplemented(busObj, "MarkNew");

          }
          else
          {
            if (busObj.IsNew)
            {
              // tell the object to insert itself
              MethodCaller.CallMethod(busObj, "Child_Insert", parameters);

            }
            else
            {
              // tell the object to update itself
              MethodCaller.CallMethod(busObj, "Child_Update", parameters);
            }
            // mark the object as old
            MethodCaller.CallMethodIfImplemented(busObj, "MarkOld");
          }

        }
        else if (obj is CommandBase)
        {
          // tell the object to update itself
          MethodCaller.CallMethod(obj, "Child_Execute", parameters);
          operation = DataPortalOperations.Execute;

        }
        else
        {
          // this is an updatable collection or some other
          // non-BusinessBase type of object
          // tell the object to update itself
          MethodCaller.CallMethod(obj, "Child_Update", parameters);
          // mark the object as old
          MethodCaller.CallMethodIfImplemented(obj, "MarkOld");
        }

        // tell the business object the Child_xyz call is complete
        MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalInvokeComplete",
          new DataPortalEventArgs(null, objectType, operation));

      }
      catch (Exception ex)
      {
        try
        {
          // tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented(obj, "Child_OnDataPortalException",
            new DataPortalEventArgs(null, objectType, operation), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException("ChildDataPortal.Update " + Properties.Resources.FailedOnServer, ex, obj);
      }

    }

    #endregion

    #region  Creating the business object

    private static object CreateBusinessObject(object criteria)
    {

      Type businessType = null;

      if (criteria.GetType().IsSubclassOf(typeof(CriteriaBase)))
      {
        // get the type of the actual business object
        // from CriteriaBase 
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
