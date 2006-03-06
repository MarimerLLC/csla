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
    Inherits IUndoableObject

    Sub RemoveChild(ByVal child As Core.BusinessBase)
  End Interface

End Namespace
