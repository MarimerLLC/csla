//-----------------------------------------------------------------------
// <copyright file="ChildDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Invoke data portal methods on child</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Reflection;
using Csla.Server;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Invoke data portal methods on child
  /// objects.
  /// </summary>
  public class ChildDataPortal<T>
  {
    #region  Data Access

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public object Create(params object[] parameters)
    {
      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(typeof(T), DataPortalOperations.Create);
      try
      {
        // create an instance of the business object
        obj = new LateBoundObject(typeof(T));

        target = obj.Instance as IDataPortalTarget;

        if (target != null)
        {
          target.Child_OnDataPortalInvoke(eventArgs);
          target.MarkAsChild();
          target.MarkNew();
        }
        else
        {
          obj.CallMethodIfImplemented("Child_OnDataPortalInvoke",
            eventArgs);
          obj.CallMethodIfImplemented("MarkAsChild");
          obj.CallMethodIfImplemented("MarkNew");
        }


        // tell the business object to fetch its data
        obj.CallMethod("Child_Create", parameters);

        if (target != null)
          target.Child_OnDataPortalInvokeComplete(eventArgs);
        else
          obj.CallMethodIfImplemented("Child_OnDataPortalInvokeComplete",
            eventArgs);

        // return the populated business object as a result
        return obj.Instance;

      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.Child_OnDataPortalException(eventArgs, ex);
          else if (obj != null)
            obj.CallMethodIfImplemented("Child_OnDataPortalException",
              eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException(
          "ChildDataPortal.Create " + Properties.Resources.FailedOnServer, 
          ex);
      }

    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public object Fetch(params object[] parameters)
    {

      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(typeof(T), DataPortalOperations.Fetch);
      try
      {
        // create an instance of the business object
        obj = new LateBoundObject(typeof(T));

        target = obj.Instance as IDataPortalTarget;

        if (target != null)
        {
          target.Child_OnDataPortalInvoke(eventArgs);
          target.MarkAsChild();
          target.MarkOld();
        }
        else
        {
          obj.CallMethodIfImplemented("Child_OnDataPortalInvoke",
            eventArgs);
          obj.CallMethodIfImplemented("MarkAsChild");
          obj.CallMethodIfImplemented("MarkOld");
        }

        // tell the business object to fetch its data
        obj.CallMethod("Child_Fetch", parameters);

        if (target != null)
          target.Child_OnDataPortalInvokeComplete(eventArgs);
        else
          obj.CallMethodIfImplemented("Child_OnDataPortalInvokeComplete",
            eventArgs);

        // return the populated business object as a result
        return obj.Instance;

      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.Child_OnDataPortalException(eventArgs, ex);
          else if (obj != null)
            obj.CallMethodIfImplemented("Child_OnDataPortalException",
              eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException(
          "ChildDataPortal.Fetch " + Properties.Resources.FailedOnServer, ex);
      }

    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    public void Update(T obj, params object[] parameters)
    {
      if (obj == null)
        return;

      var busObj = obj as Core.BusinessBase;
      if (busObj != null && busObj.IsDirty == false)
      {
        // if the object isn't dirty, then just exit
        return;
      }

      var operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      IDataPortalTarget target = obj as IDataPortalTarget;
      LateBoundObject lb = new LateBoundObject(obj);
      try
      {
        if (target != null)
          target.Child_OnDataPortalInvoke(
            new DataPortalEventArgs(objectType, operation));
        else
          lb.CallMethodIfImplemented("Child_OnDataPortalInvoke",
            new DataPortalEventArgs(objectType, operation));

        // tell the business object to update itself
        if (busObj != null)
        {
          if (busObj.IsDeleted)
          {
            if (!busObj.IsNew)
            {
              // tell the object to delete itself
              lb.CallMethod("Child_DeleteSelf", parameters);
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
              lb.CallMethod("Child_Insert", parameters);

            }
            else
            {
              // tell the object to update itself
              lb.CallMethod("Child_Update", parameters);
            }
            if (target != null)
              target.MarkOld();
            else
              lb.CallMethodIfImplemented("MarkOld");
          }

        }
        else if (obj is Core.ICommandObject)
        {
          // tell the object to update itself
          lb.CallMethod("Child_Execute", parameters);
          operation = DataPortalOperations.Execute;

        }
        else
        {
          // this is an updatable collection or some other
          // non-BusinessBase type of object
          // tell the object to update itself
          lb.CallMethod("Child_Update", parameters);
          if (target != null)
            target.MarkOld();
          else
            lb.CallMethodIfImplemented("MarkOld");
        }

        if (target != null)
          target.Child_OnDataPortalInvokeComplete(
            new DataPortalEventArgs(objectType, operation));
        else
          lb.CallMethodIfImplemented("Child_OnDataPortalInvokeComplete",
            new DataPortalEventArgs(objectType, operation));

      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.Child_OnDataPortalException(
              new DataPortalEventArgs(objectType, operation), ex);
          else if (lb != null)
            lb.CallMethodIfImplemented("Child_OnDataPortalException",
              new DataPortalEventArgs(objectType, operation), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException(
          "ChildDataPortal.Update " + Properties.Resources.FailedOnServer, ex);
      }

    }

    #endregion
  }
}