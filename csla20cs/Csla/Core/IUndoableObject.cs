namespace Csla.Core
{
  /// <summary>
  /// Defines the methods required to participate
  /// in n-level undo within the CSLA .NET framework.
  /// </summary>
  /// <remarks>
  /// This interface is used by 
  /// <see cref="UndoableBase">UndoableBase</see>
  /// to initiate begin, cancel and apply edit operations.
  /// </remarks>
  public interface IUndoableObject : IBusinessObject
  {
    void CopyState();
    void UndoChanges();
    void AcceptChanges();
  }
}
