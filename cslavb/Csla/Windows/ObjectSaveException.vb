Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Windows

  ''' <summary>
  ''' Exception indicating a failure during an object
  ''' save operation.
  ''' </summary>
  Public Class ObjectSaveException

    Inherits Exception

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()
      MyBase.New()
    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="message">
    ''' Exception message text.
    ''' </param>
    Public Sub New(ByVal message As String)
      MyBase.New(message)
    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="message">
    ''' Exception message text.
    ''' </param>
    ''' <param name="innerException">
    ''' Reference to an inner exception.
    ''' </param>
    Public Sub New(ByVal message As String, ByVal innerException As Exception)
      MyBase.New(message, innerException)
    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="innerException">
    ''' Reference to an inner exception.
    ''' </param>
    Public Sub New(ByVal innerException As Exception)
      MyBase.New(My.Resources.ExceptionOccurredDuringSaveOperation, innerException)
    End Sub

  End Class

End Namespace
