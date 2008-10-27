using System;

namespace Csla
{
  /// <summary>
  /// Object containing the results of an
  /// asynchronous data portal call.
  /// </summary>
  /// <typeparam name="T">
  /// Type of business object.
  /// </typeparam>
  public class DataPortalResult<T> : EventArgs
  {
    /// <summary>
    /// The business object returned from
    /// the data portal.
    /// </summary>
    public T Object { get; private set; }
    /// <summary>
    /// Any Exception object returned from
    /// the data portal. If this is not null,
    /// then an exception occurred and should
    /// be processed.
    /// </summary>
    public Exception Error { get; private set; }

    /// <summary>
    /// Gets the user state value.
    /// </summary>
    public object UserState { get; private set; }

    /// <summary>
    /// Creates and populates an instance of 
    /// the object.
    /// </summary>
    /// <param name="obj">
    /// The business object to return.
    /// </param>
    /// <param name="ex">
    /// The Exception (if any) to return.
    /// </param>
    /// <param name="userState">User state object.</param>
    internal DataPortalResult(T obj, Exception ex, object userState)
    {
      this.Object = obj;
      this.Error = ex;
      this.UserState = userState;
    }
  }
}
