Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  ''' <summary>
  ''' Delegate for handling the BusyChanged event.
  ''' </summary>
  ''' <param name="sender">
  ''' Object raising the event.
  ''' </param>
  ''' <param name="e">
  ''' Event arguments.
  ''' </param>
  Public Delegate Sub BusyChangedEventHandler(ByVal sender As Object, ByVal e As BusyChangedEventArgs)

  ''' <summary>
  ''' Event arguments for the BusyChanged event.
  ''' </summary>
  Public Class BusyChangedEventArgs
    Inherits EventArgs

    ''' <summary>
    ''' New busy value.
    ''' </summary>
    Private _busy As Boolean
    Public Property Busy() As Boolean
      Get
        Return _busy
      End Get
      Protected Set(ByVal value As Boolean)
        _busy = value
      End Set
    End Property

    ''' <summary>
    ''' Property for which the Busy value has changed.
    ''' </summary>
    Private _propertyName As String
    Public Property PropertyName() As String
      Get
        Return _propertyName
      End Get
      Protected Set(ByVal value As String)
        _propertyName = value
      End Set
    End Property

    ''' <summary>
    ''' Creates a new instance of the object.
    ''' </summary>
    ''' <param name="propertyName">
    ''' Property for which the Busy value has changed.
    ''' </param>
    ''' <param name="busy">
    ''' New Busy value.
    ''' </param>
    Public Sub New(ByVal propertyName As String, ByVal busy As Boolean)
      _propertyName = propertyName
      _busy = busy
    End Sub
  End Class
End Namespace