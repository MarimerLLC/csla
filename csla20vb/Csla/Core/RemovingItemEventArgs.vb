Namespace Core

  ''' <summary>
  ''' Contains event data for the RemovingItem
  ''' event.
  ''' </summary>
  Public Class RemovingItemEventArgs
    Inherits EventArgs

    Private mRemovingItem As Object

    ''' <summary>
    ''' Gets a reference to the item that was
    ''' removed from the list.
    ''' </summary>
    Public ReadOnly Property RemovingItem() As Object
      Get
        Return mRemovingItem
      End Get
    End Property

    ''' <summary>
    ''' Create an instance of the object.
    ''' </summary>
    ''' <param name="removingItem">
    ''' A reference to the item that was 
    ''' removed from the list.
    ''' </param>
    Public Sub New(ByVal removingItem As Object)
      mRemovingItem = removingItem
    End Sub
  End Class

End Namespace