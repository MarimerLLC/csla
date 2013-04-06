using System;

namespace Csla.Core
{
  /// <summary>
  /// Event arguments containing a reference
  /// to the new object that was returned
  /// as a result of the Save() operation.
  /// </summary>
  public class SavedEventArgs : EventArgs
  {
    private object _newObject;
    /// <summary>
    /// Gets the object that was returned
    /// as a result of the Save() operation.
    /// </summary>
    public object NewObject
    {
      get { return _newObject; }
    }

    private Exception _error;
    /// <summary>
    /// Gets the exception that occurred
    /// during the save (if any).
    /// </summary>
    public Exception Error
    {
      get
      {
        return _error;
      }
    }

    private object _userState;
    /// <summary>
    /// Gets the user state object.
    /// </summary>
    public object UserState
    {
      get { return _userState; }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="newObject">
    /// The object that was returned as a
    /// result of the Save() operation.
    /// </param>
    /// <param name="error">Error returned as
    /// a result of the save operation.</param>
    /// <param name="userState">User state object.</param>
    public SavedEventArgs(object newObject, Exception error, object userState)
    {
      _newObject = newObject;
      _error = error;
      _userState = userState;
    }
  }
}
