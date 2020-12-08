//-----------------------------------------------------------------------
// <copyright file="SavedEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Event arguments containing a reference</summary>
//-----------------------------------------------------------------------
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
    /// Gets any exception that occurred during the save.
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
    public SavedEventArgs(object newObject)
    {
      _newObject = newObject;
      _error = null;
      _userState = null;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="newObject">
    /// The object that was returned as a
    /// result of the Save() operation.
    /// </param>
    /// <param name="error">Exception object.</param>
    /// <param name="userState">User state object.</param>
    public SavedEventArgs(object newObject, Exception error, object userState)
    {
      _newObject = newObject;
      _error = error;
      _userState = userState;
    }
  }
}