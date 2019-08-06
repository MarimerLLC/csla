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
        (t) => DataPortalMethodNames.Default);
    }

    public void OnDataPortalInvoke(DataPortalEventArgs eventArgs)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalInvoke(eventArgs);
      else
        CallMethodIfImplemented(_methodNames.OnDataPortalInvoke, eventArgs);
    }

    public void Child_OnDataPortalInvoke(DataPortalEventArgs eventArgs)
    {
      if (_target != null)
        _target.Child_OnDataPortalInvoke(eventArgs);
      else
        CallMethodIfImplemented("Child_OnDataPortalInvoke", eventArgs);
    }


    public void OnDataPortalInvokeComplete(DataPortalEventArgs eventArgs)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalInvokeComplete(eventArgs);
      else
        CallMethodIfImplemented(_methodNames.OnDataPortalInvokeComplete, eventArgs);
    }

    internal void Child_OnDataPortalInvokeComplete(DataPortalEventArgs eventArgs)
    {
      if (_target != null)
        _target.Child_OnDataPortalInvokeComplete(eventArgs);
      else
        CallMethodIfImplemented("Child_OnDataPortalInvokeComplete", eventArgs);
    }

    internal void OnDataPortalException(DataPortalEventArgs eventArgs, Exception ex)
    {
      if (_target != null)
        _target.DataPortal_OnDataPortalException(eventArgs, ex);
      else
        CallMethodIfImplemented(_methodNames.OnDataPortalException, eventArgs, ex);
    }


    internal void Child_OnDataPortalException(DataPortalEventArgs eventArgs, Exception ex)
    {
      if (_target != null)
        _target.Child_OnDataPortalException(eventArgs, ex);
      else
        CallMethodIfImplemented("Child_OnDataPortalException", eventArgs, ex);
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

    public void MarkAsChild()
    {
      if (_target != null)
        _target.MarkAsChild();
      else
        CallMethodIfImplemented("MarkAsChild");
    }

    internal void MarkOld()
    {
      if (_target != null)
        _target.MarkOld();
      else
        CallMethodIfImplemented("MarkOld");
    }

#if !NET40
    private async Task InvokeOperationAsync(object criteria, bool isSync, Type attributeType)
    {
      object[] parameters = DataPortal<object>.GetCriteriaArray(criteria);
      await CallMethodTryAsyncDI(isSync, attributeType, parameters).ConfigureAwait(false);
    }
#endif

    public async Task CreateAsync(object criteria, bool isSync)
    {
#if NET40
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
#else
      await InvokeOperationAsync(criteria, isSync, typeof(CreateAttribute)).ConfigureAwait(false);
#endif
    }

    public async Task CreateChildAsync(object criteria)
    {
#if NET40
      if (criteria is EmptyCriteria)
      {
        await CallMethodTryAsync(_methodNames.CreateChild).ConfigureAwait(false);
      }
      else
      {
        if (criteria is Core.MobileList<object> list)
          await CallMethodTryAsync(_methodNames.CreateChild, list.ToArray()).ConfigureAwait(false);
        else
          await CallMethodTryAsync(_methodNames.CreateChild, criteria).ConfigureAwait(false);
      }
#else
      await InvokeOperationAsync(criteria, false, typeof(CreateChildAttribute)).ConfigureAwait(false);
#endif
    }

    public async Task FetchAsync(object criteria, bool isSync)
    {
#if NET40
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
#else
      await InvokeOperationAsync(criteria, isSync, typeof(FetchAttribute)).ConfigureAwait(false);
#endif
    }

    public async Task FetchChildAsync(object criteria)
    {
#if NET40
      if (criteria is EmptyCriteria)
      {
        await CallMethodTryAsync(_methodNames.FetchChild).ConfigureAwait(false);
      }
      else
      {
        if (criteria is Core.MobileList<object> list)
          await CallMethodTryAsync(_methodNames.FetchChild, list.ToArray()).ConfigureAwait(false);
        else
          await CallMethodTryAsync(_methodNames.FetchChild, criteria).ConfigureAwait(false);
      }
#else
      await InvokeOperationAsync(criteria, false, typeof(FetchChildAttribute)).ConfigureAwait(false);
#endif
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
#if NET40
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.DeleteSelf);
            await CallMethodTryAsync(_methodNames.DeleteSelf).ConfigureAwait(false);
#else
            await InvokeOperationAsync(EmptyCriteria.Instance, isSync, typeof(DeleteSelfAttribute)).ConfigureAwait(false);
#endif
          }
          MarkNew();
        }
        else
        {
          if (busObj.IsNew)
          {
            // tell the object to insert itself
#if NET40
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Insert);
            await CallMethodTryAsync(_methodNames.Insert).ConfigureAwait(false);
#else
            await InvokeOperationAsync(EmptyCriteria.Instance, isSync, typeof(InsertAttribute)).ConfigureAwait(false);
#endif
          }
          else
          {
            // tell the object to update itself
#if NET40
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Update);
            await CallMethodTryAsync(_methodNames.Update).ConfigureAwait(false);
#else
            await InvokeOperationAsync(EmptyCriteria.Instance, isSync, typeof(UpdateAttribute)).ConfigureAwait(false);
#endif
          }
          MarkOld();
        }
      }
      else
      {
        // this is an updatable collection or some other
        // non-BusinessBase type of object
        // tell the object to update itself
#if NET40
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Update);
        await CallMethodTryAsync(_methodNames.Update).ConfigureAwait(false);
#else
        await InvokeOperationAsync(EmptyCriteria.Instance, isSync, typeof(UpdateAttribute)).ConfigureAwait(false);
#endif
        MarkOld();
      }
    }

    public async Task UpdateChildAsync(object criteria)
    {
      // tell the business object to update itself
      if (Instance is Core.BusinessBase busObj)
      {
        if (busObj.IsDeleted)
        {
          if (!busObj.IsNew)
          {
            // tell the object to delete itself
#if NET40
            await CallMethodTryAsync(_methodNames.DeleteSelfChild, criteria).ConfigureAwait(false);
#else
            await InvokeOperationAsync(criteria, false, typeof(DeleteSelfChildAttribute)).ConfigureAwait(false);
#endif
            MarkNew();
          }
        }
        else
        {
          if (busObj.IsNew)
          {
            // tell the object to insert itself
#if NET40
            await CallMethodTryAsync(_methodNames.InsertChild, criteria).ConfigureAwait(false);
#else
            await InvokeOperationAsync(criteria, false, typeof(InsertChildAttribute)).ConfigureAwait(false);
#endif
          }
          else
          {
            // tell the object to update itself
#if NET40
            await CallMethodTryAsync(_methodNames.UpdateChild, criteria).ConfigureAwait(false);
#else
            await InvokeOperationAsync(criteria, false, typeof(UpdateChildAttribute)).ConfigureAwait(false);
#endif
          }
          MarkOld();
        }

      }
      else if (Instance is Core.ICommandObject)
      {
        // tell the object to update itself
#if NET40
        await CallMethodTryAsync(_methodNames.ExecuteChild, criteria).ConfigureAwait(false);
#else
        await InvokeOperationAsync(criteria, false, typeof(ExecuteChildAttribute)).ConfigureAwait(false);
#endif
      }
      else
      {
        // this is an updatable collection or some other
        // non-BusinessBase type of object
        // tell the object to update itself
#if NET40
        await CallMethodTryAsync(_methodNames.UpdateChild, criteria).ConfigureAwait(false);
#else
        await InvokeOperationAsync(criteria, false, typeof(UpdateChildAttribute)).ConfigureAwait(false);
#endif
        MarkOld();
      }
    }

    public async Task ExecuteAsync(bool isSync)
    {
#if NET40
      Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Execute);
      await CallMethodTryAsync(_methodNames.Execute).ConfigureAwait(false);
#else
      await InvokeOperationAsync(EmptyCriteria.Instance, isSync, typeof(ExecuteAttribute)).ConfigureAwait(false);
#endif
    }

    public async Task DeleteAsync(object criteria, bool isSync)
    {
#if NET40
      Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Delete, criteria);
      await CallMethodTryAsync(_methodNames.Delete, criteria).ConfigureAwait(false);
#else
      await InvokeOperationAsync(criteria, isSync, typeof(DeleteAttribute)).ConfigureAwait(false);
#endif
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
    public string CreateChild { get; set; } = "Child_Create";
    public string FetchChild { get; set; } = "Child_Fetch";
    public string UpdateChild { get; set; } = "Child_Update";
    public string InsertChild { get; set; } = "Child_Insert";
    public string DeleteSelfChild { get; set; } = "Child_DeleteSelf";
    public string ExecuteChild { get; set; } = "Child_Execute";
    public string OnDataPortalInvoke { get; set; } = "DataPortal_OnDataPortalInvoke";
    public string OnDataPortalInvokeComplete { get; set; } = "DataPortal_OnDataPortalInvokeComplete";
    public string OnDataPortalException { get; set; } = "DataPortal_OnDataPortalException";
  }
}
