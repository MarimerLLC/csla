Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Windows

  Public Class ObjectSaveException

    Inherits Exception

    Public Sub New()
      MyBase.New()
    End Sub

    Public Sub New(ByVal message As String)
      MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
      MyBase.New(message, innerException)
    End Sub

    Public Sub New(ByVal innerException As Exception)
      MyBase.New("An exception ocurred during the save operation.", innerException)
    End Sub

  End Class

End Namespace
