//-----------------------------------------------------------------------
// <copyright file="DataPortalT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Client side data portal used for making asynchronous</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Csla
{
  /// <summary>
  /// Client side data portal used for making asynchronous
  /// data portal calls in .NET.
  /// </summary>
  /// <typeparam name="T">
  /// Type of business object.
  /// </typeparam>
  public class DataPortal<T> : IDataPortal<T>
  {
    internal static Csla.Server.EmptyCriteria EmptyCriteria = new Server.EmptyCriteria();
    private Csla.DataPortal.ProxyModes _proxyMode;

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
      _proxyMode = proxyMode;
    }

    /// <summary>
    /// Gets a reference to the global context returned from
    /// the background thread and/or server.
    /// </summary>
    public Csla.Core.ContextDictionary GlobalContext { get; set; }

    #region Data Portal Async Request

    private class DataPortalAsyncRequest
    {
      public object Argument { get; set; }
      public System.Security.Principal.IPrincipal Principal { get; set; }
      public Csla.Core.ContextDictionary ClientContext { get; set; }
      public Csla.Core.ContextDictionary GlobalContext { get; set; }
      public object UserState { get; set; }
      // passes CurrentCulture and CurrentUICulture to the async thread
#if NETFX_CORE
      public string CurrentCulture;
      public string CurrentUICulture;
#else
      public CultureInfo CurrentCulture;
      public CultureInfo CurrentUICulture;
#endif

      public DataPortalAsyncRequest(object argument, object userState)
      {
        this.Argument = argument;
        this.Principal = Csla.ApplicationContext.User;
        this.ClientContext = Csla.ApplicationContext.ClientContext;
        this.GlobalContext = Csla.ApplicationContext.GlobalContext;
        this.UserState = userState;
#if NETFX_CORE
        var language = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Languages[0];
        this.CurrentCulture = language;
        this.CurrentUICulture = language;
#else
        this.CurrentCulture = Thread.CurrentThread.CurrentCulture;
        this.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
#endif
      }
    }

    private class DataPortalAsyncResult
    {
      public T Result { get; set; }
      public Csla.Core.ContextDictionary GlobalContext { get; set; }
      public object UserState { get; set; }
      public Exception Error { get; set; }

      public DataPortalAsyncResult(T result, Csla.Core.ContextDictionary globalContext, Exception error, object userState)
      {
        this.Result = result;
        this.GlobalContext = globalContext;
        this.UserState = userState;
        this.Error = error;
      }
    }

    #endregion

    #region Set Background Thread Context

    private void SetThreadContext(DataPortalAsyncRequest request)
    {
      Csla.ApplicationContext.User = request.Principal;
      Csla.ApplicationContext.SetContext(request.ClientContext, request.GlobalContext);
      // set culture info for background thread 
#if NETFX_CORE
      var list = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { request.CurrentUICulture });
      Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Languages = list;
      list = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { request.CurrentCulture });
      Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Languages = list;
#else
      Thread.CurrentThread.CurrentCulture = request.CurrentCulture;
      Thread.CurrentThread.CurrentUICulture = request.CurrentUICulture;
#endif
    }

    #endregion

    #region Synchronous methods

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public T Create(object criteria)
    {
      return DataPortal.Create<T>(criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <returns>A new object, populated with default values.</returns>
    public T Create()
    {
      return DataPortal.Create<T>();
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public object Create(Type objectType, object criteria)
    {
      return DataPortal.Create(objectType, criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    public T Fetch(object criteria)
    {
      return DataPortal.Fetch<T>(criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <returns>An object populated with values from the database.</returns>
    public T Fetch()
    {
      return DataPortal.Fetch<T>();
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
    /// <param name="obj">A reference to the Command object to be executed.</param>
    /// <returns>A reference to the updated Command object.</returns>
    public T Execute(T obj)
    {
      return DataPortal.Update<T>(obj);
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
    /// <param name="obj">A reference to the business object to be updated.</param>
    /// <returns>A reference to the updated business object.</returns>
    public T Update(T obj)
    {
      return DataPortal.Update<T>(obj);
    }

    /// <summary>
    /// Called by a Shared (static in C#) method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void Delete(object criteria)
    {
      DataPortal.Delete<T>(criteria);
    }

    #endregion

    #region Create

    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="e">
    /// The parameter provided to the event handler.
    /// </param>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    protected virtual void OnCreateCompleted(DataPortalResult<T> e)
    {
      if (CreateCompleted != null)
        CreateCompleted(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    public void BeginCreate()
    {
      BeginCreate(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void BeginCreate(object criteria)
    {
      BeginCreate(criteria, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    public void BeginCreate(object criteria, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Create_RunWorkerCompleted;
      bw.DoWork += Create_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(criteria, userState));
    }

    void Create_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      if (e.Error == null)
      {
        var result = e.Result as DataPortalAsyncResult;
        if (result != null)
        {
          GlobalContext = result.GlobalContext;
          T obj = default(T);
          if (result.Result != null)
            obj = result.Result;
          OnCreateCompleted(new DataPortalResult<T>(obj, result.Error, result.UserState));
          return;
        }
      }
      OnCreateCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    void Create_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);
      T result = default(T);
      try
      {
        object state = request.Argument;
        if (state is Csla.Server.EmptyCriteria)
          result = Csla.DataPortal.Create<T>();
        else
          result = Csla.DataPortal.Create<T>(state);
        e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    public async Task<T> CreateAsync()
    {
      return await CreateAsync(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public async Task<T> CreateAsync(object criteria)
    {
      var request = new DataPortalAsyncRequest(criteria, null);
      var result = await Task.Factory.StartNew<DataPortalAsyncResult>(DoCreateAsync, request);
      GlobalContext = result.GlobalContext;
      if (result.Error != null)
        throw result.Error;
      return result.Result;
    }

    private DataPortalAsyncResult DoCreateAsync(object e)
    {
      DataPortalAsyncResult response = null;
      var request = e as DataPortalAsyncRequest;
      SetThreadContext(request);
      T result = default(T);
      try
      {
        object state = request.Argument;
        if (state is Csla.Server.EmptyCriteria)
          result = Csla.DataPortal.Create<T>();
        else
          result = Csla.DataPortal.Create<T>(state);
        response = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        response = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
      return response;
    }

    #endregion

    #region Fetch

    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="e">
    /// The parameter provided to the event handler.
    /// </param>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    protected virtual void OnFetchCompleted(DataPortalResult<T> e)
    {
      if (FetchCompleted != null)
        FetchCompleted(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to retrieve an existing object, which is loaded 
    /// with values from the database.
    /// </summary>
    public void BeginFetch()
    {
      BeginFetch(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to retrieve an existing object, which is loaded 
    /// with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to retrieve an existing object, which is loaded 
    /// with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    public void BeginFetch(object criteria, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Fetch_RunWorkerCompleted;
      bw.DoWork += Fetch_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(criteria, userState));
    }

    private void Fetch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      if (e.Error == null)
      {
        var result = e.Result as DataPortalAsyncResult;
        if (result != null)
        {
          GlobalContext = result.GlobalContext;
          T obj = default(T);
          if (result.Result != null)
            obj = result.Result;
          OnFetchCompleted(new DataPortalResult<T>(obj, result.Error, result.UserState));
          return;
        }
      }
      OnFetchCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    private void Fetch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);
      T result = default(T);
      try
      {
        object state = request.Argument;
        if (state is Csla.Server.EmptyCriteria)
          result = Csla.DataPortal.Fetch<T>();
        else
          result = Csla.DataPortal.Fetch<T>(state);
        e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to retrieve an existing object, which is loaded 
    /// with values from the database.
    /// </summary>
    public async Task<T> FetchAsync()
    {
      return await FetchAsync(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to retrieve an existing object, which is loaded 
    /// with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public async Task<T> FetchAsync(object criteria)
    {
      var request = new DataPortalAsyncRequest(criteria, null);
      var result = await Task.Factory.StartNew<DataPortalAsyncResult>(DoFetchAsync, request);
      if (result.Error != null)
        throw result.Error;
      GlobalContext = result.GlobalContext;
      return result.Result;
    }

    private DataPortalAsyncResult DoFetchAsync(object e)
    {
      DataPortalAsyncResult response = null;
      var request = e as DataPortalAsyncRequest;
      SetThreadContext(request);
      T result = default(T);
      try
      {
        object state = request.Argument;
        if (state is Csla.Server.EmptyCriteria)
          result = Csla.DataPortal.Fetch<T>();
        else
          result = Csla.DataPortal.Fetch<T>(state);
        response = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        response = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
      return response;
    }

    #endregion

    #region Update

    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="e">
    /// The parameter provided to the event handler.
    /// </param>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    protected virtual void OnUpdateCompleted(DataPortalResult<T> e)
    {
      if (UpdateCompleted != null)
        UpdateCompleted(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    public void BeginUpdate(object obj)
    {
      BeginUpdate(obj, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    /// <param name="userState">User state data.</param>
    public void BeginUpdate(object obj, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Update_RunWorkerCompleted;
      bw.DoWork += Update_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(obj, userState));
    }

    void Update_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      if (e.Error == null)
      {
        var result = e.Result as DataPortalAsyncResult;
        if (result != null)
        {
          GlobalContext = result.GlobalContext;
          T obj = default(T);
          if (result.Result != null)
            obj = result.Result;
          OnUpdateCompleted(new DataPortalResult<T>(obj, result.Error, result.UserState));
          return;
        }
      }
      OnUpdateCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    void Update_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);
      T result = default(T);
      try
      {
        object state = request.Argument;
        result = Csla.DataPortal.Update<T>((T)state);
        e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    public async Task<T> UpdateAsync(object obj)
    {
      var request = new DataPortalAsyncRequest(obj, null);
      var result = await Task.Factory.StartNew<DataPortalAsyncResult>(DoUpdateAsync, request);
      if (result.Error != null)
        throw result.Error;
      GlobalContext = result.GlobalContext;
      return result.Result;
    }

    private DataPortalAsyncResult DoUpdateAsync(object e)
    {
      DataPortalAsyncResult response = null;
      var request = e as DataPortalAsyncRequest;
      SetThreadContext(request);
      T result = default(T);
      try
      {
        result = Csla.DataPortal.Update<T>((T)request.Argument);
        response = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        response = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
      return response;
    }

    #endregion

    #region Delete

    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="e">
    /// The parameter provided to the event handler.
    /// </param>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    protected virtual void OnDeleteCompleted(DataPortalResult<T> e)
    {
      if (DeleteCompleted != null)
        DeleteCompleted(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    public void BeginDelete(object criteria, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Delete_RunWorkerCompleted;
      bw.DoWork += Delete_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(criteria, userState));
    }

    private void Delete_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      if (e.Error == null)
      {
        var result = e.Result as DataPortalAsyncResult;
        if (result != null)
        {
          GlobalContext = result.GlobalContext;
          OnDeleteCompleted(new DataPortalResult<T>(default(T), result.Error, result.UserState));
          return;
        }
      }
      OnDeleteCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    private void Delete_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);
      try
      {
        object state = request.Argument;
        Csla.DataPortal.Delete<T>(state);
        e.Result = new DataPortalAsyncResult(default(T), Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        e.Result = new DataPortalAsyncResult(default(T), Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public async Task<T> DeleteAsync(object criteria)
    {
      var request = new DataPortalAsyncRequest(criteria, null);
      var result = await Task.Factory.StartNew<DataPortalAsyncResult>(DoDeleteAsync, request);
      if (result.Error != null)
        throw result.Error;
      GlobalContext = result.GlobalContext;
      return result.Result;
    }

    private DataPortalAsyncResult DoDeleteAsync(object e)
    {
      DataPortalAsyncResult response = null;
      var request = e as DataPortalAsyncRequest;
      SetThreadContext(request);
      try
      {
        Csla.DataPortal.Delete<T>(request.Argument);
        response = new DataPortalAsyncResult(default(T), Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        response = new DataPortalAsyncResult(default(T), Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
      return response;
    }

    #endregion

    #region Execute

    /// <summary>
    /// Event indicating an execute operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    /// <param name="userState">User state data.</param>
    public void BeginExecute(T command, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Execute_RunWorkerCompleted;
      bw.DoWork += Execute_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(command, userState));
    }

    void Execute_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      if (e.Error == null)
      {
        var result = e.Result as DataPortalAsyncResult;
        if (result != null)
        {
          GlobalContext = result.GlobalContext;
          T obj = default(T);
          if (result.Result != null)
            obj = (T)result.Result;
          OnExecuteCompleted(new DataPortalResult<T>(obj, result.Error, result.UserState));
          return;
        }
      }
      OnExecuteCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    void Execute_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) 
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);
      T result = default(T);
      try
      {
        object state = request.Argument;
        result = Csla.DataPortal.Execute<T>((T)state);
        e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
    }

    /// <summary>
    /// Raises the ExecuteCompleted event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    public async Task<T> ExecuteAsync(object command)
    {
      var request = new DataPortalAsyncRequest(command, null);
      var result = await Task.Factory.StartNew<DataPortalAsyncResult>(DoExecuteAsync, request);
      if (result.Error != null)
        throw result.Error;
      GlobalContext = result.GlobalContext;
      return result.Result;
    }

    private DataPortalAsyncResult DoExecuteAsync(object e)
    {
      DataPortalAsyncResult response = null;
      var request = e as DataPortalAsyncRequest;
      SetThreadContext(request);
      T result = default(T);
      try
      {
        result = Csla.DataPortal.Execute<T>((T)request.Argument);
        response = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, null, request.UserState);
      }
      catch (Exception ex)
      {
        response = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, ex, request.UserState);
      }
      return response;
    }

    #endregion
  }
}