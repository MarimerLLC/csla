//-----------------------------------------------------------------------
// <copyright file="SimpleDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side DataPortal as discussed</summary>
//-----------------------------------------------------------------------

using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side DataPortal as discussed
  /// in Chapter 4.
  /// </summary>
  public class SimpleDataPortal : IDataPortalServer
  {
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext">ApplicationContext</param>
    /// <param name="activator"></param>
    /// <param name="exceptionInspector"></param>
    /// <param name="dataPortalOptions"></param>
    public SimpleDataPortal(ApplicationContext applicationContext, IDataPortalActivator activator, IDataPortalExceptionInspector exceptionInspector, Configuration.DataPortalOptions dataPortalOptions)
    {
      _applicationContext = applicationContext;
      Activator = activator;
      ExceptionInspector = exceptionInspector;
      DataPortalOptions = dataPortalOptions;
    }

    private ApplicationContext _applicationContext;
    private IDataPortalActivator Activator { get; set; }
    private IDataPortalExceptionInspector ExceptionInspector { get; set; }
    private Configuration.DataPortalOptions DataPortalOptions { get; set; }

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public async Task<DataPortalResult> Create(
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Create);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        obj.OnDataPortalInvoke(eventArgs);
        obj.MarkNew();
        await obj.CreateAsync(criteria, isSync).ConfigureAwait(false);
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
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Create " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(ExceptionInspector).InspectException(objectType, outval, criteria, "DataPortal.Create", ex),
              outval, DataPortalOptions);
      }
      finally
      {
        object reference = null;
        if (obj != null)
          reference = obj.Instance;
        Activator.FinalizeInstance(reference);
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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
      {
        return await Execute(objectType, criteria, context, isSync);
      }
      
      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Fetch);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        Activator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        obj.MarkOld();
        await obj.FetchAsync(criteria, isSync).ConfigureAwait(false);
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
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Fetch " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(ExceptionInspector).InspectException(objectType, outval, criteria, "DataPortal.Fetch", ex),
              outval, DataPortalOptions);
      }
      finally
      {
        object reference = null;
        if (obj != null)
          reference = obj.Instance;
        Activator.FinalizeInstance(reference);
      }
    }

    private async Task<DataPortalResult> Execute(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Execute);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        Activator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        obj.MarkOld();
        await obj.ExecuteAsync(criteria, isSync).ConfigureAwait(false);
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
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Execute " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(ExceptionInspector).InspectException(objectType, outval, criteria, "DataPortal.Execute", ex),
              outval, DataPortalOptions);
      }
      finally
      {
        object reference = null;
        if (obj != null)
          reference = obj.Instance;
        Activator.FinalizeInstance(reference);
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      DataPortalOperations operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      var lb = _applicationContext.CreateInstanceDI<DataPortalTarget>(obj);
      if (lb.Instance is Core.ICommandObject)
        return await Execute(lb, context, isSync);

      var eventArgs = new DataPortalEventArgs(context, objectType, obj, operation);
      try
      {
        Activator.InitializeInstance(lb.Instance);
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
              new DataPortalExceptionHandler(ExceptionInspector).InspectException(obj.GetType(), obj, null, "DataPortal.Update", ex),
              obj, DataPortalOptions);
      }
      finally
      {
        object reference = null;
        if (lb != null)
          reference = lb.Instance;
        Activator.FinalizeInstance(reference);
      }
    }

    private async Task<DataPortalResult> Execute(DataPortalTarget obj, DataPortalContext context, bool isSync)
    {
      DataPortalOperations operation = DataPortalOperations.Execute;
      Type objectType = obj.Instance.GetType();
      var eventArgs = new DataPortalEventArgs(context, objectType, obj, operation);
      try
      {
        Activator.InitializeInstance(obj.Instance);
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
        object reference = null;
        reference = obj.Instance ?? obj;
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Execute " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(ExceptionInspector).InspectException(reference.GetType(), reference, null, "DataPortal.Execute", ex),
              reference, DataPortalOptions);
      }
      finally
      {
        object reference = null;
        if (obj != null)
          reference = obj.Instance;
        Activator.FinalizeInstance(reference);
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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Delete);
      try
      {
        obj = _applicationContext.CreateInstanceDI<DataPortalTarget>(_applicationContext.CreateInstanceDI(objectType));
        Activator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        await obj.DeleteAsync(criteria, isSync).ConfigureAwait(false);
        await obj.WaitForIdle().ConfigureAwait(false);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(_applicationContext);
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
        throw DataPortal.NewDataPortalException(
              _applicationContext, "DataPortal.Delete " + Resources.FailedOnServer,
              new DataPortalExceptionHandler(ExceptionInspector).InspectException(objectType, obj, null, "DataPortal.Delete", ex),
              null, DataPortalOptions);
      }
      finally
      {
        object reference = null;
        if (obj != null)
          reference = obj.Instance;
        Activator.FinalizeInstance(reference);
      }
    }
  }
}
