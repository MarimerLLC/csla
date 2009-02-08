Imports System

''' <summary>
''' Object containing the results of an
''' asynchronous data portal call.
''' </summary>
''' <typeparam name="T">
''' Type of business object.
''' </typeparam>
Public Class DataPortalResult(Of T)
  Inherits EventArgs

  Private _object As T
  Private _error As Exception
  Private _userState As Object

  ''' <summary>
  ''' The business object returned from
  ''' the data portal.
  ''' </summary>
  Public Property [Object]() As T
    Get
      Return _object
    End Get
    Private Set(ByVal value As T)
      _object = value
    End Set
  End Property

  ''' <summary>
  ''' Any Exception object returned from
  ''' the data portal. If this is not null,
  ''' then an exception occurred and should
  ''' be processed.
  ''' </summary>
  Public Property [Error]() As Exception
    Get
      Return _error
    End Get
    Private Set(ByVal value As Exception)
      _error = value
    End Set
  End Property

  ''' <summary>
  ''' Gets the user state value.
  ''' </summary>
  Public Property UserState() As Object
    Get
      Return _userState
    End Get
    Private Set(ByVal value As Object)
      _userState = value
    End Set
  End Property

  ''' <summary>
  ''' Creates and populates an instance of 
  ''' the object.
  ''' </summary>
  ''' <param name="obj">
  ''' The business object to return.
  ''' </param>
  ''' <param name="ex">
  ''' The Exception (if any) to return.
  ''' </param>
  Public Sub New(ByVal obj As T, ByVal ex As Exception, ByVal userState As Object)
    Me.Object = obj
    Me.Error = ex
    Me.UserState = userState
  End Sub
End Class
