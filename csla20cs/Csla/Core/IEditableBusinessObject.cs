using System;
using System.Collections.Generic;
using System.Text;

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
  public interface IEditableBusinessObject : IUndoableObject
  {
    bool IsDirty { get;}
    bool IsValid { get;}
    bool IsDeleted { get;}
    /// <summary>
    /// For internal use only!!
    /// </summary>
    /// <remarks>
    /// Altering this value will almost certainly
    /// break your code. This property is for use
    /// by the parent collection only!
    /// </remarks>
    int EditLevelAdded { get; set;}
    void DeleteChild();
    void SetParent(IEditableCollection parent);
  }
}
