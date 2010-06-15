//-----------------------------------------------------------------------
// <copyright file="FactoryProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Data portal proxy class that uses the </summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization.Mobile;
using Csla.Reflection;
using Csla.Server;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Data portal proxy class that uses the 
  /// object factory model.
  /// </summary>
  /// <typeparam name="T"></typeparam>
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  public class FactoryProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
    #region Constructor
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="factoryInfo">ObjectFactory attribute</param>
    public FactoryProxy(ObjectFactoryAttribute factoryInfo)
    {
      _attribute = factoryInfo;
    }
    #endregion

    #region Events and Fields
    /// <summary>
    /// Defines the method signature for the completed handler.
    /// </summary>
    /// <param name="result">Result of operation</param>
    /// <param name="ex">Exception from operation</param>
    public delegate void CompletedHandler(T result, Exception ex);
    private object _userState;
    private Csla.Server.ObjectFactoryAttribute _attribute;
    #endregion

    #region GlobalContext

    /// <summary>
    /// Gets the global context value returned from an
    /// async operation.
    /// </summary>
    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return Csla.ApplicationContext.GlobalContext; }
    }

    #endregion

    #region Create

    /// <summary>
    /// Raised when create has completed.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    /// <summary>
    /// Starts a create operation.
    /// </summary>
    public void BeginCreate()
    {
      _userState = null;
      var obj = Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(_attribute.FactoryTypeName));
      var handler = new CompletedHandler(OnCreateCompleted);
      MethodCaller.CallMethod(obj, _attribute.CreateMethodName, handler);
    }
    
    /// <summary>
    /// Starts a create operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    public void BeginCreate(object criteria)
    {
      _userState = null;
      BeginCreate(criteria, null);
    }

    /// <summary>
    /// Starts a create operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">Userstate object</param>
    public void BeginCreate(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(_attribute.FactoryTypeName));
      var handler = new CompletedHandler(OnCreateCompleted);
      MethodCaller.CallMethod(obj, _attribute.CreateMethodName, criteria, handler);
    }

    private void OnCreateCompleted(T result, Exception ex)
    {
      if (CreateCompleted != null)
        CreateCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    #endregion

    #region Fetch

    /// <summary>
    /// Raised when fetch operation completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    public void BeginFetch()
    {
      _userState = null;
      var obj = Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(_attribute.FactoryTypeName));
      var handler = new CompletedHandler(OnFetchCompleted);
      MethodCaller.CallMethod(obj, _attribute.FetchMethodName, handler);
    }

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">Userstate object</param>
    public void BeginFetch(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(_attribute.FactoryTypeName));
      var handler = new CompletedHandler(OnFetchCompleted);
      MethodCaller.CallMethod(obj, _attribute.FetchMethodName, criteria, handler);
    }

    private void OnFetchCompleted(T result, Exception ex)
    {
      if (FetchCompleted != null)
        FetchCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    #endregion

    #region Update

    /// <summary>
    /// Raised when an update completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    /// <summary>
    /// Starts an update operation.
    /// </summary>
    /// <param name="obj">Object to update</param>
    public void BeginUpdate(object obj)
    {
      BeginUpdate(obj, null);
    }

    /// <summary>
    /// Starts an update operation.
    /// </summary>
    /// <param name="obj">Object to update</param>
    /// <param name="userState">Userstate object</param>
    public void BeginUpdate(object obj, object userState)
    {
      var factory = Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(_attribute.FactoryTypeName));
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
            MethodCaller.CallMethod(factory, _attribute.UpdateMethodName, obj,  handler);
          else
            handler((T)obj, null);
        }
        else
        {
          MethodCaller.CallMethod(factory, _attribute.UpdateMethodName, obj, handler);
        }
      }
      else
      {
        MethodCaller.CallMethod(factory, _attribute.UpdateMethodName, obj, handler);
      }
    }

    private void OnUpdateCompleted(T result, Exception ex)
    {
      if (UpdateCompleted != null)
        UpdateCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    #endregion

    #region Delete

    /// <summary>
    /// Raised when a delete completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    private void OnDeleteCompleted(T result, Exception ex)
    {
      if (DeleteCompleted != null)
        DeleteCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    /// <summary>
    /// Starts a delete operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }

    /// <summary>
    /// Starts a delete operation.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">Userstate object</param>
    public void BeginDelete(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance(Csla.Reflection.MethodCaller.GetType(_attribute.FactoryTypeName));
      var handler = new CompletedHandler(OnDeleteCompleted);
      MethodCaller.CallMethod(obj, _attribute.DeleteMethodName, criteria, handler);
    }

    #endregion

    #region Execute

    /// <summary>
    /// Raised when an execute completes.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    /// <summary>
    /// Raises the ExecuteCompleted event
    /// </summary>
    /// <param name="e">Event args</param>
    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, e);
    }

    /// <summary>
    /// Starts an execute operation.
    /// </summary>
    /// <param name="command">Object to execute</param>
    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

    /// <summary>
    /// Starts an execute operation.
    /// </summary>
    /// <param name="command">Object to execute</param>
    /// <param name="userState">Userstate object</param>
    public void BeginExecute(T command, object userState)
    {
      _userState = userState;
      var handler = new CompletedHandler(OnExecuteCompleted);
      MethodCaller.CallMethod(command, "DataPortal_Execute", handler);
    }

    private void OnExecuteCompleted(T result, Exception ex)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, new DataPortalResult<T>(result, ex, _userState));
    }

    #endregion
  }
}