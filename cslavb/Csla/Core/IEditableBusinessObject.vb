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
    Inherits IBusinessObject
    Inherits ISupportUndo
    Inherits IUndoableObject
    Inherits ITrackStatus

    ''' <summary>
    ''' Returns <see langword="true" /> if this object is both dirty and valid.
    ''' </summary>
    ''' <remarks>
    ''' An object is considered dirty (changed) if 
    ''' <see cref="P:Csla.BusinessBase.IsDirty" /> returns <see langword="true" />. It is
    ''' considered valid if IsValid
    ''' returns <see langword="true" />. The IsSavable property is
    ''' a combination of these two properties. 
    ''' </remarks>
    ''' <returns>A value indicating if this object is both dirty and valid.</returns>
    ReadOnly Property IsSavable() As Boolean
    ''' <summary>
    ''' For internal use only!!
    ''' </summary>
    ''' <remarks>
    ''' Altering this value will almost certainly
    ''' break your code. This property is for use
    ''' by the parent collection only!
    ''' </remarks>
    Property EditLevelAdded() As Integer
    ''' <summary>
    ''' Called by a parent object to mark the child
    ''' for deferred deletion.
    ''' </summary>
    Sub DeleteChild()
    ''' <summary>
    ''' Used by BusinessListBase as a child object is 
    ''' created to tell the child object about its
    ''' parent.
    ''' </summary>
    ''' <param name="parent">A reference to the parent collection object.</param>
    Sub SetParent(ByVal parent As IParent)
    ''' <summary>
    ''' Marks the object for deletion. The object will be deleted as part of the
    ''' next save operation.
    ''' </summary>
    Sub Delete()

  End Interface

End Namespace
