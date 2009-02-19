using System;
using System.ServiceModel;
using Csla.Serialization.Mobile;
using Csla.Reflection;
using System.Diagnostics;

namespace Csla.DataPortalClient
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  public class DesignTimeProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {

    #region IDataPortalProxy<T> Members

    public void BeginCreate()
    {
      var obj = Activator.CreateInstance<T>();
      MethodCaller.CallMethodIfImplemented(obj, "DesignTime_Create");
      if (CreateCompleted != null)
        CreateCompleted(this, new DataPortalResult<T>(obj, null, null));
    }

    public void BeginCreate(object criteria)
    {
      BeginCreate();
    }

    public void BeginCreate(object criteria, object userState)
    {
      BeginCreate();
    }

    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    public void BeginFetch()
    {
      BeginFetch(null, null);
    }

    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }

    public void BeginFetch(object criteria, object userState)
    {
      var obj = Activator.CreateInstance<T>();
      MethodCaller.CallMethodIfImplemented(obj, "DesignTime_Create");
      if (FetchCompleted != null)
        FetchCompleted(this, new DataPortalResult<T>(obj, null, null));
    }

    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    public void BeginUpdate(object obj)
    {
      BeginUpdate(obj, null);
    }

    public void BeginUpdate(object obj, object userState)
    {
      T data = default(T);
      if (UpdateCompleted != null)
        UpdateCompleted(null, new DataPortalResult<T>(data, new NotImplementedException(), null));
    }

    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }

    public void BeginDelete(object criteria, object userState)
    {
      T data = default(T);
      if (DeleteCompleted != null)
        DeleteCompleted(null, new DataPortalResult<T>(data, new NotImplementedException(), null));
    }

    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

    public void BeginExecute(T command, object userState)
    {
      T data = default(T);
      if (ExecuteCompleted != null)
        ExecuteCompleted(null, new DataPortalResult<T>(data, new NotImplementedException(), null));
    }

    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return Csla.ApplicationContext.GlobalContext; }
    }

    #endregion
  }
}
