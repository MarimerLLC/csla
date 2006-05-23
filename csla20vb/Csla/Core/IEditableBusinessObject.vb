Namespace Core

  ''' <summary>
  ''' Defines the common methods required by all
  ''' editable CSLA single objects.
  ''' </summary>
  ''' <remarks>
  ''' It is strongly recommended that the implementations
  ''' of the methods in this interface be made Private
  ''' so as to not clutter up the native interface of
  ''' the collection objects.
  ''' </remarks>
  Public Interface IEditableBusinessObject
    Inherits IUndoableObject

    ReadOnly Property IsDirty() As Boolean
    ReadOnly Property IsValid() As Boolean
    ReadOnly Property IsDeleted() As Boolean
    ''' <summary>
    ''' For internal use only!!
    ''' </summary>
    ''' <remarks>
    ''' Altering this value will almost certainly
    ''' break your code. This property is for use
    ''' by the parent collection only!
    ''' </remarks>
    Property EditLevelAdded() As Integer
    Sub DeleteChild()
    Sub SetParent(ByVal parent As IEditableCollection)

  End Interface

End Namespace
