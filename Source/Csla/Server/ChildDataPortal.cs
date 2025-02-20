//-----------------------------------------------------------------------
// <copyright file="ChildDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Invoke data portal methods on child</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Invoke data portal methods on child
  /// objects.
  /// </summary>
  public class ChildDataPortal
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    public ChildDataPortal(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    public object Create(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType)
    {
      try
      { 
        return DoCreateAsync(objectType).Result;
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
    public object Create(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType, params object[] parameters)
    {
      try
      { 
        return DoCreateAsync(objectType, parameters).Result;
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    public async Task<T> CreateAsync<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      T>()
    {
      return (T) await DoCreateAsync(typeof(T)).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public async Task<T> CreateAsync<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      T>(params object[] parameters)
    {
      return (T)await DoCreateAsync(typeof(T), parameters).ConfigureAwait(false);
    }

    private async Task<object> DoCreateAsync(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType, params object[] parameters)
    {
      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(null, objectType, parameters, DataPortalOperations.Create);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        //ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);
        obj.Child_OnDataPortalInvoke(eventArgs);
        obj.MarkAsChild();
        obj.MarkNew();
        await obj.CreateChildAsync(parameters).ConfigureAwait(false);
        obj.Child_OnDataPortalInvokeComplete(eventArgs);
        return obj.Instance;

      }
      catch (Exception ex)
      {
        try
        {
          obj?.Child_OnDataPortalException(eventArgs, ex);
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
        //ApplicationContext.DataPortalActivator.FinalizeInstance(reference);
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    public object Fetch(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType)
    {
      try
      {
        return DoFetchAsync(objectType, null).Result;
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
    public object Fetch(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType, params object[] parameters)
    {
      try
      {
        return DoFetchAsync(objectType, parameters).Result;
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    public async Task<T> FetchAsync<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      T>()
    {
      return (T)await DoFetchAsync(typeof(T)).ConfigureAwait(false);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public async Task<T> FetchAsync<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      T>(params object[] parameters)
    {
      return (T)await DoFetchAsync(typeof(T), parameters).ConfigureAwait(false);
    }

    private async Task<object> DoFetchAsync(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType, params object[] parameters)
    {
      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(null, objectType, parameters, DataPortalOperations.Fetch);
      try
      {
        // create an instance of the business object
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        //ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);

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
          obj?.Child_OnDataPortalException(eventArgs, ex);
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
      //finally
      //{
      //  ApplicationContext.DataPortalActivator.FinalizeInstance(obj.Instance);
      //}
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public void Update(object obj)
    {
      try
      {
        DoUpdateAsync(obj, false, null).Wait();
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
        DoUpdateAsync(obj, false, parameters).Wait();
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
    public Task UpdateAsync(object obj)
    {
      return DoUpdateAsync(obj, false, null);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    public Task UpdateAsync(object obj, params object[] parameters)
    {
      return DoUpdateAsync(obj, false, parameters);
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public void UpdateAll(object obj)
    {
      DoUpdateAsync(obj, true, null).Wait();
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
      DoUpdateAsync(obj, true, parameters).Wait();
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public Task UpdateAllAsync(object obj)
    {
      return DoUpdateAsync(obj, true, null);
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    public Task UpdateAllAsync(object obj, params object[] parameters)
    {
      return DoUpdateAsync(obj, true, parameters);
    }

    private async Task DoUpdateAsync(object obj, bool bypassIsDirtyTest, params object[] parameters)
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
      DataPortalTarget lb = _applicationContext.CreateInstanceDI<DataPortalTarget>(obj);
      //ApplicationContext.DataPortalActivator.InitializeInstance(lb.Instance);

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
          lb?.Child_OnDataPortalException(
            new DataPortalEventArgs(null, objectType, obj, operation), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException(
          "ChildDataPortal.Update " + Properties.Resources.FailedOnServer, ex, obj);
      }
      //finally
      //{
      //  ApplicationContext.DataPortalActivator.FinalizeInstance(lb.Instance);
      //}
    }
  }
}