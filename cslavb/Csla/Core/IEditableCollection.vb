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
  <System.Diagnostics.CodeAnalysis.SuppressMessage( _
   "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")> _
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
    ''' Used by BusinessListBase as a child object is 
    ''' created to tell the child object about its
    ''' parent.
    ''' </summary>
    ''' <param name="parent">A reference to the parent collection object.</param>
    Sub SetParent(ByVal parent As IParent)

  End Interface

End Namespace
