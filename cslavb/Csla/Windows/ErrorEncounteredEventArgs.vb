Namespace Windows

  Public Class ErrorEncounteredEventArgs

    Inherits CslaActionEventArgs

    Public Sub New(ByVal commandName As String, ByVal ex As Exception)
      MyBase.New(commandName)
      _Ex = ex
    End Sub

    Private _Ex As Exception

    Public ReadOnly Property Ex() As Exception
      Get
        Return _Ex
      End Get
    End Property

  End Class

End Namespace
