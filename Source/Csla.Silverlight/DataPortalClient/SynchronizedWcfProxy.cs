//-----------------------------------------------------------------------
// <copyright file="SynchronizedWcfProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a data portal proxy object that only</summary>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.Serialization.Mobile;
using Csla.Threading;
using System.ComponentModel;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy object that only
  /// allows one call at a time.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SynchronizedWcfProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
    WcfProxy<T> _proxy = new WcfProxy<T>();
    Semaphore _semaphore;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public SynchronizedWcfProxy()
    {
      _proxy.CreateCompleted += (o, e) => OnCreateCompleted(this, e);
      _proxy.ExecuteCompleted += (o, e) => OnExecuteCompleted(this, e);
      _proxy.FetchCompleted += (o, e) => OnFetchCompleted(this, e);
      _proxy.UpdateCompleted += (o, e) => OnUpdateCompleted(this, e);
      _proxy.DeleteCompleted += (o, e) => OnDeleteCompleted(this, e);
    }

    #region IDataPortalProxy<T> Members

    /// <summary>
    /// Starts a create operation.
    /// </summary>
    public void BeginCreate()
    {
      RunSynchronized((Action)_proxy.BeginCreate);
    }

    /// <summary>
    /// Starts a create operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    public void BeginCreate(object criteria)
    {
      RunSynchronized((Action<object>)_proxy.BeginCreate, criteria);
    }

    /// <summary>
    /// Starts a create operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">User state object</param>
    public void BeginCreate(object criteria, object userState)
    {
      RunSynchronized((Action<object, object>)_proxy.BeginCreate, criteria, userState);
    }

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    public void BeginFetch()
    {
      RunSynchronized((Action)_proxy.BeginFetch);
    }

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    public void BeginFetch(object criteria)
    {
      RunSynchronized((Action<object>)_proxy.BeginFetch, criteria);
    }

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">User state object</param>
    public void BeginFetch(object criteria, object userState)
    {
      RunSynchronized((Action<object, object>)_proxy.BeginFetch, criteria, userState);
    }

    /// <summary>
    /// Starts an update operation.
    /// </summary>
    /// <param name="criteria">Object to update</param>
    public void BeginUpdate(object criteria)
    {
      RunSynchronized((Action<object>)_proxy.BeginUpdate, criteria);
    }

    /// <summary>
    /// Starts an update operation.
    /// </summary>
    /// <param name="criteria">Object to update</param>
    /// <param name="userState">User state object</param>
    public void BeginUpdate(object criteria, object userState)
    {
      RunSynchronized((Action<object, object>)_proxy.BeginUpdate, criteria, userState);
    }

    /// <summary>
    /// Starts a delete operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    public void BeginDelete(object criteria)
    {
      RunSynchronized((Action<object>)_proxy.BeginDelete, criteria);
    }

    /// <summary>
    /// Starts a delete operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">User state object</param>
    public void BeginDelete(object criteria, object userState)
    {
      RunSynchronized((Action<object, object>)_proxy.BeginDelete, criteria, userState);
    }

    /// <summary>
    /// Starts an execute operation.
    /// </summary>
    /// <param name="command">Object to execute</param>
    public void BeginExecute(T command)
    {
      RunSynchronized((Action<T>)_proxy.BeginExecute, command);
    }

    /// <summary>
    /// Starts an execute operation.
    /// </summary>
    /// <param name="command">Object to execute</param>
    /// <param name="userState">User state object</param>
    public void BeginExecute(T command, object userState)
    {
      RunSynchronized((Action<T, object>)_proxy.BeginExecute, command, userState);
    }

    /// <summary>
    /// Gets the global context returned by the async
    /// operation.
    /// </summary>
    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return _proxy.GlobalContext; }
    }

    private void OnCompleted(object sender, DataPortalResult<T> e, EventHandler<DataPortalResult<T>> method)
    {
      using(_semaphore)
        method(sender, e);
    }

    /// <summary>
    /// Raised when a create operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> CreateCompleted;
    /// <summary>
    /// Raise the CreateCompleted event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void OnCreateCompleted(object sender, DataPortalResult<T> e)
    {
      if (CreateCompleted != null)
        OnCompleted(sender, e, CreateCompleted);
    }

    /// <summary>
    /// Raised when a fetch operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> FetchCompleted;
    /// <summary>
    /// Raise the FetchCompleted event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void OnFetchCompleted(object sender, DataPortalResult<T> e)
    {
      if (FetchCompleted != null)
        OnCompleted(sender, e, FetchCompleted);
    }

    /// <summary>
    /// Raised when an update operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> UpdateCompleted;
    /// <summary>
    /// Raise the UpdateCompleted event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void OnUpdateCompleted(object sender, DataPortalResult<T> e)
    {
      if (UpdateCompleted != null)
        OnCompleted(sender, e, UpdateCompleted);
    }

    /// <summary>
    /// Raised when a delete operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> DeleteCompleted;
    /// <summary>
    /// Raise the DeleteCompleted event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void OnDeleteCompleted(object sender, DataPortalResult<T> e)
    {
      if (DeleteCompleted != null)
        OnCompleted(sender, e, DeleteCompleted);
    }

    /// <summary>
    /// Raised when an execute operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;
    /// <summary>
    /// Raise the ExecuteCompleted event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void OnExecuteCompleted(object sender, DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        OnCompleted(sender, e, ExecuteCompleted);
    }

    #endregion

    private void RunSynchronized(Delegate method, params object[] arguments)
    {
      System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
      worker.DoWork += (o, e) =>
      {
        _semaphore = new Semaphore();
      };
      worker.RunWorkerCompleted += (o, e) =>
      {
        method.DynamicInvoke(arguments);
      };
      worker.RunWorkerAsync();
    }
  }
}
