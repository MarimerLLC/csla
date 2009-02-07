Namespace Core

  ''' <summary>
  ''' Defines the common methods required by all
  ''' editable CSLA collection objects.
  ''' </summary>
  ''' <remarks>
  ''' It is strongly recommended that the implementations
  ''' of the methods in this interface be made Private
  ''' so as to not clutter up the native interface of
  ''' the collection objects.
  ''' </remarks>
  Public Interface IEditableCollection
    Inherits IBusinessObject
    Inherits ISupportUndo
    Inherits ITrackStatus

    ''' <summary>
    ''' Removes the specified child from the parent
    ''' collection.
    ''' </summary>
    ''' <param name="child">Child object to be removed.</param>
    Sub RemoveChild(ByVal child As Core.IEditableBusinessObject)
    ''' <summary>
    ''' Returns <see langword="true" /> if this object is both dirty and valid.
    ''' </summary>
    ''' <returns>A value indicating if this object is both dirty and valid.</returns>
    Overloads ReadOnly Property IsSavable() As Boolean
    ''' <summary>
    ''' Used by BusinessListBase as a child object is 
    ''' created to tell the child object about its
    ''' parent.
    ''' </summary>
    ''' <param name="parent">A reference to the parent collection object.</param>
    Sub SetParent(ByVal parent As IParent)
    ''' <summary>
    ''' Used by ObjectFactory to gain access to the
    ''' list of deleted items contained in the collection.
    ''' </summary>
    Function GetDeletedList() As Object

  End Interface

End Namespace
