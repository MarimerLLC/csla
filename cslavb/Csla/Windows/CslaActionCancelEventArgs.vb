Imports System
Imports System.ComponentModel

Namespace Windows

  ''' <summary>
  ''' Event args providing information about
  '''  a canceled action.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class CslaActionCancelEventArgs

    Inherits CancelEventArgs

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="cancel">
    ''' Indicates whether a cancel should occur.
    ''' </param>
    ''' <param name="commandName">
    ''' Name of the command.
    ''' </param>
    Public Sub New(ByVal cancel As Boolean, ByVal commandName As String)
      MyBase.New(cancel)
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
