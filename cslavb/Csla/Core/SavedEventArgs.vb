Imports System

Namespace Core

  ''' <summary>
  ''' Event arguments containing a reference
  ''' to the new object that was returned
  ''' as a result of the Save() operation.
  ''' </summary>
  Public Class SavedEventArgs
    Inherits EventArgs

    Private _newObject As Object

    ''' <summary>
    ''' Gets the object that was returned
    ''' as a result of the Save() operation.
    ''' </summary>
    Public ReadOnly Property NewObject() As Object
      Get
        Return _newObject
      End Get
    End Property

    Private _error As Exception
    ''' <summary>
    ''' Gets any exception that occurred during the save.
    ''' </summary>
    Public ReadOnly Property [Error]() As Exception
      Get
        Return _error
      End Get
    End Property

    Private _userState As Object
    ''' <summary>
    ''' Gets the user state object.
    ''' </summary>
    Public ReadOnly Property UserState() As Object
      Get
        Return _userState
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="newObject">
    ''' The object that was returned as a
    ''' result of the Save() operation.
    ''' </param>
    Public Sub New(ByVal newObject As Object)
      _newObject = newObject
      _error = Nothing
      _userState = Nothing
    End Sub

    Public Sub New(ByVal newObject As Object, ByVal [error] As Exception, ByVal userState As Object)
      _newObject = newObject
      _error = [error]
      _userState = userState
    End Sub

  End Class

End Namespace
