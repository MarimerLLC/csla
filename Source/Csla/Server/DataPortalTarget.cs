//-----------------------------------------------------------------------
// <copyright file="DataPortalTarget.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Encapsulates server-side data portal invocations</summary>
//-----------------------------------------------------------------------

using Csla.Core;

#if NET8_0_OR_GREATER
using System.Runtime.Loader;

using Csla.Runtime;
#endif
using Csla.Reflection;

namespace Csla.Server
{
  internal class DataPortalTarget : LateBoundObject
  {
#if NET8_0_OR_GREATER
    private static readonly ConcurrentDictionary<Type, Tuple<string, DataPortalMethodNames>> _methodNameList = new();
#else
    private static readonly ConcurrentDictionary<Type, DataPortalMethodNames> _methodNameList = new();
#endif

    private readonly IDataPortalTarget _target;
    private readonly TimeSpan _waitForIdleTimeout;
    private readonly DataPortalMethodNames _methodNames;

    public DataPortalTarget(object obj, Configuration.CslaOptions cslaOptions)
      : base(obj)
    {
      _target = obj as IDataPortalTarget;
      _waitForIdleTimeout = TimeSpan.FromSeconds(cslaOptions.DefaultWaitForIdleTimeoutInSeconds);

#if NET8_0_OR_GREATER
      var objectType = obj.GetType();

      var methodNameListInfo = _methodNameList.GetOrAdd(
        objectType,
        _ => AssemblyLoadContextManager.CreateCacheInstance(
          objectType,
          DataPortalMethodNames.Default,
          OnAssemblyLoadContextUnload
        )
      );

      _methodNames = methodNameListInfo.Item2;
#else
      _methodNames = _methodNameList.GetOrAdd(obj.GetType(),
        _ => DataPortalMethodNames.Default);
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
      if (Instance is ITrackStatus busy && busy.IsBusy)
        throw new InvalidOperationException($"{Instance.GetType().Name}.IsBusy == true");
    }

    public async Task WaitForIdle()
    {
      if (_target is not null) 
      {
        await _target.WaitForIdle(_waitForIdleTimeout).ConfigureAwait(false);
      }
      else if(Instance is INotifyBusy notifyBusy) 
      {
        await BusyHelper.WaitForIdle(notifyBusy, _waitForIdleTimeout).ConfigureAwait(false);
      }
      else 
      {
        CallMethodIfImplemented(nameof(IDataPortalTarget.WaitForIdle), _waitForIdleTimeout);
      }
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

    private async Task InvokeOperationAsync<T>(object criteria, bool isSync)
      where T : DataPortalOperationAttribute
    {
      object[] parameters = DataPortal.GetCriteriaArray(criteria);
      await CallMethodTryAsyncDI<T>(isSync, parameters).ConfigureAwait(false);
    }

    public Task CreateAsync(object criteria, bool isSync)
    {
      return InvokeOperationAsync<CreateAttribute>(criteria, isSync);
    }

    public Task CreateChildAsync(params object[] parameters)
    {
      return CallMethodTryAsyncDI<CreateChildAttribute>(false, parameters);
    }

    public Task FetchAsync(object criteria, bool isSync)
    {
      return InvokeOperationAsync<FetchAttribute>(criteria, isSync);
    }

    public Task FetchChildAsync(params object[] parameters)
    {
      return CallMethodTryAsyncDI<FetchChildAttribute>(false, parameters);
    }

    public Task ExecuteAsync(object criteria, bool isSync)
    {
      return InvokeOperationAsync<ExecuteAttribute>(criteria, isSync);
    }

    public async Task UpdateAsync(bool isSync)
    {
      if (Instance is BusinessBase busObj)
      {
        if (busObj.IsDeleted)
        {
          if (!busObj.IsNew)
          {
            // tell the object to delete itself
            await InvokeOperationAsync<DeleteSelfAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
          }
          MarkNew();
        }
        else
        {
          if (busObj.IsNew)
          {
            // tell the object to insert itself
            await InvokeOperationAsync<InsertAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
          }
          else
          {
            // tell the object to update itself
            await InvokeOperationAsync<UpdateAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
          }
          MarkOld();
        }
      }
      else
      {
        // this is an updatable collection or some other
        // non-BusinessBase type of object
        // tell the object to update itself
        await InvokeOperationAsync<UpdateAttribute>(EmptyCriteria.Instance, isSync).ConfigureAwait(false);
        MarkOld();
      }
    }

    public async Task UpdateChildAsync(params object[] parameters)
    {
      // tell the business object to update itself
      if (Instance is BusinessBase busObj)
      {
        if (busObj.IsDeleted)
        {
          if (!busObj.IsNew)
          {
            // tell the object to delete itself
            await CallMethodTryAsyncDI<DeleteSelfChildAttribute>(false, parameters).ConfigureAwait(false);
            MarkNew();
          }
        }
        else
        {
          if (busObj.IsNew)
          {
            // tell the object to insert itself
            await CallMethodTryAsyncDI<InsertChildAttribute>(false, parameters).ConfigureAwait(false);
          }
          else
          {
            // tell the object to update itself
            await CallMethodTryAsyncDI<UpdateChildAttribute>(false, parameters).ConfigureAwait(false);
          }
          MarkOld();
        }

      }
      else if (Instance is ICommandObject)
      {
        // tell the object to update itself
        await CallMethodTryAsyncDI<ExecuteChildAttribute>(false, parameters).ConfigureAwait(false);
      }
      else
      {
        // this is an updatable collection or some other
        // non-BusinessBase type of object
        // tell the object to update itself
        await CallMethodTryAsyncDI<UpdateChildAttribute>(false, parameters).ConfigureAwait(false);
        MarkOld();
      }
    }

    public Task ExecuteAsync(bool isSync)
    {
      return InvokeOperationAsync<ExecuteAttribute>(EmptyCriteria.Instance, isSync);
    }

    public Task DeleteAsync(object criteria, bool isSync)
    {
      return InvokeOperationAsync<DeleteAttribute>(criteria, isSync);
    }
#if NET8_0_OR_GREATER

    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      AssemblyLoadContextManager.RemoveFromCache(_methodNameList, context, true);
    }
#endif
  }

  internal class DataPortalMethodNames
  {
    public static readonly DataPortalMethodNames Default = new();
    public string OnDataPortalInvoke { get; set; } = "DataPortal_OnDataPortalInvoke";
    public string OnDataPortalInvokeComplete { get; set; } = "DataPortal_OnDataPortalInvokeComplete";
    public string OnDataPortalException { get; set; } = "DataPortal_OnDataPortalException";
  }
}
