Imports System

Namespace Windows

  ''' <summary>
  ''' Event args for an action.
  ''' </summary>
  Public Class CslaActionEventArgs

    Inherits EventArgs

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="commandName">
    ''' Name of the command.
    ''' </param>
    Public Sub New(ByVal commandName As String)
      _CommandName = commandName
    End Sub

    Private _CommandName As String

    ''' <summary>
    ''' Gets the name of the command.
    ''' </summary>
    Public ReadOnly Property CommandName() As String
      Get
        Return _CommandName
      End Get
    End Property

  End Class

End Namespace
