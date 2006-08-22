using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines the interface that must be implemented
  /// by any business object that contains child
  /// objects.
  /// </summary>
  public interface IParent
  {
    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    void RemoveChild(Core.IEditableBusinessObject child);
    /// <summary>
    /// Override this method to be notified when a child object's
    /// <see cref="Core.BusinessBase.ApplyEdit" /> method has
    /// completed.
    /// </summary>
    /// <param name="child">The child object that was edited.</param>
    void ApplyEditChild(Core.IEditableBusinessObject child);
  }
}
