//-----------------------------------------------------------------------
// <copyright file="ChildDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Invoke data portal methods on child</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Reflection;

namespace Csla.Server
{
  /// <summary>
  /// Invoke data portal methods on child
  /// objects.
  /// </summary>
  public class ChildDataPortal
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    public object Create(System.Type objectType)
    {
      return Create(objectType, false);
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public object Create(System.Type objectType, params object[] parameters)
    {
      return Create(objectType, true, parameters);
    }

    private object Create(System.Type objectType, bool hasParameters, params object[] parameters)
    {
      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(null, objectType, parameters, DataPortalOperations.Create);
      try
      {
        // create an instance of the business object
        obj = new LateBoundObject(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);

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


        // tell the business object to create its data
        if (hasParameters)
          obj.CallMethod("Child_Create", parameters);
        else
          obj.CallMethod("Child_Create");

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
        object bo = null;
        if (obj != null)
          bo = obj.Instance;
        throw new Csla.DataPortalException(
          "ChildDataPortal.Create " + Properties.Resources.FailedOnServer, ex, bo);
      }
      finally
      {
        ApplicationContext.DataPortalActivator.FinalizeInstance(obj.Instance);
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    public object Fetch(Type objectType)
    {
      return Fetch(objectType, false, null);
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
      return Fetch(objectType, true, parameters);
    }

    private object Fetch(Type objectType, bool hasParameters, params object[] parameters)
    {

      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(null, objectType, parameters, DataPortalOperations.Fetch);
      try
      {
        // create an instance of the business object
        obj = new LateBoundObject(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);

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
        if (hasParameters)
          obj.CallMethod("Child_Fetch", parameters);
        else
          obj.CallMethod("Child_Fetch");

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
        object bo = null;
        if (obj != null)
          bo = obj.Instance;
        throw new Csla.DataPortalException(
          "ChildDataPortal.Fetch " + Properties.Resources.FailedOnServer, ex, bo);
      }
      finally
      {
        ApplicationContext.DataPortalActivator.FinalizeInstance(obj.Instance);
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public void Update(object obj)
    {
      Update(obj, false, false, null);
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
      Update(obj, true, false, parameters);
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public void UpdateAll(object obj)
    {
      Update(obj, false, true, null);
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    public void UpdateAll(object obj, params object[] parameters)
    {
      Update(obj, true, true, parameters);
    }

    private void Update(object obj, bool hasParameters, bool bypassIsDirtyTest, params object[] parameters)
    {
      if (obj == null)
        return;

      var busObj = obj as Core.BusinessBase;
      if (busObj != null && busObj.IsDirty == false && bypassIsDirtyTest == false)
      {
        // if the object isn't dirty, then just exit
        return;
      }

      var operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      IDataPortalTarget target = obj as IDataPortalTarget;
      LateBoundObject lb = new LateBoundObject(obj);
      ApplicationContext.DataPortalActivator.InitializeInstance(lb.Instance);

      try
      {
        if (target != null)
          target.Child_OnDataPortalInvoke(
            new DataPortalEventArgs(null, objectType, obj, operation));
        else
          lb.CallMethodIfImplemented("Child_OnDataPortalInvoke",
            new DataPortalEventArgs(null, objectType, obj, operation));

        // tell the business object to update itself
        if (busObj != null)
        {
          if (busObj.IsDeleted)
          {
            if (!busObj.IsNew)
            {
              // tell the object to delete itself
              if (hasParameters)
                lb.CallMethod("Child_DeleteSelf", parameters);
              else
                lb.CallMethod("Child_DeleteSelf");
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
              if (hasParameters)
                lb.CallMethod("Child_Insert", parameters);
              else
              {
                lb.CallMethod("Child_Insert");
              }

            }
            else
            {
              // tell the object to update itself
              if (hasParameters)
                lb.CallMethod("Child_Update", parameters);
              else
              {
                lb.CallMethod("Child_Update");
              }
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
          if (hasParameters)
            lb.CallMethod("Child_Execute", parameters);
          else
            lb.CallMethod("Child_Execute");
          operation = DataPortalOperations.Execute;

        }
        else
        {
          // this is an updatable collection or some other
          // non-BusinessBase type of object
          // tell the object to update itself
          if (hasParameters)
            lb.CallMethod("Child_Update", parameters);
          else
            lb.CallMethod("Child_Update");
          if (target != null)
            target.MarkOld();
          else
            lb.CallMethodIfImplemented("MarkOld");
        }

        if (target != null)
          target.Child_OnDataPortalInvokeComplete(
            new DataPortalEventArgs(null, objectType, obj, operation));
        else
          lb.CallMethodIfImplemented("Child_OnDataPortalInvokeComplete",
            new DataPortalEventArgs(null, objectType, obj, operation));

      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.Child_OnDataPortalException(
              new DataPortalEventArgs(null, objectType, obj, operation), ex);
          else if (lb != null)
            lb.CallMethodIfImplemented("Child_OnDataPortalException",
              new DataPortalEventArgs(null, objectType, obj, operation), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException(
          "ChildDataPortal.Update " + Properties.Resources.FailedOnServer, ex, obj);
      }
      finally
      {
        ApplicationContext.DataPortalActivator.FinalizeInstance(lb.Instance);
      }
    }
  }
}