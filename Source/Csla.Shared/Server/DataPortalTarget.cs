//-----------------------------------------------------------------------
// <copyright file="DataPortalTarget.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Encapsulates server-side data portal invocations</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Csla.Reflection;

namespace Csla.Server
{
  internal class DataPortalTarget : LateBoundObject
  {
    private readonly IDataPortalTarget _target;

    public DataPortalTarget(object obj)
      : base(obj)
    {
      _target = obj as IDataPortalTarget;
    }

    public void OnDataPortalInvoke(DataPortalEventArgs eventArgs)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalInvoke(eventArgs);
      else
        CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs);
    }

    public void OnDataPortalInvokeComplete(DataPortalEventArgs eventArgs)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
      else
        CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete", eventArgs);
    }

    internal void OnDataPortalException(DataPortalEventArgs eventArgs, Exception ex)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalException(eventArgs, ex);
      else
        CallMethodIfImplemented("DataPortal_OnDataPortalException", eventArgs, ex);
    }

    public void ThrowIfBusy()
    {
      if (Instance is Csla.Core.ITrackStatus busy && busy.IsBusy)
        throw new InvalidOperationException(string.Format("{0}.IsBusy == true", Instance.GetType().Name));
    }

    public void MarkNew()
    {
      if (_target != null)
        _target.MarkNew();
      else
        CallMethodIfImplemented("MarkNew");
    }

    internal void MarkOld()
    {
      if (_target != null)
        _target.MarkOld();
      else
        CallMethodIfImplemented("MarkOld");
    }

    public async Task CreateAsync(object criteria, bool isSync)
    {
      if (criteria is EmptyCriteria)
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Create");
        await CallMethodTryAsync("DataPortal_Create").ConfigureAwait(false);
      }
      else
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Create", criteria);
        await CallMethodTryAsync("DataPortal_Create", criteria).ConfigureAwait(false);
      }
    }

    public async Task FetchAsync(object criteria, bool isSync)
    {
      if (criteria is EmptyCriteria)
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Fetch");
        await CallMethodTryAsync("DataPortal_Fetch").ConfigureAwait(false);
      }
      else
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Fetch", criteria);
        await CallMethodTryAsync("DataPortal_Fetch", criteria).ConfigureAwait(false);
      }
    }

    public async Task UpdateAsync(bool isSync)
    {
      if (Instance is Core.BusinessBase busObj)
      {
        if (busObj.IsDeleted)
        {
          if (!busObj.IsNew)
          {
            // tell the object to delete itself
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_DeleteSelf");
            await CallMethodTryAsync("DataPortal_DeleteSelf").ConfigureAwait(false);
          }
          MarkNew();
        }
        else
        {
          if (busObj.IsNew)
          {
            // tell the object to insert itself
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Insert");
            await CallMethodTryAsync("DataPortal_Insert").ConfigureAwait(false);
          }
          else
          {
            // tell the object to update itself
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Update");
            await CallMethodTryAsync("DataPortal_Update").ConfigureAwait(false);
          }
          MarkOld();
        }
      }
      else
      {
        // this is an updatable collection or some other
        // non-BusinessBase type of object
        // tell the object to update itself
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Update");
        await CallMethodTryAsync("DataPortal_Update").ConfigureAwait(false);
        MarkOld();
      }
    }

    public async Task ExecuteAsync(bool isSync)
    {
      Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Execute");
      await CallMethodTryAsync("DataPortal_Execute").ConfigureAwait(false);
    }

    public async Task DeleteAsync(object criteria, bool isSync)
    {
      Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, "DataPortal_Delete", criteria);
      await CallMethodTryAsync("DataPortal_Delete", criteria).ConfigureAwait(false);
    }
  }
}
