using System;
using System.ServiceModel;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.DataPortalClient;
using System.Windows;
using System.ComponentModel;

namespace Csla
{
  /// <summary>
  /// Creates, retrieves, updates or deletes a
  /// business object.
  /// </summary>
  public static class DataPortal
  {
    #region Config Settings

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
          _proxyTypeName = typeof(WcfProxy<>).AssemblyQualifiedName;
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

    public static bool IsInDesignMode
    {
      get
      {
        if (Application.Current != null && Application.Current.RootVisual != null)
        {
          return DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual);
        }
        else
          return false;
      }
    }

    #endregion

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
      var dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }

    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginCreate<T>(null, callback, userState);
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
      where T : IMobileObject
    {
      BeginCreate<T>(criteria, callback, null);
    }

    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate(criteria, userState);
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
      var dp = new DataPortal<T>();
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
      BeginFetch<T>(criteria, callback, null);
    }

    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginFetch<T>(null, callback, userState);
    }
    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
     where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(criteria, userState);
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
      BeginUpdate<T>(obj, callback, null);
    }

    public static void BeginUpdate<T>(object obj, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.UpdateCompleted += callback;
      dp.BeginUpdate(obj, userState);
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
      BeginDelete<T>(criteria, callback, null);
    }

    public static void BeginDelete<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.DeleteCompleted += callback;
      dp.BeginDelete(criteria, userState);
    }


    #endregion

    #region Begin Execute

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginExecute<T>(T command, EventHandler<DataPortalResult<T>> callback)
      where T : CommandBase
    {
      BeginExecute<T>(command, callback, null);
    }

    public static void BeginExecute<T>(T command, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : CommandBase
    {
      var dp = new DataPortal<T>();
      dp.ExecuteCompleted += callback;
      dp.BeginExecute(command, userState);
    }

    #endregion

    #region Child

    public static T FetchChild<T>(params object[] parameters)
    {
      ChildDataPortal<T> dp = new ChildDataPortal<T>();
      return (T)dp.Fetch(parameters);
    }

    public static T CreateChild<T>(params object[] parameters)
    {
      ChildDataPortal<T> dp = new ChildDataPortal<T>();
      return (T)dp.Create(parameters);
    }

    public static void UpdateChild<T>(T child, params object[] parameters)
    {
      ChildDataPortal<T> dp = new ChildDataPortal<T>();
      dp.Update(child, parameters);
    }
    #endregion

    #endregion
  }
}
