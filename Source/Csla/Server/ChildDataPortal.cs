//-----------------------------------------------------------------------
// <copyright file="ChildDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Invoke data portal methods on child</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

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
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public ChildDataPortal(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is <see langword="null"/>.</exception>
    public object Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));

      return ExecuteWithAggregateExceptionHandling(
        () => DoCreateAsync(objectType).Result
      );
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is <see langword="null"/>.</exception>
    public object Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, params object?[]? parameters)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));

      return ExecuteWithAggregateExceptionHandling(
        () => DoCreateAsync(objectType, parameters).Result
      );
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    public async Task<T> CreateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
    {
      return (T) await DoCreateAsync(typeof(T)).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public async Task<T> CreateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(params object?[]? parameters)
    {
      return (T)await DoCreateAsync(typeof(T), parameters).ConfigureAwait(false);
    }

    private async Task<object> DoCreateAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, params object?[]? parameters)
    {
      DataPortalTarget? obj = null;
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
        object? outval = null;
        if (obj != null) outval = obj.Instance;
        throw new Csla.DataPortalException("ChildDataPortal.Create " + Properties.Resources.FailedOnServer, ex, outval);
      }
      finally
      {
        object? reference = null;
        if (obj != null)
          reference = obj.Instance;
        //ApplicationContext.DataPortalActivator.FinalizeInstance(reference);
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is <see langword="null"/>.</exception>
    public object Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));

      return ExecuteWithAggregateExceptionHandling(
  () => DoFetchAsync(objectType).Result
      );
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is <see langword="null"/>.</exception>
    public object Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, params object?[]? parameters)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));

      return ExecuteWithAggregateExceptionHandling(
  () => DoFetchAsync(objectType, parameters).Result
      );
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    public async Task<T> FetchAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
    {
      return (T)await DoFetchAsync(typeof(T)).ConfigureAwait(false);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="parameters">
    /// Criteria parameters passed from caller.
    /// </param>
    public async Task<T> FetchAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(params object?[]? parameters)
    {
      return (T)await DoFetchAsync(typeof(T), parameters).ConfigureAwait(false);
    }

    private async Task<object> DoFetchAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, params object?[]? parameters)
    {
      DataPortalTarget? obj = null;
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
        object? outval = null;
        if (obj != null) outval = obj.Instance;
        throw new Csla.DataPortalException("ChildDataPortal.Fetch " + Properties.Resources.FailedOnServer, ex, outval);
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
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public void Update(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      _ = ExecuteWithAggregateExceptionHandling(() =>
        {
          DoUpdateAsync(obj, false).Wait();
          return default!;
        }
      );
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public void Update(object obj, params object?[]? parameters)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      _ = ExecuteWithAggregateExceptionHandling(() =>
        {
          DoUpdateAsync(obj, false, parameters).Wait();
          return default!;
        }
      );
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public Task UpdateAsync(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      return DoUpdateAsync(obj, false);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public Task UpdateAsync(object obj, params object?[]? parameters)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      return DoUpdateAsync(obj, false, parameters);
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public void UpdateAll(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      DoUpdateAsync(obj, true).Wait();
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public void UpdateAll(object obj, params object?[]? parameters)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      DoUpdateAsync(obj, true, parameters).Wait();
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public Task UpdateAllAsync(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      return DoUpdateAsync(obj, true);
    }

    /// <summary>
    /// Update a business object. Include objects which are not dirty.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="parameters">
    /// Parameters passed to method.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public Task UpdateAllAsync(object obj, params object?[]? parameters)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      return DoUpdateAsync(obj, true, parameters);
    }

    private async Task DoUpdateAsync(object obj, bool bypassIsDirtyTest, params object?[]? parameters)
    {
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
        lb.Child_OnDataPortalInvoke(new DataPortalEventArgs(null, objectType, obj, operation));
        await lb.UpdateChildAsync(parameters).ConfigureAwait(false);
        lb.Child_OnDataPortalInvokeComplete(new DataPortalEventArgs(null, objectType, obj, operation));
      }
      catch (Exception ex)
      {
        try
        {
          lb.Child_OnDataPortalException(new DataPortalEventArgs(null, objectType, obj, operation), ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw new Csla.DataPortalException("ChildDataPortal.Update " + Properties.Resources.FailedOnServer, ex, obj);
      }
      //finally
      //{
      //  ApplicationContext.DataPortalActivator.FinalizeInstance(lb.Instance);
      //}
    }

    private object ExecuteWithAggregateExceptionHandling(Func<object> operation)
    {
      try
      {
        return operation();
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException is not null)
        {
          throw ex.InnerException;
        }

        if (ex.InnerExceptions.Count > 0)
        {
          throw ex.InnerExceptions[0];
        }

        throw new Exception(ex.Message) { Source = ex.Source };
      }
    }
  }
}