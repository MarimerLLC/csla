Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.Common

Namespace Data

  ''' <summary>
  ''' Provides an automated way to reuse open
  ''' database connections within the context
  ''' of a single data portal operation.
  ''' </summary>
  ''' <remarks>
  ''' This type stores the open database connection
  ''' in <see cref="Csla.ApplicationContext.LocalContext" />
  ''' and uses reference counting through
  ''' <see cref="IDisposable" /> to keep the connection
  ''' open for reuse by child objects, and to automatically
  ''' dispose the connection when the last consumer
  ''' has called Dispose."
  ''' </remarks>
  Public Class ConnectionManager

    Implements IDisposable

    Private Shared _lock As New Object
    Private _connection As IDbConnection
    Private _connectionString As String

    ''' <summary>
    ''' Gets the ConnectionManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' Database name as shown in the config file.
    ''' </param>
    Public Shared Function GetManager(ByVal database As String) As ConnectionManager

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
    Public Shared Function GetManager(ByVal database As String, ByVal isDatabaseName As Boolean) As ConnectionManager

      If isDatabaseName Then
        Dim conn = ConfigurationManager.ConnectionStrings(database).ConnectionString
        If String.IsNullOrEmpty(conn) Then
          Throw New ConfigurationErrorsException(String.Format(My.Resources.DatabaseNameNotFound, database))
        End If
        database = conn
      End If

      SyncLock _lock
        Dim mgr As ConnectionManager = Nothing
        If ApplicationContext.LocalContext.Contains("__db:" & database) Then
          mgr = CType(ApplicationContext.LocalContext("__db:" & database), ConnectionManager)

        Else
          mgr = New ConnectionManager(database)
          ApplicationContext.LocalContext("__db:" & database) = mgr
        End If
        mgr.AddRef()
        Return mgr
      End SyncLock

    End Function

    Private Sub New(ByVal connectionString As String)

      _connectionString = connectionString

      Dim provider As String = ConfigurationManager.AppSettings("dbProvider")
      If (String.IsNullOrEmpty(provider)) Then
        provider = "System.Data.SqlClient"
      End If

      Dim factory As DbProviderFactory = DbProviderFactories.GetFactory(provider)

      ' open connection
      _connection = factory.CreateConnection()
      _connection.ConnectionString = connectionString
      _connection.Open()

    End Sub

    ''' <summary>
    ''' Dispose object, dereferencing or
    ''' disposing the connection it is
    ''' managing.
    ''' </summary>
    Public ReadOnly Property Connection() As IDbConnection
      Get
        Return _connection
      End Get
    End Property

#Region " Reference counting "

    Private _refCount As Integer

    Private Sub AddRef()
      _refCount += 1
    End Sub

    Private Sub DeRef()

      SyncLock _lock
        _refCount -= 1
        If _refCount = 0 Then
          _connection.Dispose()
          ApplicationContext.LocalContext.Remove("__db:" & _connectionString)
        End If
      End SyncLock

    End Sub

#End Region

#Region " IDisposable "

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
