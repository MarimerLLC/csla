//-----------------------------------------------------------------------
// <copyright file="ChildDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Invoke data portal methods on child</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
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

    /// <summary>
    /// Create a new business object.
    /// </summary>
    public async Task<T> CreateAsync<T>()
    {
      return (T) await Create(typeof(T), false, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public async Task<T> CreateAsync<T>(params object[] parameters)
    {
      return (T)await Create(typeof(T), true, parameters).ConfigureAwait(false);
    }

    private async Task<object> Create(System.Type objectType, bool hasParameters, params object[] parameters)
    {
      object criteria = EmptyCriteria.Instance;
      if (hasParameters && parameters == null)
        criteria = null;
      else if (hasParameters)
        criteria = parameters;

      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(null, objectType, criteria, DataPortalOperations.Create);
      try
      {
        obj = new DataPortalTarget(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);
        obj.Child_OnDataPortalInvoke(eventArgs);
        obj.MarkAsChild();
        obj.MarkNew();
        await obj.CreateChildAsync(criteria).ConfigureAwait(false);
        obj.OnDataPortalInvokeComplete(eventArgs);
        return obj.Instance;

      }
      catch (Exception ex)
      {
        try
        {
          if (obj != null)
            obj.Child_OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw new Csla.DataPortalException(
          "ChildDataPortal.Create " + Properties.Resources.FailedOnServer, ex, outval);
      }
      finally
      {
        object reference = null;
        if (obj != null)
          reference = obj.Instance;
        ApplicationContext.DataPortalActivator.FinalizeInstance(reference);
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

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    public async Task<T> FetchAsync<T>()
    {
      return (T)await Fetch(typeof(T), false, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public async Task<T> FetchAsync<T>(params object[] parameters)
    {
      return (T)await Fetch(typeof(T), true, parameters).ConfigureAwait(false);
    }

    private async Task<object> Fetch(Type objectType, bool hasParameters, params object[] parameters)
    {
      object criteria = EmptyCriteria.Instance;
      if (hasParameters && parameters == null)
        criteria = null;
      else if (hasParameters)
        criteria = parameters;

      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(null, objectType, parameters, DataPortalOperations.Fetch);
      try
      {
        // create an instance of the business object
        obj = new DataPortalTarget(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);

        obj.Child_OnDataPortalInvoke(eventArgs);
        obj.MarkAsChild();
        obj.MarkOld();
        await obj.FetchChildAsync(criteria).ConfigureAwait(false);
        obj.Child_OnDataPortalInvokeComplete(eventArgs);
        return obj.Instance;
      }
      catch (Exception ex)
      {
        try
        {
          if (obj != null)
            obj.Child_OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw new Csla.DataPortalException(
          "ChildDataPortal.Fetch " + Properties.Resources.FailedOnServer, ex, outval);
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