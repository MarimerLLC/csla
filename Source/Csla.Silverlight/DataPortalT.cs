//-----------------------------------------------------------------------
// <copyright file="DataPortalT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Creates, retrieves, updates or deletes a</summary>
//-----------------------------------------------------------------------
using System;
using System.ServiceModel;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.DataPortalClient;

namespace Csla
{
  /// <summary>
  /// Creates, retrieves, updates or deletes a
  /// business object.
  /// </summary>
  /// <typeparam name="T">
  /// Type of business object.
  /// </typeparam>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  public class DataPortal<T> : DataPortalClient.IDataPortalProxy<T>, IDataPortal<T>
    where T : IMobileObject
  {
    #region GlobalContext

    /// <summary>
    /// Gets the global context value returned by
    /// the async operation.
    /// </summary>
    public Csla.Core.ContextDictionary GlobalContext
    {
      get
      {
        if (_proxy == null)
          return Csla.ApplicationContext.GlobalContext;
        else
          return _proxy.GlobalContext;
      }
    }

    #endregion

    private DataPortalClient.IDataPortalProxy<T> _proxy;

    /// <summary>
    /// Creates an instance of the data portal
    /// object, choosing a proxy object based
    /// on current configuration.
    /// </summary>
    public DataPortal()
      : this(DataPortal.ProxyModes.Auto)
    { }

    /// <summary>
    /// Creates an instance of the data portal
    /// object, allowing the caller to specify
    /// the type of proxy object to use.
    /// </summary>
    /// <param name="proxyMode">
    /// Proxy mode used by this data portal instance.
    /// </param>
    public DataPortal(DataPortal.ProxyModes proxyMode)
    {
      _proxy = DataPortal.ProxyFactory.GetProxy<T>(proxyMode);
      HookEvents(_proxy);
    }

    private void HookEvents(DataPortalClient.IDataPortalProxy<T> proxy)
    {
      _proxy.CreateCompleted += new EventHandler<DataPortalResult<T>>(proxy_CreateCompleted);
      _proxy.FetchCompleted += new EventHandler<DataPortalResult<T>>(proxy_FetchCompleted);
      _proxy.UpdateCompleted += new EventHandler<DataPortalResult<T>>(proxy_UpdateCompleted);
      _proxy.DeleteCompleted += new EventHandler<DataPortalResult<T>>(proxy_DeleteCompleted);
      _proxy.ExecuteCompleted += new EventHandler<DataPortalResult<T>>(proxy_ExecuteCompleted);
    }

    #region Create

    /// <summary>
    /// Event raised when an asynchronous create method
    /// completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    /// <summary>
    /// Raises the CreateCompleted event
    /// </summary>
    /// <param name="e">
    /// Object containing the results of the data portal call.
    /// </param>
    protected virtual void OnCreateCompleted(DataPortalResult<T> e)
    {
      if (CreateCompleted != null)
        CreateCompleted(this, e);
    }

    /// <summary>
    /// Starts an asynchronous create operation.
    /// </summary>
    public void BeginCreate()
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginCreate();
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    /// <summary>
    /// Starts an asynchronous create operation.
    /// </summary>
    /// <param name="criteria">
    /// Criteria object provided to the
    /// DataPortal_Create() method.
    /// </param>
    public void BeginCreate(object criteria)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginCreate(criteria);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    /// <summary>
    /// Starts an asynchronous create operation.
    /// </summary>
    /// <param name="criteria">
    /// Criteria object provided to the
    /// DataPortal_Create() method.
    /// </param>
    /// <param name="userState">User state object.</param>
    public void BeginCreate(object criteria, object userState)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginCreate(criteria, userState);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }

    }

    private void proxy_CreateCompleted(object sender, DataPortalResult<T> e)
    {
      OnCreateCompleted(e);
    }

    #endregion

    #region Fetch

    /// <summary>
    /// Event raised when an asynchronous fetch method
    /// completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    /// <summary>
    /// Raises the FetchCompleted event
    /// </summary>
    /// <param name="e">
    /// Object containing the results of the data portal call.
    /// </param>
    protected virtual void OnFetchCompleted(DataPortalResult<T> e)
    {
      if (FetchCompleted != null)
        FetchCompleted(this, e);
    }

    /// <summary>
    /// Starts an asynchronous fetch operation.
    /// </summary>
    public void BeginFetch()
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginFetch();
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    /// <summary>
    /// Starts an asynchronous fetch operation.
    /// </summary>
    /// <param name="criteria">
    /// Criteria object provided to the
    /// DataPortal_Fetch() method.
    /// </param>
    public void BeginFetch(object criteria)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginFetch(criteria);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    /// <summary>
    /// Starts an asynchronous fetch operation.
    /// </summary>
    /// <param name="criteria">
    /// Criteria object provided to the
    /// DataPortal_Fetch() method.
    /// </param>
    /// <param name="userState">User state object.</param>
    public void BeginFetch(object criteria, object userState)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginFetch(criteria, userState);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    private void proxy_FetchCompleted(object sender, DataPortalResult<T> e)
    {
      OnFetchCompleted(e);
    }

    #endregion

    #region Update

    /// <summary>
    /// Event raised when an asynchronous update method
    /// completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    /// <summary>
    /// Raises the UpdateCompleted event
    /// </summary>
    /// <param name="e">
    /// Object containing the results of the data portal call.
    /// </param>
    protected virtual void OnUpdateCompleted(DataPortalResult<T> e)
    {
      if (UpdateCompleted != null)
        UpdateCompleted(this, e);
    }

    /// <summary>
    /// Starts an asynchronous update operation.
    /// </summary>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    public void BeginUpdate(object obj)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginUpdate(obj);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    /// <summary>
    /// Starts an asynchronous update operation.
    /// </summary>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="userState">User state object.</param>
    public void BeginUpdate(object obj, object userState)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginUpdate(obj, userState);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    private void proxy_UpdateCompleted(object sender, DataPortalResult<T> e)
    {
      OnUpdateCompleted(e);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Event raised when an asynchronous delete method
    /// completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    /// <summary>
    /// Raises the DeleteCompleted event
    /// </summary>
    /// <param name="e">
    /// Object containing the results of the data portal call.
    /// </param>
    protected virtual void OnDeleteCompleted(DataPortalResult<T> e)
    {
      if (DeleteCompleted != null)
        DeleteCompleted(this, e);
    }

    /// <summary>
    /// Starts an asynchronous delete operation.
    /// </summary>
    /// <param name="criteria">
    /// Criteria object provided to the
    /// DataPortal_Delete() method.
    /// </param>
    public void BeginDelete(object criteria)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginDelete(criteria);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    /// <summary>
    /// Starts an asynchronous delete operation.
    /// </summary>
    /// <param name="criteria">
    /// Criteria object provided to the
    /// DataPortal_Delete() method.
    /// </param>
    /// <param name="userState">User state object.</param>
    public void BeginDelete(object criteria, object userState)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginDelete(criteria, userState);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    private void proxy_DeleteCompleted(object sender, DataPortalResult<T> e)
    {
      OnDeleteCompleted(e);
    }

    #endregion

    #region Execute

    /// <summary>
    /// Event raised when an execute operation completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    /// <summary>
    /// Starts an asynchronous execute operation.
    /// </summary>
    /// <param name="command">
    /// Object to execute
    /// </param>
    public void BeginExecute(T command)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginExecute(command);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    /// <summary>
    /// Starts an asynchronous execute operation.
    /// </summary>
    /// <param name="command">
    /// Object to execute
    /// </param>
    /// <param name="userState">User state object.</param>
    public void BeginExecute(T command, object userState)
    {
      try
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);
        _proxy.BeginExecute(command, userState);
      }
      finally
      {
        ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client);
      }
    }

    private void proxy_ExecuteCompleted(object sender, DataPortalResult<T> e)
    {
      OnExecuteCompleted(e);
    }

    /// <summary>
    /// Raises the ExecuteCompleted event.
    /// </summary>
    /// <param name="e">Event args</param>
    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, e);
    }

    #endregion
  }
}