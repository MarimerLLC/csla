namespace Csla.Core
{
  /// <summary>
  /// Defines the common methods required by all
  /// editable CSLA collection objects.
  /// </summary>
  /// <remarks>
  /// It is strongly recommended that the implementations
  /// of the methods in this interface be made private
  /// so as to not clutter up the native interface of
  /// the collection objects.
  /// </remarks>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", 
    "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
  public interface IEditableCollection : IUndoableObject
  {
    void RemoveChild(Core.BusinessBase child);
  }
}