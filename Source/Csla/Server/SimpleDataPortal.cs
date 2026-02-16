//-----------------------------------------------------------------------
// <copyright file="SimpleDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side DataPortal as discussed</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side DataPortal as discussed
  /// in Chapter 4.
  /// </summary>
  public class SimpleDataPortal : IDataPortalServer
  {
    private readonly ApplicationContext _applicationContext;
    private readonly IDataPortalActivator _activator;
    private readonly IDataPortalExceptionInspector _exceptionInspector;
    private readonly Configuration.DataPortalOptions _dataPortalOptions;

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext">ApplicationContext</param>
    /// <param name="activator"></param>
    /// <param name="exceptionInspector"></param>
    /// <param name="dataPortalOptions"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="activator"/>, <paramref name="exceptionInspector"/> or <paramref name="dataPortalOptions"/> is <see langword="null"/>.</exception>
    public SimpleDataPortal(ApplicationContext applicationContext, IDataPortalActivator activator, IDataPortalExceptionInspector exceptionInspector, Configuration.DataPortalOptions dataPortalOptions)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      _activator = activator ?? throw new ArgumentNullException(nameof(activator));
      _exceptionInspector = exceptionInspector ?? throw new ArgumentNullException(nameof(exceptionInspector));
      _dataPortalOptions = dataPortalOptions ?? throw new ArgumentNullException(nameof(dataPortalOptions));
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public async Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      DataPortalTarget? obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Create);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        obj.OnDataPortalInvoke(eventArgs);
        obj.MarkNew();
        await obj.CreateAsync(criteria, isSync, context.OperationName).ConfigureAwait(false);
        await obj.WaitForIdle().ConfigureAwait(false);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(_applicationContext, obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          obj?.OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object? outval = null;
        if (obj != null) 
          outval = obj.Instance;
        
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Create " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(_exceptionInspector).InspectException(objectType, outval, criteria, "DataPortal.Create", ex),
              outval, _dataPortalOptions);
      }
      finally
      {
        if (obj?.Instance is not null)
          _activator.FinalizeInstance(obj.Instance);
      }
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public async Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
      {
        return await Execute(objectType, criteria, context, isSync);
      }
      
      DataPortalTarget? obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Fetch);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        _activator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        obj.MarkOld();
        await obj.FetchAsync(criteria, isSync, context.OperationName).ConfigureAwait(false);
        await obj.WaitForIdle().ConfigureAwait(false);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(_applicationContext, obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          obj?.OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object? outval = null;
        if (obj != null) 
          outval = obj.Instance;
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Fetch " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(_exceptionInspector).InspectException(objectType, outval, criteria, "DataPortal.Fetch", ex),
              outval, _dataPortalOptions);
      }
      finally
      {
        if (obj?.Instance is not null)
          _activator.FinalizeInstance(obj.Instance);
      }
    }

    private async Task<DataPortalResult> Execute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalTarget? obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Execute);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        _activator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        obj.MarkOld();
        await obj.ExecuteAsync(criteria, isSync, context.OperationName).ConfigureAwait(false);
        await obj.WaitForIdle().ConfigureAwait(false);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(_applicationContext, obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          obj?.OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object? outval = null;
        if (obj != null) outval = obj.Instance;
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Execute " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(_exceptionInspector).InspectException(objectType, outval, criteria, "DataPortal.Execute", ex),
              outval, _dataPortalOptions);
      }
      finally
      {
        if (obj?.Instance is not null)
          _activator.FinalizeInstance(obj.Instance);
      }
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    public async Task<DataPortalResult> Update(ICslaObject obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      DataPortalOperations operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      var lb = _applicationContext.CreateInstanceDI<DataPortalTarget>(obj);
      if (lb.Instance is Core.ICommandObject)
        return await Execute(lb, context, isSync);

      var eventArgs = new DataPortalEventArgs(context, objectType, obj, operation);
      try
      {
        _activator.InitializeInstance(lb.Instance);
        lb.OnDataPortalInvoke(eventArgs);
        await lb.UpdateAsync(isSync).ConfigureAwait(false);
        await lb.WaitForIdle().ConfigureAwait(false);
        lb.ThrowIfBusy();
        lb.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(_applicationContext, lb.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          lb.OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Update " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(_exceptionInspector).InspectException(obj.GetType(), obj, null, "DataPortal.Update", ex),
              obj, _dataPortalOptions);
      }
      finally
      {
        _activator.FinalizeInstance(lb.Instance);
      }
    }

    private async Task<DataPortalResult> Execute(DataPortalTarget obj, DataPortalContext context, bool isSync)
    {
      DataPortalOperations operation = DataPortalOperations.Execute;
      Type objectType = obj.Instance.GetType();
      var eventArgs = new DataPortalEventArgs(context, objectType, obj, operation);
      try
      {
        _activator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        await obj.ExecuteAsync(isSync).ConfigureAwait(false);
        await obj.WaitForIdle().ConfigureAwait(false);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(_applicationContext, obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          obj.OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        var reference = obj.Instance;
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Execute " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(_exceptionInspector).InspectException(reference.GetType(), reference, null, "DataPortal.Execute", ex),
              reference, _dataPortalOptions);
      }
      finally
      {
        _activator.FinalizeInstance(obj.Instance);
      }
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    public async Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalTarget? obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Delete);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        _activator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        await obj.DeleteAsync(criteria, isSync, context.OperationName).ConfigureAwait(false);
        await obj.WaitForIdle().ConfigureAwait(false);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(_applicationContext);
      }
      catch (Exception ex)
      {
        try
        {
          obj?.OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Delete " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(_exceptionInspector).InspectException(objectType, obj, null, "DataPortal.Delete", ex),
              null, _dataPortalOptions);
      }
      finally
      {
        if (obj?.Instance is not null)
          _activator.FinalizeInstance(obj.Instance);
      }
    }
  }
}