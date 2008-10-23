Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core

  Public Class ErrorEventArgs
    Inherits EventArgs

    Private _originalSender As Object
    Public Property OriginalSender() As Object
      Get
        Return _originalSender
      End Get
      Protected Set(ByVal value As Object)
        _originalSender = value
      End Set
    End Property

    Private _error As Exception
    Public Property [Error]() As Exception
      Get
        Return _error
      End Get
      Protected Set(ByVal value As Exception)
        _error = value
      End Set
    End Property

    Public Sub New(ByVal originalSender As Object, ByVal [error] As Exception)
      originalSender = originalSender
      [error] = [error]
    End Sub
  End Class
End Namespace