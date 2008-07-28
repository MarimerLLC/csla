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
  public static class DataPortal
  {
    /// <summary>
    /// Data portal proxy mode options.
    /// </summary>
    public enum ProxyModes
    {
      /// <summary>
      /// Allow the data portal to auto-detect
      /// the mode based on configuration.
      /// </summary>
      Auto,
      /// <summary>
      /// Force the data portal to only
      /// execute in local mode.
      /// </summary>
      LocalOnly
    }

    private static string _proxyTypeName;

    /// <summary>
    /// Gets or sets the assembly qualified type
    /// name of the proxy object to be loaded
    /// by the data portal. "Local" is a special
    /// value used to indicate that the data
    /// portal should run in local mode.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a reference to a ProxyFactory object
    /// that is used to create an instance of the data
    /// portal proxy object.
    /// </summary>
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

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria object passed to DataPortal_Create().
    /// </param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T: IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate(criteria);
    }

    #endregion

    #region Begin Fetch

    /// <summary>
    /// Retrieves an existing business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += callback;
      dp.BeginFetch();
    }

    /// <summary>
    /// Retrieves an existing business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria object passed to DataPortal_Fetch().
    /// </param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(criteria);
    } 

    #endregion

    #region Begin Update

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginUpdate<T>(object obj, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.UpdateCompleted += callback;
      dp.BeginUpdate(obj);
    }

    #endregion

    #region Begin Delete

    /// <summary>
    /// Deletes an existing business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria object passed to DataPortal_Delete().
    /// </param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
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
  public class DataPortal<T> : DataPortalClient.IDataPortalProxy<T> where T : IMobileObject
  {
    private DataPortalClient.IDataPortalProxy<T> _proxy;

    /// <summary>
    /// Creates an instance of the data portal
    /// object, choosing a proxy object based
    /// on current configuration.
    /// </summary>
    public DataPortal() : this(DataPortal.ProxyModes.Auto)
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
      _proxy.BeginCreate();
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
      _proxy.BeginCreate(criteria);
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
      _proxy.BeginFetch();
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
      _proxy.BeginFetch(criteria);
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
      _proxy.BeginUpdate(obj);
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
      _proxy.BeginDelete(criteria);
    }

    private void proxy_DeleteCompleted(object sender, DataPortalResult<T> e)
    {
      OnDeleteCompleted(e);
    }

    #endregion
  }
}
