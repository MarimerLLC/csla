//-----------------------------------------------------------------------
// <copyright file="SimpleDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Create);
      try
      {
        // create an instance of the business object.
        obj = new LateBoundObject(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);

        target = obj.Instance as IDataPortalTarget;

        if (target != null)
        {
          target.DataPortal_OnDataPortalInvoke(eventArgs);
          target.MarkNew();
        }
        else
        {
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs);
          obj.CallMethodIfImplemented("MarkNew");
        }

        // tell the business object to create its data
        if (criteria is EmptyCriteria)
        {
          Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Create");

          await obj.CallMethodTryAsync("DataPortal_Create").ConfigureAwait(false);
        }
        else
        {
          Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Create", criteria);

          await obj.CallMethodTryAsync("DataPortal_Create", criteria).ConfigureAwait(false);
        }

        var busy = obj.Instance as Csla.Core.ITrackStatus;
        if (busy != null && busy.IsBusy)
          throw new InvalidOperationException(string.Format("{0}.IsBusy == true", objectType.Name));

        if (target != null)
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
        else
          obj.CallMethodIfImplemented(
            "DataPortal_OnDataPortalInvokeComplete", eventArgs);

        // return the populated business object as a result
        return new DataPortalResult(obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.DataPortal_OnDataPortalException(eventArgs, ex);
          else if (obj != null)
            obj.CallMethodIfImplemented("DataPortal_OnDataPortalException", eventArgs, ex);
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
      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Fetch);
      try
      {
        // create an instance of the business object.
        obj = new LateBoundObject(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);

        target = obj.Instance as IDataPortalTarget;

        if (target != null)
        {
          target.DataPortal_OnDataPortalInvoke(eventArgs);
          target.MarkOld();
        }
        else
        {
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs);
          obj.CallMethodIfImplemented("MarkOld");
        }

        // tell the business object to fetch its data
        if (criteria is EmptyCriteria)
        {
          Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Fetch");

          await obj.CallMethodTryAsync("DataPortal_Fetch").ConfigureAwait(false);
        }
        else
        {
          Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Fetch", criteria);

          await obj.CallMethodTryAsync("DataPortal_Fetch", criteria).ConfigureAwait(false);
        }

        var busy = obj.Instance as Csla.Core.ITrackStatus;
        if (busy != null && busy.IsBusy)
          throw new InvalidOperationException(string.Format("{0}.IsBusy == true", objectType.Name));

        if (target != null)
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
        else
          obj.CallMethodIfImplemented(
            "DataPortal_OnDataPortalInvokeComplete",
            eventArgs);

        // return the populated business object as a result
        return new DataPortalResult(obj.Instance);
      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.DataPortal_OnDataPortalException(eventArgs, ex);
          else if (obj != null)
            obj.CallMethodIfImplemented("DataPortal_OnDataPortalException", eventArgs, ex);
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
      var target = obj as IDataPortalTarget;
      LateBoundObject lb = new LateBoundObject(obj);
      ApplicationContext.DataPortalActivator.InitializeInstance(lb.Instance);
      try
      {
        if (target != null)
          target.DataPortal_OnDataPortalInvoke(
            new DataPortalEventArgs(context, objectType, obj, operation));
        else
          lb.CallMethodIfImplemented(
            "DataPortal_OnDataPortalInvoke",
            new DataPortalEventArgs(context, objectType, obj, operation));

        // tell the business object to update itself
        var busObj = obj as Core.BusinessBase;
        if (busObj != null)
        {
          if (busObj.IsDeleted)
          {
            if (!busObj.IsNew)
            {
              Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_DeleteSelf");

              // tell the object to delete itself
              await lb.CallMethodTryAsync("DataPortal_DeleteSelf").ConfigureAwait(false);
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
              Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Insert");

              // tell the object to insert itself
              await lb.CallMethodTryAsync("DataPortal_Insert").ConfigureAwait(false);
            }
            else
            {
              Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Update");

              // tell the object to update itself
              await lb.CallMethodTryAsync("DataPortal_Update").ConfigureAwait(false);
            }
            if (target != null)
              target.MarkOld();
            else
              lb.CallMethodIfImplemented("MarkOld");
          }
        }
        else if (obj is Core.ICommandObject)
        {
          operation = DataPortalOperations.Execute;

          Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Execute");

          // tell the object to update itself
          await lb.CallMethodTryAsync("DataPortal_Execute").ConfigureAwait(false);
        }
        else
        {
          Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Update");

          // this is an updatable collection or some other
          // non-BusinessBase type of object
          // tell the object to update itself
          await lb.CallMethodTryAsync("DataPortal_Update").ConfigureAwait(false);
          if (target != null)
            target.MarkOld();
          else
            lb.CallMethodIfImplemented("MarkOld");
        }

        var busy = busObj as Csla.Core.ITrackStatus;
        if (busy != null && busy.IsBusy)
          throw new InvalidOperationException(string.Format("{0}.IsBusy == true", objectType.Name));

        if (target != null)
          target.DataPortal_OnDataPortalInvokeComplete(
            new DataPortalEventArgs(context, objectType, obj, operation));
        else
          lb.CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete",
            new DataPortalEventArgs(context, objectType, obj, operation));

        return new DataPortalResult(obj);
      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.DataPortal_OnDataPortalException(
              new DataPortalEventArgs(context, objectType, obj, operation), ex);
          else
            lb.CallMethodIfImplemented("DataPortal_OnDataPortalException", new DataPortalEventArgs(context, objectType, obj, operation), ex);
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
      LateBoundObject obj = null;
      IDataPortalTarget target = null;
      var eventArgs = new DataPortalEventArgs(context, objectType, criteria, DataPortalOperations.Delete);
      try
      {
        // create an instance of the business objet
        obj = new LateBoundObject(ApplicationContext.DataPortalActivator.CreateInstance(objectType));
        ApplicationContext.DataPortalActivator.InitializeInstance(obj.Instance);

        target = obj.Instance as IDataPortalTarget;

        if (target != null)
          target.DataPortal_OnDataPortalInvoke(eventArgs);
        else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs);

        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, target, "DataPortal_Delete", criteria);

        // tell the business object to delete itself
        await obj.CallMethodTryAsync("DataPortal_Delete", criteria).ConfigureAwait(false);

        var busy = obj.Instance as Csla.Core.ITrackStatus;
        if (busy != null && busy.IsBusy)
          throw new InvalidOperationException(string.Format("{0}.IsBusy == true", objectType.Name));

        if (target != null)
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
        else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete", eventArgs);

        return new DataPortalResult();
      }
      catch (Exception ex)
      {
        try
        {
          if (target != null)
            target.DataPortal_OnDataPortalException(eventArgs, ex);
          else if (obj != null)
            obj.CallMethodIfImplemented("DataPortal_OnDataPortalException", eventArgs, ex);
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