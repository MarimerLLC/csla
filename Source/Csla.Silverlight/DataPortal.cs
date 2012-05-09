//-----------------------------------------------------------------------
// <copyright file="DataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Creates, retrieves, updates or deletes a</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization.Mobile;
using Csla.DataPortalClient;
#if !WINDOWS_PHONE
using System.Threading.Tasks;
#endif

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
#if WINDOWS_PHONE
          _proxyTypeName = "Local";
#else
          _proxyTypeName = typeof(WcfProxy<>).AssemblyQualifiedName;
#endif
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
    /// <param name="userState">User state object</param>
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
    /// <param name="userState">User state object</param>
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate(criteria, userState);
    }

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="callback">Delegate reference to a method that is invoked
    /// when the async operation is complete.</param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback,  ProxyModes proxyMode)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>(proxyMode);
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="callback">Delegate reference to a method that is invoked
    /// when the async operation is complete.</param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="userState">User state object</param>
    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback, ProxyModes proxyMode, object userState)
      where T : IMobileObject
    {
      BeginCreate<T>(null, callback, proxyMode, userState);
    }

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="criteria">Criteria object passed to DataPortal_Create().</param>
    /// <param name="callback">Delegate reference to a method that is invoked
    /// when the async operation is complete.</param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback, ProxyModes proxyMode)
      where T : IMobileObject
    {
      BeginCreate<T>(criteria, callback, proxyMode, null);
    }

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="criteria">Criteria object passed to DataPortal_Create().</param>
    /// <param name="callback">Delegate reference to a method that is invoked
    /// when the async operation is complete.</param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="userState">User state object</param>
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback, ProxyModes proxyMode, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>(proxyMode);
      dp.CreateCompleted += callback;
      dp.BeginCreate(criteria, userState);
    }

    #if !WINDOWS_PHONE

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    public static Task<T> CreateAsync<T>()
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginCreate<T>((o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      });
      return tcs.Task;
    }

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static Task<T> CreateAsync<T>(ProxyModes proxyMode)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginCreate<T>((o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      }, proxyMode);
      return tcs.Task;
    }

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="criteria">Criteria object passed to DataPortal_Create().</param>
    public static Task<T> CreateAsync<T>(object criteria)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginCreate<T>(criteria, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      });
      return tcs.Task;
    }

    /// <summary>
    /// Creates and initializes a business object.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="criteria">Criteria object passed to DataPortal_Create().</param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static Task<T> CreateAsync<T>(object criteria, ProxyModes proxyMode)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginCreate<T>(criteria, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      }, proxyMode);
      return tcs.Task;
    }
#endif

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
    /// <param name="userState">User state object.</param>
    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginFetch<T>(callback, ProxyModes.Auto, userState);
    }

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
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="userState">User state object.</param>
    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback, ProxyModes proxyMode, object userState)
     where T : IMobileObject
    {
      BeginFetch<T>(null, callback, proxyMode, userState);
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
    /// <param name="userState">User state object.</param>
    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
     where T : IMobileObject
    {
      BeginFetch<T>(criteria, callback, ProxyModes.Auto, userState);
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
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="userState">User state object.</param>
    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback, ProxyModes proxyMode, object userState)
     where T : IMobileObject
    {
      var dp = new DataPortal<T>(proxyMode);
      dp.FetchCompleted += callback;
      dp.BeginFetch(criteria, userState);
    }

#if !WINDOWS_PHONE
    /// <summary>
    /// Retrieves an existing business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    public static Task<T> FetchAsync<T>()
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginFetch<T>((o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      });
      return tcs.Task;
    }

    /// <summary>
    /// Retrieves an existing business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static Task<T> FetchAsync<T>(ProxyModes proxyMode)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginFetch<T>((o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      }, proxyMode);
      return tcs.Task;
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
    public static Task<T> FetchAsync<T>(object criteria)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginFetch<T>(criteria, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      });
      return tcs.Task;
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
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static Task<T> FetchAsync<T>(object criteria, ProxyModes proxyMode)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginFetch<T>(criteria, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      }, proxyMode);
      return tcs.Task;
    }
#endif

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

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginUpdate<T>(object obj, ProxyModes proxyMode, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginUpdate<T>(obj, proxyMode, callback, null);
    }

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
    /// <param name="userState">User state object.</param>
    public static void BeginUpdate<T>(object obj, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginUpdate<T>(obj, ProxyModes.Auto, callback, userState);
    }

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginUpdate<T>(object obj, ProxyModes proxyMode, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>(proxyMode);
      dp.UpdateCompleted += callback;
      dp.BeginUpdate(obj, userState);
    }

#if !WINDOWS_PHONE
    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    public static Task<T> UpdateAsync<T>(object obj)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginUpdate<T>(obj, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      });
      return tcs.Task;
    }

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static Task<T> UpdateAsync<T>(object obj, ProxyModes proxyMode)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginUpdate<T>(obj, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      }, proxyMode);
      return tcs.Task;
    }
#endif

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
    /// <param name="userState">User state object.</param>
    public static void BeginDelete<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginDelete<T>(criteria, ProxyModes.Auto, callback, userState);
    }

    /// <summary>
    /// Deletes an existing business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria object passed to DataPortal_Delete().
    /// </param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginDelete<T>(object criteria, ProxyModes proxyMode, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>(proxyMode);
      dp.DeleteCompleted += callback;
      dp.BeginDelete(criteria, userState);
    }

#if !WINDOWS_PHONE
    /// <summary>
    /// Deletes a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria object passed to DataPortal_Delete().
    /// </param>
    public static Task<T> DeleteAsync<T>(object criteria)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginDelete<T>(criteria, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      });
      return tcs.Task;
    }

    /// <summary>
    /// Deletes a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria object passed to DataPortal_Delete().
    /// </param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static Task<T> DeleteAsync<T>(object criteria, ProxyModes proxyMode)
      where T : IMobileObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginDelete<T>(criteria, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      }, proxyMode);
      return tcs.Task;
    }
#endif

    #endregion

    #region Begin Execute

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="command">Object to execute.</param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginExecute<T>(T command, EventHandler<DataPortalResult<T>> callback)
      where T : Core.ICommandObject
    {
      BeginExecute<T>(command, callback, null);
    }

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="command">Object to execute.</param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    public static void BeginExecute<T>(T command, ProxyModes proxyMode, EventHandler<DataPortalResult<T>> callback)
      where T : Core.ICommandObject
    {
      BeginExecute<T>(command, proxyMode, callback, null);
    }

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="command">Object to execute.</param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginExecute<T>(T command, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : Core.ICommandObject
    {
      BeginExecute<T>(command, ProxyModes.Auto, callback, userState);
    }

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="command">Object to execute.</param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    /// <param name="callback">
    /// Delegate reference to a method that is invoked
    /// when the async operation is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginExecute<T>(T command, ProxyModes proxyMode, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : Core.ICommandObject
    {
      var dp = new DataPortal<T>(proxyMode);
      dp.ExecuteCompleted += callback;
      dp.BeginExecute(command, userState);
    }

#if !WINDOWS_PHONE
    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="command">Object to execute.</param>
    public static Task<T> ExecuteAsync<T>(T command)
      where T : Core.ICommandObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginExecute<T>(command, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      });
      return tcs.Task;
    }

    /// <summary>
    /// Updates a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object.
    /// </typeparam>
    /// <param name="command">Object to execute.</param>
    /// <param name="proxyMode">The proxy mode, Local or Auto.</param>
    public static Task<T> ExecuteAsync<T>(T command, ProxyModes proxyMode)
      where T : Core.ICommandObject
    {
      var tcs = new TaskCompletionSource<T>();
      BeginExecute<T>(command, (o, e) =>
      {
        if (e.Error != null)
          tcs.SetException(e.Error);
        else
          tcs.SetResult(e.Object);
      }, proxyMode);
      return tcs.Task;
    }
#endif

    #endregion

    #region Child

    /// <summary>
    /// Creates and loads an existing
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to retrieve.
    /// </typeparam>
    /// <param name="parameters">
    /// Parameters passed to child fetch method.
    /// </param>
    public static T FetchChild<T>(params object[] parameters)
    {
      ChildDataPortal<T> dp = new ChildDataPortal<T>();
      return (T)dp.Fetch(parameters);
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="parameters">
    /// Parameters passed to child create method.
    /// </param>
    public static T CreateChild<T>(params object[] parameters)
    {
      ChildDataPortal<T> dp = new ChildDataPortal<T>();
      return (T)dp.Create(parameters);
    }

    /// <summary>
    /// Inserts, updates or deletes an existing
    /// child business object.
    /// </summary>
    /// <param name="child">
    /// Business object to update.
    /// </param>
    /// <param name="parameters">
    /// Parameters passed to child update method.
    /// </param>
    public static void UpdateChild<T>(T child, params object[] parameters)
    {
      ChildDataPortal<T> dp = new ChildDataPortal<T>();
      dp.Update(child, parameters);
    }
    #endregion

    #endregion
  }
}