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
  public class SynchronizedWcfProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
    WcfProxy<T> _proxy = new WcfProxy<T>();
    Semaphore _semaphore;

    public SynchronizedWcfProxy()
    {
      _proxy.CreateCompleted += (o, e) => OnCreateCompleted(this, e);
      _proxy.ExecuteCompleted += (o, e) => OnExecuteCompleted(this, e);
      _proxy.FetchCompleted += (o, e) => OnFetchCompleted(this, e);
      _proxy.UpdateCompleted += (o, e) => OnUpdateCompleted(this, e);
      _proxy.DeleteCompleted += (o, e) => OnDeleteCompleted(this, e);
    }

    #region IDataPortalProxy<T> Members

    public void BeginCreate()
    {
      RunSynchronized((Action)_proxy.BeginCreate);
    }

    public void BeginCreate(object criteria)
    {
      RunSynchronized((Action<object>)_proxy.BeginCreate, criteria);
    }

    public void BeginCreate(object criteria, object userState)
    {
      RunSynchronized((Action<object, object>)_proxy.BeginCreate, criteria, userState);
    }

    public void BeginFetch()
    {
      RunSynchronized((Action)_proxy.BeginFetch);
    }

    public void BeginFetch(object criteria)
    {
      RunSynchronized((Action<object>)_proxy.BeginFetch, criteria);
    }

    public void BeginFetch(object criteria, object userState)
    {
      RunSynchronized((Action<object, object>)_proxy.BeginFetch, criteria, userState);
    }

    public void BeginUpdate(object criteria)
    {
      RunSynchronized((Action<object>)_proxy.BeginUpdate, criteria);
    }

    public void BeginUpdate(object criteria, object userState)
    {
      RunSynchronized((Action<object, object>)_proxy.BeginUpdate, criteria, userState);
    }

    public void BeginDelete(object criteria)
    {
      RunSynchronized((Action<object>)_proxy.BeginDelete, criteria);
    }

    public void BeginDelete(object criteria, object userState)
    {
      RunSynchronized((Action<object, object>)_proxy.BeginDelete, criteria, userState);
    }

    public void BeginExecute(T command)
    {
      RunSynchronized((Action<T>)_proxy.BeginExecute, command);
    }

    public void BeginExecute(T command, object userState)
    {
      RunSynchronized((Action<T, object>)_proxy.BeginExecute, command, userState);
    }

    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return _proxy.GlobalContext; }
    }

    private void OnCompleted(object sender, DataPortalResult<T> e, EventHandler<DataPortalResult<T>> method)
    {
      try
      {
        method(sender, e);
      }        
      finally
      {
        _semaphore.Dispose();
      }
    }

    public event EventHandler<DataPortalResult<T>> CreateCompleted;
    protected void OnCreateCompleted(object sender, DataPortalResult<T> e)
    {
      if (CreateCompleted != null)
        OnCompleted(sender, e, CreateCompleted);
    }

    public event EventHandler<DataPortalResult<T>> FetchCompleted;
    protected void OnFetchCompleted(object sender, DataPortalResult<T> e)
    {
      if (FetchCompleted != null)
        OnCompleted(sender, e, FetchCompleted);
    }

    public event EventHandler<DataPortalResult<T>> UpdateCompleted;
    protected void OnUpdateCompleted(object sender, DataPortalResult<T> e)
    {
      if (UpdateCompleted != null)
        OnCompleted(sender, e, UpdateCompleted);
    }

    public event EventHandler<DataPortalResult<T>> DeleteCompleted;
    protected void OnDeleteCompleted(object sender, DataPortalResult<T> e)
    {
      if (DeleteCompleted != null)
        OnCompleted(sender, e, DeleteCompleted);
    }

    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;
    protected void OnExecuteCompleted(object sender, DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        OnCompleted(sender, e, ExecuteCompleted);
    }

    #endregion

    private void RunSynchronized(Delegate method, params object[] arguments)
    {
      BackgroundWorker worker = new BackgroundWorker();
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
