Namespace Windows

  Public Class CslaActionEventArgs

    Inherits EventArgs

    Public Sub New(ByVal commandName As String)
      _CommandName = commandName
    End Sub

    Private _CommandName As String

    Public ReadOnly Property CommandName() As String
      Get
        Return _CommandName
      End Get
    End Property

  End Class

End Namespace
