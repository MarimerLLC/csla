//-----------------------------------------------------------------------
// <copyright file="ISavable.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Specifies that the object can save</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;

namespace Csla.Core
{
  /// <summary>
  /// Specifies that the object can save
  /// itself.
  /// </summary>
  public interface ISavable
  {
#if NETSTANDARD || (!ANDROID && !IOS && !NETFX_CORE)
    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <returns>A new object containing the saved values.</returns>
    object Save();
    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <returns>A new object containing the saved values.</returns>
    /// <param name="forceUpdate">true to force the save to be an update.</param>
    object Save(bool forceUpdate);
#endif
    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <returns>A new object containing the saved values.</returns>
    Task<object> SaveAsync();
    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <returns>A new object containing the saved values.</returns>
    /// <param name="forceUpdate">true to force the save to be an update.</param>
    Task<object> SaveAsync(bool forceUpdate);
    /// <summary>
    /// Saves the object to the database asynchronously. The saved event will contain
    /// the new object when the save operation completes.
    /// </summary>
    void BeginSave();
    /// <summary>
    /// Saves the object to the database asynchronously. The saved event will contain
    /// the new object when the save operation completes.
    /// </summary>
    /// <param name="userState">
    /// User state object.
    /// </param>
    void BeginSave(object userState);
    /// <summary>
    /// INTERNAL CSLA .NET USE ONLY.
    /// </summary>
    /// <param name="newObject">
    /// The new object returned as a result of the save.
    /// </param>
    void SaveComplete(object newObject);
    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    event EventHandler<SavedEventArgs> Saved;
  }
}