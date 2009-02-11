Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core

  ''' <summary>
  ''' Event arguments for an unhandled async
  ''' exception.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class ErrorEventArgs
    Inherits EventArgs

    Private _originalSender As Object
    ''' <summary>
    ''' Reference to the original sender of the event.
    ''' </summary>
    Public Property OriginalSender() As Object
      Get
        Return _originalSender
      End Get
      Protected Set(ByVal value As Object)
        _originalSender = value
      End Set
    End Property

    Private _error As Exception
    ''' <summary>
    ''' Reference to the unhandled async exception object.
    ''' </summary>
    Public Property [Error]() As Exception
      Get
        Return _error
      End Get
      Protected Set(ByVal value As Exception)
        _error = value
      End Set
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="originalSender">
    ''' Reference to the original sender of the event.
    ''' </param>
    ''' <param name="error">
    ''' Reference to the unhandled async exception object.
    ''' </param>
    Public Sub New(ByVal originalSender As Object, ByVal [error] As Exception)
      originalSender = originalSender
      [error] = [error]
    End Sub
  End Class
End Namespace