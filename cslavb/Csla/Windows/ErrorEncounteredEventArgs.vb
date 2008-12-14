Imports System

Namespace Windows

  ''' <summary>
  ''' Event args indicating an error.
  ''' </summary>
  Public Class ErrorEncounteredEventArgs

    Inherits CslaActionEventArgs

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="commandName">
    ''' Name of the command.
    ''' </param>
    ''' <param name="ex">
    ''' Reference to the exception.
    ''' </param>
    Public Sub New(ByVal commandName As String, ByVal ex As Exception)
      MyBase.New(commandName)
      _Ex = ex
    End Sub

    Private _Ex As Exception

    ''' <summary>
    ''' Gets a reference to the exception object.
    ''' </summary>
    Public ReadOnly Property Ex() As Exception
      Get
        Return _Ex
      End Get
    End Property

  End Class

End Namespace
