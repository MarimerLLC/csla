using System;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Interface implemented by data portal proxy
  /// components.
  /// </summary>
  public interface IDataPortalProxy<T> where T:Csla.Serialization.Mobile.IMobileObject
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    void BeginCreate();
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    void BeginCreate(object criteria);
    /// <summary>
    /// Event raised when the create operation is
    /// complete.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">Userstate object</param>
    void BeginCreate(object criteria, object userState);
    /// <summary>
    /// Raised when a create operation is complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> CreateCompleted;
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    void BeginFetch();
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    void BeginFetch(object criteria);
    /// <summary>
    /// Event raised when the fetch operation is
    /// complete.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">Userstate object</param>
    void BeginFetch(object criteria, object userState);
    /// <summary>
    /// Raised when a fetch operation is complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> FetchCompleted;
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Object to update</param>
    void BeginUpdate(object obj);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Object to update</param>
    /// <param name="userState">Userstate object</param>
    void BeginUpdate(object obj, object userState);
    /// <summary>
    /// Event raised when the update operation is
    /// complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> UpdateCompleted;
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    void BeginDelete(object criteria);
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="criteria">Criteria object</param>
    /// <param name="userState">Userstate object</param>
    void BeginDelete(object criteria, object userState);
    /// <summary>
    /// Event raised when the delete operation is
    /// complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> DeleteCompleted;
    /// <summary>
    /// Execute a command
    /// </summary>
    /// <param name="command">Command to execute</param>
    void BeginExecute(T command);
    /// <summary>
    /// Execute a command
    /// </summary>
    /// <param name="command">Command to execute</param>
    /// <param name="userState">Userstate object</param>
    void BeginExecute(T command, object userState);
    /// <summary>
    /// Raised when an execute operation is complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> ExecuteCompleted;
    /// <summary>
    /// Gets the global context returned by the async
    /// operation.
    /// </summary>
    Csla.Core.ContextDictionary GlobalContext {get;}
  }
}
