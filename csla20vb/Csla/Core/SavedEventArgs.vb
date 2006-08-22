Namespace Core

  ''' <summary>
  ''' Event arguments containing a reference
  ''' to the new object that was returned
  ''' as a result of the Save() operation.
  ''' </summary>
  Public Class SavedEventArgs
    Inherits EventArgs

    Private mNewObject As Object
    ''' <summary>
    ''' Gets the object that was returned
    ''' as a result of the Save() operation.
    ''' </summary>
    Public ReadOnly Property NewObject() As Object
      Get
        Return mNewObject
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="newObject">
    ''' The object that was returned as a
    ''' result of the Save() operation.
    ''' </param>
    Public Sub New(ByVal newObject As Object)
      mNewObject = newObject
    End Sub
  End Class

End Namespace
