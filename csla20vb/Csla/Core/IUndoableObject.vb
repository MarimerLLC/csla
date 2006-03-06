Namespace Core

  ''' <summary>
  ''' Defines the methods required to participate
  ''' in n-level undo within the CSLA .NET framework.
  ''' </summary>
  ''' <remarks>
  ''' This interface is used by Csla.Core.UndoableBase
  ''' to initiate begin, cancel and apply edit operations.
  ''' </remarks>
  Public Interface IUndoableObject
    Inherits IBusinessObject

    Sub CopyState()
    Sub UndoChanges()
    Sub AcceptChanges()
  End Interface

End Namespace
