//-----------------------------------------------------------------------
// <copyright file="IEditableBusinessObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the common methods required by all</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines the common methods required by all
  /// editable CSLA single objects.
  /// </summary>
  /// <remarks>
  /// It is strongly recommended that the implementations
  /// of the methods in this interface be made Private
  /// so as to not clutter up the native interface of
  /// the collection objects.
  /// </remarks>
  public interface IEditableBusinessObject : IBusinessObject, ISupportUndo, IUndoableObject, ITrackStatus
  {
    /// <summary>
    /// For internal use only!!
    /// </summary>
    /// <remarks>
    /// Altering this value will almost certainly
    /// break your code. This property is for use
    /// by the parent collection only!
    /// </remarks>
    int EditLevelAdded { get; set;}
    /// <summary>
    /// Called by a parent object to mark the child
    /// for deferred deletion.
    /// </summary>
    void DeleteChild();
    /// <summary>
    /// Used by BusinessListBase as a child object is 
    /// created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    void SetParent(IParent parent);
    /// <summary>
    /// Marks the object for deletion. The object will be deleted as part of the
    /// next save operation.
    /// </summary>
    void Delete();
  }
}