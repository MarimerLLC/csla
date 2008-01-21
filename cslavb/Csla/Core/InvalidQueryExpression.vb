Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  Friend Class InvalidQueryException
    Inherits System.Exception
    Private message_Renamed As String

    Public Sub New(ByVal message As String)
      Me.message_Renamed = message & " "
    End Sub

    Public Overrides ReadOnly Property Message() As String
      Get
        Return "The client query is invalid: " & message_Renamed
      End Get
    End Property
  End Class
End Namespace