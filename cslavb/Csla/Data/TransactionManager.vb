Imports System
Imports System.Configuration
Imports System.Data

Namespace Data

  ''' <summary>
  ''' Provides an automated way to reuse open
  ''' database connections and associated
  ''' ADO.NET transactions within the context
  ''' of a single data portal operation.
  ''' </summary>
  ''' <typeparam name="C">
  ''' Type of database connection object to use.
  ''' </typeparam>
  ''' <typeparam name="T">
  ''' Type of ADO.NET transaction object to use.
  ''' </typeparam>
  ''' <remarks>
  ''' This type stores the open ADO.NET transaction
  ''' in <see cref="ApplicationContext.LocalContext" />
  ''' and uses reference counting through
  ''' <see cref="IDisposable" /> to keep the transaction
  ''' open for reuse by child objects, and to automatically
  ''' dispose the transaction when the last consumer
  ''' has called Dispose."
  ''' </remarks>
  Public Class TransactionManager(Of C As {IDbConnection, New}, T As IDbTransaction)
    Implements IDisposable

    Private Shared _lock As Object = New Object()
    Private _connection As C
    Private _transaction As T
    Private _connectionString As String
    Private _label As String

    ''' <summary>
    ''' Gets the TransactionManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' Database name as shown in the config file.
    ''' </param>
    Public Shared Function GetManager(ByVal database As String) As TransactionManager(Of C, T)
      Return GetManager(database, True)
    End Function

    ''' <summary>
    ''' Gets the TransactionManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' Database name as shown in the config file.
    ''' </param>
    ''' <param name="label">Label for this transaction.</param>
    Public Shared Function GetManager(ByVal database As String, ByVal label As String) As TransactionManager(Of C, T)
      Return GetManager(database, True, label)
    End Function

    ''' <summary>
    ''' Gets the TransactionManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' The database name or connection string.
    ''' </param>
    ''' <param name="isDatabaseName">
    ''' True to indicate that the Transaction string
    ''' should be retrieved from the config file. If
    ''' False, the database parameter is directly 
    ''' used as a Transaction string.
    ''' </param>
    ''' <returns>TransactionManager object for the name.</returns>
    Public Shared Function GetManager(ByVal database As String, ByVal isDatabaseName As Boolean) As TransactionManager(Of C, T)
      Return GetManager(database, isDatabaseName, "default")
    End Function

    ''' <summary>
    ''' Gets the TransactionManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' The database name or connection string.
    ''' </param>
    ''' <param name="isDatabaseName">
    ''' True to indicate that the Transaction string
    ''' should be retrieved from the config file. If
    ''' False, the database parameter is directly 
    ''' used as a Transaction string.
    ''' </param>
    ''' <returns>TransactionManager object for the name.</returns>
    Public Shared Function GetManager(ByVal database As String, ByVal isDatabaseName As Boolean, ByVal label As String) As TransactionManager(Of C, T)

      If isDatabaseName Then
        Dim conn As String = ConfigurationManager.ConnectionStrings(database).ConnectionString
        If String.IsNullOrEmpty(conn) Then
          Throw New ConfigurationErrorsException(String.Format(My.Resources.DatabaseNameNotFound, database))
        End If

        database = conn
      End If

      SyncLock (_lock)
        Dim contextLabel = GetContextName(database, label)
        Dim mgr As TransactionManager(Of C, T) = Nothing
        If ApplicationContext.LocalContext.Contains(contextLabel) Then
          mgr = CType(ApplicationContext.LocalContext(contextLabel), TransactionManager(Of C, T))
        Else
          mgr = New TransactionManager(Of C, T)(database, label)
          ApplicationContext.LocalContext(contextLabel) = mgr
        End If
        mgr.AddRef()
        Return mgr
      End SyncLock
    End Function

    Private Sub New(ByVal connectionString As String, ByVal label As String)
      _label = label
      _connectionString = connectionString

      'create and open connection
      _connection = New C()
      _connection.ConnectionString = connectionString
      _connection.Open()
      'start transaction
      _transaction = CType(_connection.BeginTransaction(), T)
    End Sub

    Private Shared Function GetContextName(ByVal connectionString As String, ByVal label As String) As String
      Return "__transaction:" + label + "-" + connectionString
    End Function

    ''' <summary>
    ''' Gets a reference to the current ADO.NET
    ''' transaction object.
    ''' </summary>
    Public ReadOnly Property Transaction() As T
      Get
        Return _transaction
      End Get
    End Property

    ''' <summary>
    ''' Gets a reference to the current ADO.NET
    ''' connection object that is associated with 
    ''' current trasnaction.
    ''' </summary>
    Public ReadOnly Property Connection() As C
      Get
        Return _connection
      End Get
    End Property


#Region "Reference counting"

    Private mRefCount As Integer

    Private Sub AddRef()
      mRefCount += 1
    End Sub

    Private Sub DeRef()
      SyncLock _lock
        mRefCount -= 1

        If mRefCount = 0 Then
          _transaction.Dispose()
          _connection.Dispose()
          ApplicationContext.LocalContext.Remove(GetContextName(_connectionString, _label))
        End If

      End SyncLock
    End Sub

#End Region

#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
      DeRef()
    End Sub

#End Region

  End Class
End Namespace

