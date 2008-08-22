using System;
using System.ServiceModel;
using Csla.Serialization.Mobile;
using Csla.Reflection;
using System.Diagnostics;

namespace Csla.DataPortalClient
{
  public class LocalProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
    #region Events and Fields
    public delegate void CompletedHandler(T result, Exception ex);
    private object _userState;
    #endregion

    #region GlobalContext

    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return Csla.ApplicationContext.GlobalContext; }
    }

    #endregion

    #region Create

    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    public void BeginCreate()
    {
      _userState = null;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnCreateCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Create", handler);
    }
    public void BeginCreate(object criteria)
    {
      _userState = null;
      BeginCreate(criteria, null);
    }
    public void BeginCreate(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnCreateCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Create", handler, criteria);
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

    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    public void BeginFetch()
    {
      _userState = null;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnFetchCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Fetch", handler);
    }

    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }

    public void BeginFetch(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnFetchCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Fetch", handler, criteria);
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

    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    public void BeginUpdate(object obj)
    {
      BeginUpdate(obj, null);
    }
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

    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }

    public void BeginDelete(object criteria, object userState)
    {
      _userState = userState;
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnDeleteCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Delete", handler, criteria);
    }

    #endregion

    #region Execute

    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, e);
    }

    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }
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
