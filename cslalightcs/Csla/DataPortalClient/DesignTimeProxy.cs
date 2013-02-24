using System;
using System.ServiceModel;
using Csla.Serialization.Mobile;
using Csla.Reflection;
using System.Diagnostics;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Data portal proxy used for XAML design time
  /// sample data generation.
  /// </summary>
  /// <typeparam name="T">Type of business object.</typeparam>
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  public class DesignTimeProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {

    #region IDataPortalProxy<T> Members

    /// <summary>
    /// Starts a create operation.
    /// </summary>
    public void BeginCreate()
    {
      var obj = Activator.CreateInstance<T>();
      MethodCaller.CallMethodIfImplemented(obj, "DesignTime_Create");
      if (CreateCompleted != null)
        CreateCompleted(this, new DataPortalResult<T>(obj, null, null));
    }

    /// <summary>
    /// Starts a create operation.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginCreate(object criteria)
    {
      BeginCreate();
    }

    /// <summary>
    /// Starts a create operation.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginCreate(object criteria, object userState)
    {
      BeginCreate();
    }

    /// <summary>
    /// Event raised when async create operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    public void BeginFetch()
    {
      BeginFetch(null, null);
    }

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }

    /// <summary>
    /// Starts a fetch operation.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginFetch(object criteria, object userState)
    {
      var obj = Activator.CreateInstance<T>();
      MethodCaller.CallMethodIfImplemented(obj, "DesignTime_Create");
      if (FetchCompleted != null)
        FetchCompleted(this, new DataPortalResult<T>(obj, null, null));
    }

    /// <summary>
    /// Event raised when async fetch operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    /// <summary>
    /// Starts an update operation.
    /// </summary>
    /// <param name="obj">Object to be updated.</param>
    public void BeginUpdate(object obj)
    {
      BeginUpdate(obj, null);
    }

    /// <summary>
    /// Starts an update operation.
    /// </summary>
    /// <param name="obj">Object to be updated.</param>
    /// <param name="userState">User state object.</param>
    public void BeginUpdate(object obj, object userState)
    {
      T data = default(T);
      if (UpdateCompleted != null)
        UpdateCompleted(null, new DataPortalResult<T>(data, new NotImplementedException(), null));
    }

    /// <summary>
    /// Event raised when async update operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    /// <summary>
    /// Starts a delete operation.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }

    /// <summary>
    /// Starts a delete operation.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginDelete(object criteria, object userState)
    {
      T data = default(T);
      if (DeleteCompleted != null)
        DeleteCompleted(null, new DataPortalResult<T>(data, new NotImplementedException(), null));
    }

    /// <summary>
    /// Event raised when async delete operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    /// <summary>
    /// Starts an execute operation.
    /// </summary>
    /// <param name="command">Object to execute.</param>
    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

    /// <summary>
    /// Starts an execute operation.
    /// </summary>
    /// <param name="command">Object to execute.</param>
    /// <param name="userState">User state object.</param>
    public void BeginExecute(T command, object userState)
    {
      T data = default(T);
      if (ExecuteCompleted != null)
        ExecuteCompleted(null, new DataPortalResult<T>(data, new NotImplementedException(), null));
    }

    /// <summary>
    /// Event raised when async execute operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    /// <summary>
    /// Gets the global context value returned by the
    /// async operation.
    /// </summary>
    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return Csla.ApplicationContext.GlobalContext; }
    }

    #endregion
  }
}
