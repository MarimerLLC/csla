//-----------------------------------------------------------------------
// <copyright file="LocalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Data portal proxy used to execute data</summary>
//-----------------------------------------------------------------------
using System;
using System.ServiceModel;
using Csla.Serialization.Mobile;
using Csla.Reflection;
using System.Diagnostics;
using Csla.Server;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Data portal proxy used to execute data
  /// portal operations on the Silverlight client.
  /// </summary>
  /// <typeparam name="T"></typeparam>
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  public class LocalProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
    #region Events and Fields

    /// <summary>
    /// Delegate defining the method
    /// signature for the callback method
    /// invoked when a data portal operation
    /// is complete.
    /// </summary>
    /// <param name="result">Result of the operation.</param>
    /// <param name="ex">Exception that occurred during the operation.</param>
    public delegate void CompletedHandler(T result, Exception ex);

    private object _userState;

    #endregion

    #region GlobalContext

    /// <summary>
    /// Gets the global context dictionary resulting
    /// from the asynchronous data portal operation.
    /// </summary>
    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return Csla.ApplicationContext.GlobalContext; }
    }

    #endregion

    #region Create

    /// <summary>
    /// Event raised when the create
    /// operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to create a business object.
    /// </summary>
    public void BeginCreate()
    {
      _userState = null;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnCreateCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Create", handler);
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to create a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginCreate(object criteria)
    {
      _userState = null;
      BeginCreate(criteria, null);
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to create a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginCreate(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnCreateCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Create", criteria, handler);
    }

    private void OnCreateCompleted(T result, Exception ex)
    {
      if (result != null)
      {
        var target = result as IDataPortalTarget;
        if (target != null)
          target.MarkNew();
      }
      if (CreateCompleted != null)
        CreateCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    #endregion

    #region Fetch

    /// <summary>
    /// Event raised when the fetch
    /// operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to retrieve a business object.
    /// </summary>
    public void BeginFetch()
    {
      _userState = null;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnFetchCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Fetch", handler);
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to retrieve a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to retrieve a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginFetch(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnFetchCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Fetch", criteria, handler);
    }

    private void OnFetchCompleted(T result, Exception ex)
    {
      if (result != null)
      {
        var target = result as IDataPortalTarget;
        if (target != null)
          target.MarkOld();
      }
      if (FetchCompleted != null)
        FetchCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    #endregion

    #region Update

    /// <summary>
    /// Event raised when the update
    /// operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    public void BeginUpdate(object obj)
    {
      BeginUpdate(obj, null);
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="userState">User state object.</param>
    public void BeginUpdate(object obj, object userState)
    {
      var handler = new CompletedHandler(OnUpdateCompleted);
      _userState = userState;
      var cloneable = obj as ICloneable;
      if (cloneable != null)
        obj = cloneable.Clone();

      var busObj = obj as Core.BusinessBase;
      if (busObj != null)
      {
        if (busObj.IsDeleted)
        {
          if (!busObj.IsNew)
            MethodCaller.CallMethod(obj, "DataPortal_DeleteSelf", handler);
          else
            handler((T)obj, null);
        }
        else
        {
          if (busObj.IsNew)
            MethodCaller.CallMethod(obj, "DataPortal_Insert", handler);
          else
            MethodCaller.CallMethod(obj, "DataPortal_Update", handler);
        }
      }
      else
      {
        MethodCaller.CallMethod(obj, "DataPortal_Update", handler);
      }
    }

    private void OnUpdateCompleted(T result, Exception ex)
    {
      if (result != null)
      {
        var target = result as IDataPortalTarget;
        if (target != null)
        {
          var busObj = result as Core.BusinessBase;
          if (busObj != null)
          {
            if (busObj.IsDeleted)
              target.MarkNew();
            else
              target.MarkOld();
          }
          else
          {
            target.MarkOld();
          }
        }
      }
      if (UpdateCompleted != null)
        UpdateCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    #endregion

    #region Delete

    /// <summary>
    /// Event raised when the delete
    /// operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    private void OnDeleteCompleted(T result, Exception ex)
    {
      if (result != null)
      {
        var target = result as IDataPortalTarget;
        if (target != null)
          target.MarkNew();
      }
      if (DeleteCompleted != null)
        DeleteCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to delete a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to delete a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginDelete(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnDeleteCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Delete", criteria, handler);
    }

    #endregion

    #region Execute

    /// <summary>
    /// Event raised when the execute
    /// operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    /// <summary>
    /// Raises the ExecuteCompleted event.
    /// </summary>
    /// <param name="e">Event argument.</param>
    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, e);
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to execute a command object.
    /// </summary>
    /// <param name="command">Business object to execute.</param>
    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

    /// <summary>
    /// Begins an asynchronous data portal
    /// operation to execute a command object.
    /// </summary>
    /// <param name="command">Business object to execute.</param>
    /// <param name="userState">User state object.</param>
    public void BeginExecute(T command, object userState)
    {
      _userState = userState;
      var handler = new CompletedHandler(OnExecuteCompleted);
      MethodCaller.CallMethod(command, "DataPortal_Execute", handler);
    }

    private void OnExecuteCompleted(T result, Exception ex)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, new DataPortalResult<T>(result, ex,  _userState));
    }

    #endregion
  }
}