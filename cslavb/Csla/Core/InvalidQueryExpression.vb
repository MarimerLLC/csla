Namespace Core

  Class InvalidQueryException

    Inherits System.Exception

    Private _messageRenamed As String

    Public Sub New(ByVal message As String)
      Me._messageRenamed = message & " "
    End Sub

    Public Overrides ReadOnly Property Message() As String
      Get
        Return My.Resources.ClientQueryIsInvalid & _messageRenamed
      End Get
    End Property

  End Class

End Namespace