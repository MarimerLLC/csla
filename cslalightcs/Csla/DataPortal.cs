using System;
using System.ServiceModel;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.DataPortalClient;

namespace Csla
{
  public static class DataPortal
  {
    private static string _proxyTypeName;

    public static string ProxyTypeName
    {
      get
      {
        if (string.IsNullOrEmpty(_proxyTypeName))
          _proxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
        return _proxyTypeName;
      }
      set { _proxyTypeName = value; }
    }

    private static DataPortalClient.ProxyFactory _factory;

    public static DataPortalClient.ProxyFactory ProxyFactory
    {
      get
      {
        if (_factory == null)
          _factory = new Csla.DataPortalClient.ProxyFactory();
        return _factory;
      }
      set
      {
        _factory = value;
      }
    }

    #region Static Helpers
    
    #region Begin Create

    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T: IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate(criteria);
    }

    #endregion

    #region Begin Fetch

    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += callback;
      dp.BeginFetch();
    }

    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(criteria);
    } 

    #endregion

    #region Begin Update

    public static void BeginUpdate<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.UpdateCompleted += callback;
      dp.BeginUpdate(criteria);
    }

    #endregion

    #region Begin Delete

    public static void BeginDelete<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.UpdateCompleted += callback;
      dp.BeginDelete(criteria);
    }

    #endregion

    #endregion
  }

#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  public class DataPortal<T> : DataPortalClient.IDataPortalProxy<T> where T : IMobileObject
  {
    private DataPortalClient.IDataPortalProxy<T> _proxy;

    public DataPortal()
    {
      _proxy = DataPortal.ProxyFactory.GetProxy<T>();
      HookEvents(_proxy);
    }

    private void HookEvents(DataPortalClient.IDataPortalProxy<T> proxy)
    {
      _proxy.CreateCompleted += new EventHandler<DataPortalResult<T>>(proxy_CreateCompleted);
      _proxy.FetchCompleted += new EventHandler<DataPortalResult<T>>(proxy_FetchCompleted);
      _proxy.UpdateCompleted += new EventHandler<DataPortalResult<T>>(proxy_UpdateCompleted);
      _proxy.DeleteCompleted += new EventHandler<DataPortalResult<T>>(proxy_DeleteCompleted);
    }

    #region Create

    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    protected virtual void OnCreateCompleted(DataPortalResult<T> e)
    {
      if (CreateCompleted != null)
        CreateCompleted(this, e);
    }

    public void BeginCreate()
    {
      _proxy.BeginCreate();
    }

    public void BeginCreate(object criteria)
    {
      _proxy.BeginCreate(criteria);
    }

    private void proxy_CreateCompleted(object sender, DataPortalResult<T> e)
    {
      OnCreateCompleted(e);
    }

    #endregion

    #region Fetch

    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    protected virtual void OnFetchCompleted(DataPortalResult<T> e)
    {
      if (FetchCompleted != null)
        FetchCompleted(this, e);
    }

    public void BeginFetch()
    {
      _proxy.BeginFetch();
    }

    public void BeginFetch(object criteria)
    {
      _proxy.BeginFetch(criteria);
    }

    private void proxy_FetchCompleted(object sender, DataPortalResult<T> e)
    {
      OnFetchCompleted(e);
    }

    #endregion

    #region Update

    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    protected virtual void OnUpdateCompleted(DataPortalResult<T> e)
    {
      if (UpdateCompleted != null)
        UpdateCompleted(this, e);
    }

    public void BeginUpdate(object obj)
    {
      _proxy.BeginUpdate(obj);
    }

    private void proxy_UpdateCompleted(object sender, DataPortalResult<T> e)
    {
      OnUpdateCompleted(e);
    }

    #endregion

    #region Delete

    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    protected virtual void OnDeleteCompleted(DataPortalResult<T> e)
    {
      if (DeleteCompleted != null)
        DeleteCompleted(this, e);
    }

    public void BeginDelete(object criteria)
    {
      _proxy.BeginDelete(criteria);
    }

    private void proxy_DeleteCompleted(object sender, DataPortalResult<T> e)
    {
      OnDeleteCompleted(e);
    }

    #endregion
  }
}
