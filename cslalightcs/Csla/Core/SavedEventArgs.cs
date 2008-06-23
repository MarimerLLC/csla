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

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="newObject">
    /// The object that was returned as a
    /// result of the Save() operation.
    /// </param>
    public SavedEventArgs(object newObject)
    {
      _newObject = newObject;
    }
  }
}
