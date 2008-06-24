using System;
using System.ServiceModel;
using Csla.Serialization.Mobile;
using Csla.Reflection;

namespace Csla.DataPortalClient
{
  public class LocalProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
    public delegate void CompletedHandler(DataPortalResult<T> e);

    #region Create

    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    protected virtual void OnCreateCompleted(DataPortalResult<T> e)
    {
      if (CreateCompleted != null)
        CreateCompleted(this, e);
    }

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

    //public void DataPortal_Create(CompletedHandler completed, SingleCriteria<Customer, int> criteria)
    //{
    //  completed(new DataPortalResult<T>(default(T), new Exception()));
    //}

    #endregion

    #region Fetch

    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    protected virtual void OnFetchCompleted(DataPortalResult<T> e)
    {
      if (FetchCompleted != null)
        FetchCompleted(this, e);
    }

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

    #endregion

    #region Update

    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    protected virtual void OnUpdateCompleted(DataPortalResult<T> e)
    {
      if (UpdateCompleted != null)
        UpdateCompleted(this, e);
    }

    public void BeginUpdate(object obj)
    {
      var handler = new CompletedHandler(OnUpdateCompleted);
      MethodCaller.CallMethod(obj, "DataPortal_Update", handler);
    }

    #endregion

    #region Delete

    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    protected virtual void OnDeleteCompleted(DataPortalResult<T> e)
    {
      if (DeleteCompleted != null)
        DeleteCompleted(this, e);
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
