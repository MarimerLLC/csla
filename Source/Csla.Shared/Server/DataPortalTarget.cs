//-----------------------------------------------------------------------
// <copyright file="DataPortalTarget.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Encapsulates server-side data portal invocations</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Csla.Reflection;

namespace Csla.Server
{
  internal class DataPortalTarget : LateBoundObject
  {
    private static readonly ConcurrentDictionary<Type, DataPortalMethodNames> _methodNameList =
      new ConcurrentDictionary<Type, DataPortalMethodNames>();
    private readonly IDataPortalTarget _target;
    private readonly DataPortalMethodNames _methodNames;

    public DataPortalTarget(object obj)
      : base(obj)
    {
      _target = obj as IDataPortalTarget;
      _methodNames = _methodNameList.GetOrAdd(obj.GetType(), 
        (t) => DataPortalMethodNames.Default); // TODO: get method names dynamically
    }

    public void OnDataPortalInvoke(DataPortalEventArgs eventArgs)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalInvoke(eventArgs);
      else
        CallMethodIfImplemented(_methodNames.OnDataPortalInvoke, eventArgs);
    }

    public void OnDataPortalInvokeComplete(DataPortalEventArgs eventArgs)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
      else
        CallMethodIfImplemented(_methodNames.OnDataPortalInvokeComplete, eventArgs);
    }

    internal void OnDataPortalException(DataPortalEventArgs eventArgs, Exception ex)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalException(eventArgs, ex);
      else
        CallMethodIfImplemented(_methodNames.OnDataPortalException, eventArgs, ex);
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
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Create);
        await CallMethodTryAsync(_methodNames.Create).ConfigureAwait(false);
      }
      else
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.CreateCriteria, criteria);
        if (criteria is Core.MobileList<object> list)
          await CallMethodTryAsync(_methodNames.CreateCriteria, list.ToArray()).ConfigureAwait(false);
        else
          await CallMethodTryAsync(_methodNames.CreateCriteria, criteria).ConfigureAwait(false);
      }
    }

    public async Task FetchAsync(object criteria, bool isSync)
    {
      if (criteria is EmptyCriteria)
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Fetch);
        await CallMethodTryAsync(_methodNames.Fetch).ConfigureAwait(false);
      }
      else
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.FetchCriteria, criteria);
        await CallMethodTryAsync(_methodNames.FetchCriteria, criteria).ConfigureAwait(false);
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
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.DeleteSelf);
            await CallMethodTryAsync(_methodNames.DeleteSelf).ConfigureAwait(false);
          }
          MarkNew();
        }
        else
        {
          if (busObj.IsNew)
          {
            // tell the object to insert itself
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Insert);
            await CallMethodTryAsync(_methodNames.Insert).ConfigureAwait(false);
          }
          else
          {
            // tell the object to update itself
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Update);
            await CallMethodTryAsync(_methodNames.Update).ConfigureAwait(false);
          }
          MarkOld();
        }
      }
      else
      {
        // this is an updatable collection or some other
        // non-BusinessBase type of object
        // tell the object to update itself
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Update);
        await CallMethodTryAsync(_methodNames.Update).ConfigureAwait(false);
        MarkOld();
      }
    }

    public async Task ExecuteAsync(bool isSync)
    {
      Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Execute);
      await CallMethodTryAsync(_methodNames.Execute).ConfigureAwait(false);
    }

    public async Task DeleteAsync(object criteria, bool isSync)
    {
      Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Delete, criteria);
      await CallMethodTryAsync(_methodNames.Delete, criteria).ConfigureAwait(false);
    }
  }

  internal class DataPortalMethodNames
  {
    public static readonly DataPortalMethodNames Default = new DataPortalMethodNames();
    public string Create { get; set; } = "DataPortal_Create";
    public string CreateCriteria { get; set; } = "DataPortal_Create";
    public string Fetch { get; set; } = "DataPortal_Fetch";
    public string FetchCriteria { get; set; } = "DataPortal_Fetch";
    public string Insert { get; set; } = "DataPortal_Insert";
    public string Update { get; set; } = "DataPortal_Update";
    public string Execute { get; set; } = "DataPortal_Execute";
    public string Delete { get; set; } = "DataPortal_Delete";
    public string DeleteSelf { get; set; } = "DataPortal_DeleteSelf";
    public string OnDataPortalInvoke { get; set; } = "DataPortal_OnDataPortalInvoke";
    public string OnDataPortalInvokeComplete { get; set; } = "DataPortal_OnDataPortalInvokeComplete";
    public string OnDataPortalException { get; set; } = "DataPortal_OnDataPortalException";
  }
}
