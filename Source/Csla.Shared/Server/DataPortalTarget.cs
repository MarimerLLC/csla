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

#if NET5_0_OR_GREATER
using System.Runtime.Loader;

using Csla.Runtime;
#endif

using Csla.Reflection;

namespace Csla.Server
{
  internal class DataPortalTarget : LateBoundObject
  {
#if NET5_0_OR_GREATER
    private static readonly ConcurrentDictionary<Type, Tuple<string, DataPortalMethodNames>> _methodNameList =
      new ConcurrentDictionary<Type, Tuple<string, DataPortalMethodNames>>();
#else
    private static readonly ConcurrentDictionary<Type, DataPortalMethodNames> _methodNameList =
      new ConcurrentDictionary<Type, DataPortalMethodNames>();
#endif

    private readonly IDataPortalTarget _target;
    private readonly DataPortalMethodNames _methodNames;

    public DataPortalTarget(object obj)
      : base(obj)
    {
      _target = obj as IDataPortalTarget;
#if NET5_0_OR_GREATER
      var objectType = obj.GetType();

      var methodNameListInfo = _methodNameList.GetOrAdd(
        objectType,
        (t) => AssemblyLoadContextManager.CreateCacheInstance(
          objectType,
          DataPortalMethodNames.Default,
          OnAssemblyLoadContextUnload
        )
      );

      _methodNames = methodNameListInfo.Item2;
#else
      _methodNames = _methodNameList.GetOrAdd(obj.GetType(), 
        (t) => DataPortalMethodNames.Default);
#endif
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
    private async Task InvokeOperationAsync<T>(object criteria, bool isSync)
      where T : DataPortalOperationAttribute
    {
      object[] parameters = DataPortal.GetCriteriaArray(criteria);
      await CallMethodTryAsyncDI<T>(isSync, parameters).ConfigureAwait(false);
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
      await InvokeOperationAsync<CreateAttribute>(criteria, isSync).ConfigureAwait(false);
#endif
    }

    public async Task CreateChildAsync(params object[] parameters)
    {
#if NET40
      await CallMethodTryAsync(_methodNames.CreateChild, parameters).ConfigureAwait(false);
#else
      await CallMethodTryAsyncDI<CreateChildAttribute>(false, parameters).ConfigureAwait(false);
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
      await InvokeOperationAsync<FetchAttribute>(criteria, isSync).ConfigureAwait(false);
#endif
    }

    public async Task FetchChildAsync(params object[] parameters)
    {
#if NET40
      await CallMethodTryAsync(_methodNames.FetchChild, parameters).ConfigureAwait(false);
#else
      await CallMethodTryAsyncDI<FetchChildAttribute>(false, parameters).ConfigureAwait(false);
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
            await InvokeOperationAsync<DeleteSelfAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
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
            await InvokeOperationAsync<InsertAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
#endif
          }
          else
          {
            // tell the object to update itself
#if NET40
            Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Update);
            await CallMethodTryAsync(_methodNames.Update).ConfigureAwait(false);
#else
            await InvokeOperationAsync<UpdateAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
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
        await InvokeOperationAsync<UpdateAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
#endif
        MarkOld();
      }
    }

    public async Task UpdateChildAsync(params object[] parameters)
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
            await CallMethodTryAsync(_methodNames.DeleteSelfChild, parameters).ConfigureAwait(false);
#else
            await CallMethodTryAsyncDI<DeleteSelfChildAttribute>(false, parameters).ConfigureAwait(false);
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
            await CallMethodTryAsync(_methodNames.InsertChild, parameters).ConfigureAwait(false);
#else
            await CallMethodTryAsyncDI<InsertChildAttribute>(false, parameters).ConfigureAwait(false);
#endif
          }
          else
          {
            // tell the object to update itself
#if NET40
            await CallMethodTryAsync(_methodNames.UpdateChild, parameters).ConfigureAwait(false);
#else
            await CallMethodTryAsyncDI<UpdateChildAttribute>(false, parameters).ConfigureAwait(false);
#endif
          }
          MarkOld();
        }

      }
      else if (Instance is Core.ICommandObject)
      {
        // tell the object to update itself
#if NET40
        await CallMethodTryAsync(_methodNames.ExecuteChild, parameters).ConfigureAwait(false);
#else
        await CallMethodTryAsyncDI<ExecuteChildAttribute>(false, parameters).ConfigureAwait(false);
#endif
      }
      else
      {
        // this is an updatable collection or some other
        // non-BusinessBase type of object
        // tell the object to update itself
#if NET40
        await CallMethodTryAsync(_methodNames.UpdateChild, parameters).ConfigureAwait(false);
#else
        await CallMethodTryAsyncDI<UpdateChildAttribute>(false, parameters).ConfigureAwait(false);
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
      await InvokeOperationAsync<ExecuteAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
#endif
    }

    public async Task DeleteAsync(object criteria, bool isSync)
    {
#if NET40
      Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, Instance, _methodNames.Delete, criteria);
      await CallMethodTryAsync(_methodNames.Delete, criteria).ConfigureAwait(false);
#else
      await InvokeOperationAsync<DeleteAttribute>(criteria, isSync).ConfigureAwait(false);
#endif
    }

#if NET5_0_OR_GREATER
    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      AssemblyLoadContextManager.RemoveFromCache(_methodNameList, context, true);
    }
#endif
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
