//-----------------------------------------------------------------------
// <copyright file="DataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the client-side DataPortal.</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using Csla.Reflection;
using Csla.Properties;
using Csla.Server;
using Csla.Serialization.Mobile;
using System.Threading.Tasks;

namespace Csla
{
  /// <summary>
  /// This is the client-side DataPortal.
  /// </summary>
  public static class DataPortal
  {
    /// <summary>
    /// Raised by DataPortal before it starts
    /// setting up to call a server-side
    /// DataPortal method.
    /// </summary>
    public static event Action<System.Object> DataPortalInitInvoke;

    /// <summary>
    /// Raised by DataPortal prior to calling the 
    /// requested server-side DataPortal method.
    /// </summary>
    public static event Action<DataPortalEventArgs> DataPortalInvoke;

    /// <summary>
    /// Raised by DataPortal after the requested 
    /// server-side DataPortal method call is complete.
    /// </summary>
    public static event Action<DataPortalEventArgs> DataPortalInvokeComplete;

    internal static void OnDataPortalInitInvoke(object e)
    {
      DataPortalInitInvoke?.Invoke(e);
    }

    internal static void OnDataPortalInvoke(DataPortalEventArgs e)
    {
      DataPortalInvoke?.Invoke(e);
    }

    internal static void OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      DataPortalInvokeComplete?.Invoke(e);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public static T Create<T>(params object[] criteria)
    {
      var dp = new DataPortal<T>();
      return dp.Create(criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <returns>A new object, populated with default values.</returns>
    public static T Create<T>()
    {
      return Create<T>(EmptyCriteria.Instance);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public static object Create(Type objectType, object criteria)
    {
      return DataPortal<object>.Create(objectType, criteria);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    [Obsolete]
    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginCreate<T>(EmptyCriteria.Instance, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginCreate<T>(EmptyCriteria.Instance, callback, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    [Obsolete]
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginCreate<T>(criteria, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate(criteria, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    public static async Task<T> CreateAsync<T>()
    {
      DataPortal<T> dp = new DataPortal<T>();
      return await dp.CreateAsync();
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    public static async Task<T> CreateAsync<T>(params object[] criteria)
    {
      DataPortal<T> dp = new DataPortal<T>();
      return await dp.CreateAsync(criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    public static T Fetch<T>(params object[] criteria)
    {
      var dp = new DataPortal<T>();
      return dp.Fetch(criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <returns>An object populated with values from the database.</returns>
    public static T Fetch<T>()
    {
      return Fetch<T>(EmptyCriteria.Instance);
    }

    internal static object Fetch(Type objectType, object criteria)
    {
      return DataPortal<object>.Fetch(objectType, criteria);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    [Obsolete]
    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginFetch<T>(EmptyCriteria.Instance, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginFetch<T>(EmptyCriteria.Instance, callback, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to fetch.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    [Obsolete]
    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginFetch<T>(criteria, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to fetch.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(criteria, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    public static async Task<T> FetchAsync<T>()
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      return await dp.FetchAsync();
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to fetch.
    /// </param>
    public static async Task<T> FetchAsync<T>(params object[] criteria)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      return await dp.FetchAsync(criteria);
    }

    /// <summary>
    /// Called by the business object's Save() method to
    /// insert, update or delete an object in the database.
    /// </summary>
    /// <remarks>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </remarks>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <param name="obj">A reference to the business object to be updated.</param>
    /// <returns>A reference to the updated business object.</returns>
    public static T Update<T>(T obj)
    {
      var dp = new DataPortal<T>();
      return dp.Update(obj);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// update a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to update.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    [Obsolete]
    public static void BeginUpdate<T>(T obj, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginUpdate<T>(obj, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// update a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to update.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public static void BeginUpdate<T>(T obj, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.UpdateCompleted += callback;
      dp.BeginUpdate(obj, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// update a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to update.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    public static async Task<T> UpdateAsync<T>(T obj)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      return await dp.UpdateAsync(obj);
    }

    /// <summary>
    /// Called by a Shared (static in C#) method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public static void Delete<T>(params object[] criteria)
    {
      var dp = new DataPortal<T>();
      dp.Delete(criteria);
    }

    internal static void Delete(Type objectType, object criteria)
    {
      DataPortal<object>.Delete(objectType, criteria);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// delete a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to delete.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to delete.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    [Obsolete]
    public static void BeginDelete<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginDelete<T>(criteria, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// delete a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to delete.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to delete.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public static void BeginDelete<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.DeleteCompleted += callback;
      dp.BeginDelete(criteria, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// delete a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to delete.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to delete.
    /// </param>
    public static async Task DeleteAsync<T>(params object[] criteria)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      await dp.DeleteAsync(criteria);
    }

    /// <summary>
    /// Called to execute a Command object on the server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To be a Command object, the object must inherit from
    /// CommandBase.
    /// </para><para>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </para><para>
    /// On the server, the Command object's DataPortal_Execute() method will
    /// be invoked and on an ObjectFactory the Execute method will be invoked. 
    /// Write any server-side code in that method. 
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Specific type of the Command object.</typeparam>
    /// <param name="obj">A reference to the Command object to be executed.</param>
    /// <returns>A reference to the updated Command object.</returns>
    public static T Execute<T>(T obj)
    {
      return Update(obj);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// execute a command object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object to execute.
    /// </typeparam>
    /// <param name="obj">
    /// Reference to the object to execute.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    [Obsolete]
    public static void BeginExecute<T>(T obj, EventHandler<DataPortalResult<T>> callback) 
      where T : IMobileObject
    {
      BeginExecute<T>(obj, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// execute a command object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object to execute.
    /// </typeparam>
    /// <param name="obj">
    /// Reference to the object to execute.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public static void BeginExecute<T>(T obj, EventHandler<DataPortalResult<T>> callback, object userState) 
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.ExecuteCompleted += callback;
      dp.BeginExecute(obj, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// execute a command object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object to execute.
    /// </typeparam>
    /// <param name="command">
    /// Reference to the object to execute.
    /// </param>
    public static async Task<T> ExecuteAsync<T>(T command) 
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      return await dp.ExecuteAsync(command);
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    public static T CreateChild<T>()
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return (T)(portal.Create(typeof(T)));
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
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return (T)(portal.Create(typeof(T), parameters));
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    public static async Task<T> CreateChildAsync<T>()
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return await portal.CreateAsync<T>();
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
    public static async Task<T> CreateChildAsync<T>(params object[] parameters)
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return await portal.CreateAsync<T>(parameters);
    }

    /// <summary>
    /// Creates and loads an existing
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to retrieve.
    /// </typeparam>
    public static T FetchChild<T>()
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return (T)(portal.Fetch(typeof(T)));
    }

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
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return (T)(portal.Fetch(typeof(T), parameters));
    }

    /// <summary>
    /// Fetchs and initializes a new
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to Fetch.
    /// </typeparam>
    public static async Task<T> FetchChildAsync<T>()
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return await portal.FetchAsync<T>();
    }

    /// <summary>
    /// Fetchs and initializes a new
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to Fetch.
    /// </typeparam>
    /// <param name="parameters">
    /// Parameters passed to child Fetch method.
    /// </param>
    public static async Task<T> FetchChildAsync<T>(params object[] parameters)
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return await portal.FetchAsync<T>(parameters);
    }

    /// <summary>
    /// Inserts, updates or deletes an existing
    /// child business object.
    /// </summary>
    /// <param name="child">
    /// Business object to update.
    /// </param>
    public static void UpdateChild(object child)
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      portal.Update(child);
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
    public static void UpdateChild(object child, params object[] parameters)
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      portal.Update(child, parameters);
    }

    /// <summary>
    /// Inserts, updates or deletes an existing
    /// child business object.
    /// </summary>
    /// <param name="child">
    /// Business object to update.
    /// </param>
    public static async Task UpdateChildAsync(object child)
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      await portal.UpdateAsync(child).ConfigureAwait(false);
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
    public static async Task UpdateChildAsync(object child, params object[] parameters)
    {
      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      await portal.UpdateAsync(child, parameters).ConfigureAwait(false);
    }

    private static DataPortalClient.IDataPortalProxyFactory _dataProxyFactory;

    /// <summary>
    /// Loads the data portal factory.
    /// </summary>
    internal static void LoadDataPortalProxyFactory()
    {
      if (_dataProxyFactory == null)
      {
        if (String.IsNullOrEmpty(ApplicationContext.DataPortalProxyFactory) || ApplicationContext.DataPortalProxyFactory == "Default")
        {
          _dataProxyFactory = new DataPortalClient.DataPortalProxyFactory();
        }
        else
        {
          var proxyFactoryType = 
            Type.GetType(ApplicationContext.DataPortalProxyFactory) ?? 
            throw new InvalidOperationException(
              string.Format(Resources.UnableToLoadDataPortalProxyFactory, ApplicationContext.DataPortalProxyFactory));

          _dataProxyFactory = (DataPortalClient.IDataPortalProxyFactory)MethodCaller.CreateInstance(proxyFactoryType);
        }
      }
    }

    /// <summary>
    /// Gets or sets a reference to a ProxyFactory object
    /// that is used to create an instance of the data
    /// portal proxy object.
    /// </summary>
    public static DataPortalClient.IDataPortalProxyFactory ProxyFactory
    {
      get
      {
        if (_dataProxyFactory == null)
          LoadDataPortalProxyFactory();
        return _dataProxyFactory;
      }
      set
      {
        _dataProxyFactory = value;
      }
    }

    /// <summary>
    /// Gets or sets the assembly qualified type
    /// name of the proxy object to be loaded
    /// by the data portal. "Local" is a special
    /// value used to indicate that the data
    /// portal should run in local mode.
    /// </summary>
    /// <remarks>
    /// Deprecated: use ApplicationContext.DataPortalProxy
    /// </remarks>
    public static string ProxyTypeName
    {
      get { return ApplicationContext.DataPortalProxy; }
      set { ApplicationContext.DataPortalProxy = value; }
    }

    /// <summary>
    /// Resets the data portal proxy type, so the
    /// next data portal call will reload the proxy
    /// type based on current configuration values.
    /// </summary>
    public static void ResetProxyFactory()
    {
      _dataProxyFactory = null;
    }

    /// <summary>
    /// Resets the data portal proxy type, so the
    /// next data portal call will reload the proxy
    /// type based on current configuration values.
    /// </summary>
    public static void ResetProxyType()
    {
      if (_dataProxyFactory != null)
      {
        _dataProxyFactory.ResetProxyType();
      }
    }

    /// <summary>
    /// Releases any remote data portal proxy object, so
    /// the next data portal call will create a new
    /// proxy instance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Proxies no longer cached")]
    public static void ReleaseProxy()
    { }
  }
}