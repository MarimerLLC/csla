Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  Public Delegate Sub BusyChangedEventHandler(ByVal sender As Object, ByVal e As BusyChangedEventArgs)

  Public Class BusyChangedEventArgs
    Inherits EventArgs

    Private _busy As Boolean
    Public Property Busy() As Boolean
      Get
        Return _busy
      End Get
      Protected Set(ByVal value As Boolean)
        _busy = value
      End Set
    End Property

    Private _propertyName As String
    Public Property PropertyName() As String
      Get
        Return _propertyName
      End Get
      Protected Set(ByVal value As String)
        _propertyName = value
      End Set
    End Property

    Public Sub New(ByVal propertyName As String, ByVal busy As Boolean)
      propertyName = propertyName
      busy = busy
    End Sub
  End Class
End Namespace