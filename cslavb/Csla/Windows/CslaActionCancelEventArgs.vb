Imports System.ComponentModel

Namespace Windows

  Public Class CslaActionCancelEventArgs

    Inherits CancelEventArgs

    Public Sub New(ByVal cancel As Boolean, ByVal commandName As String)
      MyBase.New(cancel)
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
