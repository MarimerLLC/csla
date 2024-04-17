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
    /// <summary>
    /// Gets the object that was returned
    /// as a result of the Save() operation.
    /// </summary>
    public object NewObject { get; }

    /// <summary>
    /// Gets any exception that occurred during the save.
    /// </summary>
    public Exception Error { get; }

    /// <summary>
    /// Gets the user state object.
    /// </summary>
    public object UserState { get; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="newObject">
    /// The object that was returned as a
    /// result of the Save() operation.
    /// </param>
    public SavedEventArgs(object newObject)
    {
      NewObject = newObject;
      Error = null;
      UserState = null;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="newObject">
    /// The object that was returned as a
    /// result of the Save() operation.
    /// </param>
    /// <param name="error">Exception object.</param>
    /// <param name="userState">User state object.</param>
    public SavedEventArgs(object newObject, Exception error, object userState)
    {
      NewObject = newObject;
      Error = error;
      UserState = userState;
    }
  }
}