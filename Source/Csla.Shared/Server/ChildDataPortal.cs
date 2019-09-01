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
    public object Create(Type objectType)
    {
      try
      { 
        return Create(objectType, false).Result;
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public object Create(Type objectType, params object[] parameters)
    {
      try
      { 
        return Create(objectType, true, parameters).Result;
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    public async Task<T> CreateAsync<T>()
    {
      return (T) await Create(typeof(T), false).ConfigureAwait(false);
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

    private async Task<object> Create(Type objectType, bool hasParameters, params object[] parameters)
    {
      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(null, objectType, parameters, DataPortalOperations.Create);
      try
      {
        obj = new DataPortalTarget(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);
        obj.Child_OnDataPortalInvoke(eventArgs);
        obj.MarkAsChild();
        obj.MarkNew();
        await obj.CreateChildAsync(parameters).ConfigureAwait(false);
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
      try
      {
        return Fetch(objectType, false, null).Result;
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
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
      try
      {
        return Fetch(objectType, true, parameters).Result;
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    public async Task<T> FetchAsync<T>()
    {
      return (T)await Fetch(typeof(T), false).ConfigureAwait(false);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public async Task<T> FetchAsync<T>(params object[] parameters)
    {
      return (T)await Fetch(typeof(T), true, parameters).ConfigureAwait(false);
    }

    private async Task<object> Fetch(Type objectType, bool hasParameters, params object[] parameters)
    {
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
        await obj.FetchChildAsync(parameters).ConfigureAwait(false);
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
      try
      {
        Update(obj, false, false, null).Wait();
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
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
      try
      { 
        Update(obj, true, false, parameters).Wait();
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public async Task UpdateAsync(object obj)
    {
      await Update(obj, false, false, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    public async Task UpdateAsync(object obj, params object[] parameters)
    {
      await Update(obj, true, false, parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public void UpdateAll(object obj)
    {
      Update(obj, false, true, null).Wait();
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
      Update(obj, true, true, parameters).Wait();
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public async Task UpdateAllAsync(object obj)
    {
      await Update(obj, false, true, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    public async Task UpdateAllAsync(object obj, params object[] parameters)
    {
      await Update(obj, true, true, parameters).ConfigureAwait(false);
    }

    private async Task Update(object obj, bool hasParameters, bool bypassIsDirtyTest, params object[] parameters)
    {
      if (obj == null)
        return;

      if (obj is Core.BusinessBase busObj && busObj.IsDirty == false && bypassIsDirtyTest == false)
      {
        // if the object isn't dirty, then just exit
        return;
      }

      var operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      DataPortalTarget lb = new DataPortalTarget(obj);
      ApplicationContext.DataPortalActivator.InitializeInstance(lb.Instance);

      try
      {
        lb.Child_OnDataPortalInvoke(
          new DataPortalEventArgs(null, objectType, obj, operation));
        await lb.UpdateChildAsync(parameters).ConfigureAwait(false);
        lb.Child_OnDataPortalInvokeComplete(
            new DataPortalEventArgs(null, objectType, obj, operation));
      }
      catch (Exception ex)
      {
        try
        {
          if (lb != null)
            lb.Child_OnDataPortalException(
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