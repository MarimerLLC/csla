//-----------------------------------------------------------------------
// <copyright file="IEditableCollection.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the common methods required by all</summary>
//-----------------------------------------------------------------------
namespace Csla.Core
{
  /// <summary>
  /// Defines the common methods required by all
  /// editable CSLA collection objects.
  /// </summary>
  /// <remarks>
  /// It is strongly recommended that the implementations
  /// of the methods in this interface be made Private
  /// so as to not clutter up the native interface of
  /// the collection objects.
  /// </remarks>
  public interface IEditableCollection : IBusinessObject, ISupportUndo, ITrackStatus
  {
    /// <summary>
    /// Removes the specified child from the parent
    /// collection.
    /// </summary>
    /// <param name="child">Child object to be removed.</param>
    void RemoveChild(Core.IEditableBusinessObject child);
    /// <summary>
    /// Used by BusinessListBase as a child object is 
    /// created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    void SetParent(IParent parent);
    /// <summary>
    /// Used by ObjectFactory to gain access to the
    /// list of deleted items contained in the collection.
    /// </summary>
    object GetDeletedList();
  }
}