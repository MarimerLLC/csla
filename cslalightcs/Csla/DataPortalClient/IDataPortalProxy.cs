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
    void BeginCreate(object criteria);
    /// <summary>
    /// Event raised when the create operation is
    /// complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> CreateCompleted;
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    void BeginFetch();
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    void BeginFetch(object criteria);
    /// <summary>
    /// Event raised when the fetch operation is
    /// complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> FetchCompleted;
    /// <summary>
    /// Update a business object.
    /// </summary>
    void BeginUpdate(object obj);
    /// <summary>
    /// Event raised when the update operation is
    /// complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> UpdateCompleted;
    /// <summary>
    /// Delete a business object.
    /// </summary>
    void BeginDelete(object criteria);
    /// <summary>
    /// Event raised when the delete operation is
    /// complete.
    /// </summary>
    event EventHandler<DataPortalResult<T>> DeleteCompleted;
  }
}
