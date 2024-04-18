//-----------------------------------------------------------------------
// <copyright file="ISavable.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Specifies that the object can save</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Specifies that the object can save
  /// itself.
  /// </summary>
  public interface ISavable
  {
#if !(ANDROID || IOS) && !NETFX_CORE
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
    /// Saves the object to the database, merging the result into the
    /// original object graph
    /// </summary>
    /// <returns>A new object containing the saved values.</returns>
    Task SaveAndMergeAsync();
    /// <summary>
    /// Saves the object to the database, merging the result into the
    /// original object graph
    /// </summary>
    /// <returns>A new object containing the saved values.</returns>
    /// <param name="forceUpdate">true to force the save to be an update.</param>
    Task SaveAndMergeAsync(bool forceUpdate);
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