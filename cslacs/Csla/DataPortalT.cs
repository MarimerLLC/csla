using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla
{
  /// <summary>
  /// Client side data portal used for making asynchronous
  /// data portal calls in .NET.
  /// </summary>
  /// <typeparam name="T">
  /// Type of business object.
  /// </typeparam>
  public class DataPortal<T>
  {
    internal const int EmptyCriteria = 1;

    #region Data Portal Async Request

    private class DataPortalAsyncRequest
    {
      public object Argument { get; set; }
      public System.Security.Principal.IPrincipal Principal { get; set; }
      public Csla.Core.ContextDictionary ClientContext { get; set; }
      public Csla.Core.ContextDictionary GlobalContext { get; set; }
      public object UserState { get; set; }

      public DataPortalAsyncRequest(object argument, object userState)
      {
        this.Argument = argument;
        this.Principal = Csla.ApplicationContext.User;
        this.ClientContext = Csla.ApplicationContext.ClientContext;
        this.GlobalContext = Csla.ApplicationContext.GlobalContext;
        this.UserState = userState;
      }
    }

    private class DataPortalAsyncResult
    {
      public T Result { get; set; }
      public Csla.Core.ContextDictionary GlobalContext { get; set; }
      public object UserState { get; set; }

      public DataPortalAsyncResult(T result, Csla.Core.ContextDictionary globalContext, object userState)
      {
        this.Result = result;
        this.GlobalContext = globalContext;
        this.UserState = userState;
      }
    }

    #endregion

    #region Set Background Thread Context

    private void SetThreadContext(DataPortalAsyncRequest request)
    {
      Csla.ApplicationContext.User = request.Principal;
      Csla.ApplicationContext.SetContext(request.ClientContext, request.GlobalContext);
    }

    #endregion

    #region GlobalContext

    private Csla.Core.ContextDictionary _globalContext;

    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return _globalContext; }
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

    public void BeginCreate(object criteria, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Create_RunWorkerCompleted;
      bw.DoWork += Create_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(criteria, userState));
    }

    void Create_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      var result = e.Result as DataPortalAsyncResult;
      if (result != null)
      {
        _globalContext = result.GlobalContext;
        if (result.Result != null)
          OnCreateCompleted(new DataPortalResult<T>((T)result.Result, e.Error, result.UserState));
        else
          OnCreateCompleted(new DataPortalResult<T>(default(T), e.Error, result.UserState));
      }
      else
        OnCreateCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    void Create_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);

      object state = request.Argument;
      T result = default(T);
      if (state is int)
        result = (T)Csla.DataPortal.Create<T>();
      else
        result = (T)Csla.DataPortal.Create<T>(state);
      e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, request.UserState);
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

    public void BeginFetch(object criteria, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Fetch_RunWorkerCompleted;
      bw.DoWork += Fetch_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(criteria, userState));
    }

    private void Fetch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      var result = e.Result as DataPortalAsyncResult;
      if (result != null)
      {
        _globalContext = result.GlobalContext;
        if (result.Result != null)
          OnFetchCompleted(new DataPortalResult<T>((T)result.Result, e.Error, result.UserState));
        else
          OnFetchCompleted(new DataPortalResult<T>(default(T), e.Error, result.UserState));
      }
      else
        OnFetchCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    private void Fetch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);

      object state = request.Argument;
      T result = default(T);
      if (state is int)
        result = (T)Csla.DataPortal.Fetch<T>();
      else
        result = (T)Csla.DataPortal.Fetch<T>(state);
      e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, request.UserState);
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

    public void BeginUpdate(object obj, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Update_RunWorkerCompleted;
      bw.DoWork += Update_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(obj, userState));
    }

    void Update_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      var result = e.Result as DataPortalAsyncResult;
      if (result != null)
      {
        _globalContext = result.GlobalContext;
        if (result.Result != null)
          OnUpdateCompleted(new DataPortalResult<T>((T)result.Result, e.Error, result.UserState));
        else
          OnUpdateCompleted(new DataPortalResult<T>(default(T), e.Error, result.UserState));
      }
      else
        OnUpdateCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    void Update_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);

      object state = request.Argument;
      T result = default(T);
      result = (T)Csla.DataPortal.Update<T>((T)state);
      e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, request.UserState);
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

    public void BeginDelete(object criteria, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Delete_RunWorkerCompleted;
      bw.DoWork += Delete_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(criteria, userState));
    }

    private void Delete_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      var result = e.Result as DataPortalAsyncResult;
      if (result != null)
        _globalContext = result.GlobalContext;
      OnDeleteCompleted(new DataPortalResult<T>(default(T), e.Error, result.UserState));
    }

    private void Delete_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);

      object state = request.Argument;
      Csla.DataPortal.Delete<T>(state);
      e.Result = new DataPortalAsyncResult(default(T), Csla.ApplicationContext.GlobalContext, request.UserState);
    }

    #endregion

    #region Execute

    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

    public void BeginExecute(T command, object userState)
    {
      var bw = new System.ComponentModel.BackgroundWorker();
      bw.RunWorkerCompleted += Execute_RunWorkerCompleted;
      bw.DoWork += Execute_DoWork;
      bw.RunWorkerAsync(new DataPortalAsyncRequest(command, userState));
    }

    void Execute_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      var result = e.Result as DataPortalAsyncResult;
      if (result != null)
      {
        _globalContext = result.GlobalContext;
        if (result.Result != null)
          OnExecuteCompleted(new DataPortalResult<T>((T)result.Result, e.Error, result.UserState));
        else
          OnExecuteCompleted(new DataPortalResult<T>(default(T), e.Error, result.UserState));
      }
      else
        OnExecuteCompleted(new DataPortalResult<T>(default(T), e.Error, null));
    }

    void Execute_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) 
    {
      var request = e.Argument as DataPortalAsyncRequest;
      SetThreadContext(request);

      object state = request.Argument;
      T result = default(T);
      result = Csla.DataPortal.Execute<T>((T)state);
      e.Result = new DataPortalAsyncResult(result, Csla.ApplicationContext.GlobalContext, request.UserState);
    }

    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, e);
    }

    #endregion
  }
}
