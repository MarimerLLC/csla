//-----------------------------------------------------------------------
// <copyright file="ISavable.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Specifies that the object can save</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Specifies that the object can save
  /// itself.
  /// </summary>
  public interface ISavable
  {
    /// <summary>
    /// Saves the object to the database asynchronously. The saved event will contain
    /// the new object when the save operation completes.
    /// </summary>
    void BeginSave();

    /// <summary>
    /// Saves the object to the database asynchronously. The saved event will contain
    /// the new object when the save operation completes.
    /// </summary>
    /// <param name="userState">User state object.</param>
    void BeginSave(object userState);
    /// <summary>
    /// INTERNAL CSLA .NET USE ONLY.
    /// </summary>
    /// <param name="newObject">
    /// The new object returned as a result of the save.
    /// </param>
    /// <param name="error">Exception returned from operation.</param>
    /// <param name="userState">User state object.</param>
    void SaveComplete(object newObject, Exception error, object userState);
    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    event EventHandler<SavedEventArgs> Saved;
  }
}