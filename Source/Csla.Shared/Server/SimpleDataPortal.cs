//-----------------------------------------------------------------------
// <copyright file="SimpleDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side DataPortal as discussed</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side DataPortal as discussed
  /// in Chapter 4.
  /// </summary>
  public class SimpleDataPortal : IDataPortalServer
  {
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
        obj = new DataPortalTarget(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        obj.MarkNew();
        await obj.CreateAsync(criteria, isSync);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          if (obj != null)
            obj.OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw DataPortal.NewDataPortalException(
              "DataPortal.Create " + Resources.FailedOnServer,
              new DataPortalExceptionHandler().InspectException(objectType, outval, criteria, "DataPortal.Create", ex),
              outval);
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
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalTarget obj = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Fetch);
      try
      {
        obj = new DataPortalTarget(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        obj.MarkOld();
        await obj.FetchAsync(criteria, isSync);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          if (obj != null)
            obj.OnDataPortalException(eventArgs, ex);
        }
        catch
        {
          // ignore exceptions from the exception handler
        }
        object outval = null;
        if (obj != null) outval = obj.Instance;
        throw DataPortal.NewDataPortalException(
              "DataPortal.Fetch " + Resources.FailedOnServer,
              new DataPortalExceptionHandler().InspectException(objectType, outval, criteria, "DataPortal.Fetch", ex),
              outval);
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
      var lb = new DataPortalTarget(obj);
      if (lb.Instance is Core.ICommandObject)
        return await Execute(lb, context, isSync);

      var eventArgs = new DataPortalEventArgs(context, objectType, obj, operation);
      try
      {
        ApplicationContext.DataPortalActivator.InitializeInstance(lb.Instance);
        lb.OnDataPortalInvoke(eventArgs);
        await lb.UpdateAsync(isSync);
        lb.ThrowIfBusy();
        lb.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(lb.Instance);
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
              "DataPortal.Update " + Resources.FailedOnServer,
              new DataPortalExceptionHandler().InspectException(obj.GetType(), obj, null, "DataPortal.Update", ex),
              obj);
      }
      finally
      {
        object reference = null;
        if (lb != null)
          reference = lb.Instance;
        ApplicationContext.DataPortalActivator.FinalizeInstance(reference);
      }
    }

    private async Task<DataPortalResult> Execute(DataPortalTarget obj, DataPortalContext context, bool isSync)
    {
      DataPortalOperations operation = DataPortalOperations.Execute;
      Type objectType = obj.Instance.GetType();
      var eventArgs = new DataPortalEventArgs(context, objectType, obj, operation);
      try
      {
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        await obj.ExecuteAsync(isSync);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult(obj.Instance);
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
              "DataPortal.Execute " + Resources.FailedOnServer,
              new DataPortalExceptionHandler().InspectException(reference.GetType(), reference, null, "DataPortal.Execute", ex),
              reference);       
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
        obj = new DataPortalTarget(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);
        obj.OnDataPortalInvoke(eventArgs);
        await obj.DeleteAsync(criteria, isSync);
        obj.ThrowIfBusy();
        obj.OnDataPortalInvokeComplete(eventArgs);
        return new DataPortalResult();
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
              "DataPortal.Delete " + Resources.FailedOnServer,
              new DataPortalExceptionHandler().InspectException(objectType, obj, null, "DataPortal.Delete", ex),
              null);
      }
      finally
      {
        object reference = null;
        if (obj != null)
          reference = obj.Instance;
        ApplicationContext.DataPortalActivator.FinalizeInstance(reference);
      }
    }
  }
}