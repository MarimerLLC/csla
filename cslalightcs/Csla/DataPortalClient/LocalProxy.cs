using System;
using System.ServiceModel;
using Csla.Serialization.Mobile;
using Csla.Reflection;

namespace Csla.DataPortalClient
{
  public class LocalProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
    public delegate void CompletedHandler(T result, Exception ex);

    #region Create

    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    public void BeginCreate()
    {
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnCreateCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Create", handler);
    }

    public void BeginCreate(object criteria)
    {
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
        CreateCompleted(this, new DataPortalResult<T>(result, ex));
    }

    #endregion

    #region Fetch

    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    public void BeginFetch()
    {
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnFetchCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Fetch", handler);
    }

    public void BeginFetch(object criteria)
    {
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
        FetchCompleted(this, new DataPortalResult<T>(result, ex));
    }

    #endregion

    #region Update

    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    public void BeginUpdate(object obj)
    {
      var handler = new CompletedHandler(OnUpdateCompleted);

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
        UpdateCompleted(this, new DataPortalResult<T>(result, ex));
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
        DeleteCompleted(this, new DataPortalResult<T>(result, ex));
    }

    public void BeginDelete(object criteria)
    {
      var obj = Activator.CreateInstance<T>();
      var handler = new CompletedHandler(OnDeleteCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Delete", handler, criteria);
    }

    #endregion
  }
}
