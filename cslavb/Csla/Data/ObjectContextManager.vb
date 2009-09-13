#If Not CLIENTONLY Then
Imports System
Imports System.Configuration
Imports System.Data.Objects

Namespace Data

  ''' <summary>
  ''' Provides an automated way to reuse 
  ''' Entity Framework object context objects 
  ''' within the context
  ''' of a single data portal operation.
  ''' </summary>
  ''' <typeparam name="C">
  ''' Type of database 
  ''' object context object to use.
  ''' </typeparam>
  ''' <remarks>
  ''' This type stores the object context object 
  ''' in <see cref="Csla.ApplicationContext.LocalContext" />
  ''' and uses reference counting through
  ''' <see cref="IDisposable" /> to keep the data context object
  ''' open for reuse by child objects, and to automatically
  ''' dispose the object when the last consumer
  ''' has called Dispose."
  ''' </remarks>
  Public Class ObjectContextManager(Of C As ObjectContext)
    Implements IDisposable

    Private Shared _lock As New Object()
    Private _context As C
    Private _connectionString As String
    Private _label As String

    ''' <summary>
    ''' Gets the ObjectContextManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' Database name as shown in the config file.
    ''' </param>
    Public Shared Function GetManager(ByVal database As String) As ObjectContextManager(Of C)
      Return GetManager(database, True)
    End Function

    ''' <summary>
    ''' Gets the ObjectContextManager object for the 
    ''' specified database.
    ''' </summary>
    ''' <param name="database">
    ''' Database name as shown in the config file.
    ''' </param>
    ''' <param name="label">Label for this context.</param>
    Public Shared Function GetManager(ByVal database As String, ByVal label As String) As ObjectContextManager(Of C)
      Return GetManager(database, True, label)
    End Function

    ''' <summary>
    ''' Gets the ObjectContextManager object for the 
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
    ''' <returns>ContextManager object for the name.</returns>
    Public Shared Function GetManager(ByVal database As String, ByVal isDatabaseName As Boolean) As ObjectContextManager(Of C)
      Return GetManager(database, isDatabaseName, "default")
    End Function

    ''' <summary>
    ''' Gets the ObjectContextManager object for the 
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
    ''' <param name="label">Label for this context.</param>
    ''' <returns>ContextManager object for the name.</returns>
    Public Shared Function GetManager(ByVal database As String, ByVal isDatabaseName As Boolean, ByVal label As String) As ObjectContextManager(Of C)

      If isDatabaseName Then

        Dim connection = ConfigurationManager.ConnectionStrings(database)
        If connection Is Nothing Then
          Throw New ConfigurationErrorsException(String.Format(My.Resources.DatabaseNameNotFound, database))
        End If

        Dim conn = ConfigurationManager.ConnectionStrings(database).ConnectionString
        If String.IsNullOrEmpty(conn) Then Throw New ConfigurationErrorsException(String.Format(My.Resources.DatabaseNameNotFound, database))

        database = conn
      End If

      SyncLock _lock
        Dim contextLabel = GetContextName(database, label)
        Dim mgr As ObjectContextManager(Of C) = Nothing
        If (ApplicationContext.LocalContext.Contains(contextLabel)) Then

          mgr = DirectCast(ApplicationContext.LocalContext(contextLabel), ObjectContextManager(Of C))


        Else
          mgr = New ObjectContextManager(Of C)(database, label)
          ApplicationContext.LocalContext(contextLabel) = mgr
        End If

        mgr.AddRef()
        Return mgr
      End SyncLock
    End Function

    Private Sub New(ByVal connectionString As String, ByVal label As String)
      _label = label
      _connectionString = connectionString
      _context = CType(Activator.CreateInstance(GetType(C), connectionString), C)
      _context.Connection.Open()
    End Sub

    Private Shared Function GetContextName(ByVal connectionString As String, ByVal label As String) As String
      Return "__octx:" + label + "-" + connectionString
    End Function

    ''' <summary>
    ''' Gets the EF object context object.
    ''' </summary>
    Public ReadOnly Property ObjectContext() As C
      Get
        Return _context
      End Get
    End Property

#Region "  Reference counting"

    Private mRefCount As Integer

    Private Sub AddRef()

      mRefCount += 1
    End Sub

    Private Sub DeRef()
      SyncLock (_lock)
        mRefCount -= 1

        If mRefCount = 0 Then

          _context.Dispose()
          ApplicationContext.LocalContext.Remove(GetContextName(_connectionString, _label))
        End If
      End SyncLock

    End Sub

#End Region

#Region "  IDisposable"

    ''' <summary>
    ''' Dispose object, dereferencing or
    ''' disposing the context it is
    ''' managing.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
      DeRef()
    End Sub

#End Region

  End Class
End Namespace
#End If