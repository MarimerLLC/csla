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
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", 
    "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
  public interface IEditableCollection : IBusinessObject, ISupportUndo, ITrackStatus
  {
    /// <summary>
    /// Removes the specified child from the parent
    /// collection.
    /// </summary>
    /// <param name="child">Child object to be removed.</param>
    void RemoveChild(Core.IEditableBusinessObject child);
    /// <summary>
    /// Returns <see langword="true" /> if this object is both dirty and valid.
    /// </summary>
    /// <returns>A value indicating if this object is both dirty and valid.</returns>
    bool IsSavable { get; }
  }
}