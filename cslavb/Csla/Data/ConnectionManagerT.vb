Imports System
Imports System.Configuration
Imports System.Data

Namespace Data
  ''' <summary>
  ''' Provides an automated way to reuse open
  ''' database connections within the context
  ''' of a single data portal operation.
  ''' </summary>
  ''' <typeparam name="C">
  ''' Type of database connection object to use.
  ''' </typeparam>
  ''' <remarks>
  ''' This type stores the open database connection
  ''' in <see cref="Csla.ApplicationContext.LocalContext" />
  ''' and uses reference counting through
  ''' <see cref="IDisposable" /> to keep the connection
  ''' open for reuse by child objects, and to automatically
  ''' dispose the connection when the last consumer
  ''' has called Dispose."
  ''' </remarks>
  Public Class ConnectionManager(Of C As {IDbConnection, New})
    Implements IDisposable

    Private Shared _lock As New Object()
    Private _connection As C
    Private _connectionString As String

    ''' <summary>
    ''' Gets the ConnectionManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' Database name as shown in the config file.
    ''' </param>
    Public Shared Function GetManager(ByVal database As String) As ConnectionManager(Of C)
      Return GetManager(database, True)
    End Function

    ''' <summary>
    ''' Gets the ConnectionManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' The database name or connection string.
    ''' </param>
    ''' <param name="isDatabaseName">
    ''' True to indicate that the connection string
    ''' should be retrieved from the config file. If
    ''' False, the database parameter is directly 
    ''' used as a connection string.
    ''' </param>
    ''' <returns>ConnectionManager object for the name.</returns>
    Public Shared Function GetManager(ByVal database As String, ByVal isDatabaseName As Boolean) As ConnectionManager(Of C)
      If isDatabaseName Then

        Dim connection = ConfigurationManager.ConnectionStrings(database)

        If connection Is Nothing Then Throw New ConfigurationErrorsException(String.Format(My.Resources.DatabaseNameNotFound, database))

        Dim conn = ConfigurationManager.ConnectionStrings(database).ConnectionString

        If String.IsNullOrEmpty(conn) Then Throw New ConfigurationErrorsException(String.Format(My.Resources.DatabaseNameNotFound, database))

        database = conn
      End If

      SyncLock _lock
        Dim mgr As ConnectionManager(Of C) = Nothing

        If (ApplicationContext.LocalContext.Contains("__db:" + database)) Then
          mgr = DirectCast(ApplicationContext.LocalContext("__db:" + database), ConnectionManager(Of C))
        Else
          mgr = New ConnectionManager(Of C)(database)
          ApplicationContext.LocalContext("__db:" + database) = mgr
        End If
        mgr.AddRef()
        Return mgr
      End SyncLock
    End Function

    Private Sub New(ByVal connectionString As String)


      _connectionString = connectionString

      ' open connection
      _connection = New C()
      _connection.ConnectionString = connectionString
      _connection.Open()

    End Sub

    ''' <summary>
    ''' Gets the open database connection object.
    ''' </summary>
    Public ReadOnly Property Connection() As C
      Get
        Return _connection
      End Get
    End Property

#Region " Reference counting "

    Private mRefCount As Integer

    Private Sub AddRef()
      mRefCount += 1
    End Sub

    Private Sub DeRef()

      SyncLock _lock
        mRefCount -= 1

        If mRefCount = 0 Then
          _connection.Dispose()
          ApplicationContext.LocalContext.Remove("__db:" + _connectionString)
        End If
      End SyncLock
    End Sub

#End Region

#Region " IDisposable"

    ''' <summary>
    ''' Dispose object, dereferencing or
    ''' disposing the connection it is
    ''' managing.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
      DeRef()
    End Sub

#End Region

  End Class
End Namespace