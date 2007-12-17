Imports System.Configuration
Imports System.Data.SqlClient

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

    Private Shared mLock As New Object
    Private mConnection As C
    Private mName As String

    ''' <summary>
    ''' Gets the ConnectionManager object for the specified
    ''' database name. The name must correspond to a named
    ''' connection string in the config file.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Type of database connection object to use.
    ''' </typeparam>
    ''' <param name="name">
    ''' Name of the database connection string stored in
    ''' the application config file.
    ''' </param>
    ''' <returns>ConnectionManager object for the connection.</returns>
    Public Shared Function GetManager(Of T As {IDbConnection, New})(ByVal name As String) As ConnectionManager(Of T)

      SyncLock mLock
        Dim mgr As ConnectionManager(Of T)
        If ApplicationContext.LocalContext.Contains("__db:" & name) Then
          mgr = CType(ApplicationContext.LocalContext("__db:" & name), ConnectionManager(Of T))

        Else
          mgr = New ConnectionManager(Of T)(name)
          ApplicationContext.LocalContext("__db:" & name) = mgr
        End If
        mgr.AddRef()
        Return mgr
      End SyncLock

    End Function

    Private Sub New(ByVal name As String)

      mName = name
      ' get connection string
      Dim connectionString As String = _
        ConfigurationManager.ConnectionStrings(name).ConnectionString

      ' open connection
      mConnection = New C
      mConnection.ConnectionString = name
      mConnection.Open()

    End Sub

    ''' <summary>
    ''' Gets the open database connection object.
    ''' </summary>
    Public ReadOnly Property Connection() As C
      Get
        Return mConnection
      End Get
    End Property

    ''' <summary>
    ''' Gets the name of the database connection string.
    ''' </summary>
    Public ReadOnly Property Name() As String
      Get
        Return mName
      End Get
    End Property

#Region " Reference counting "

    Private mRefCount As Integer

    Private Sub AddRef()
      mRefCount += 1
    End Sub

    Private Sub DeRef()

      SyncLock mLock
        mRefCount -= 1
        If mRefCount = 0 Then
          mConnection.Dispose()
          ApplicationContext.LocalContext.Remove("__db:" & mName)
        End If
      End SyncLock

    End Sub

#End Region

#Region " IDisposable "

    Public Sub Dispose() Implements IDisposable.Dispose
      DeRef()
    End Sub

#End Region

  End Class

End Namespace
