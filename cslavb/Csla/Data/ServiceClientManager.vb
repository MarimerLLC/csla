Imports System
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Namespace Data
  ''' <summary>
  ''' Provides an automated way to reuse 
  ''' a service client proxy objects within 
  ''' the context of a single data portal operation.
  ''' </summary>
  ''' <typeparam name="C">
  ''' Type of ClientBase object to use.
  ''' </typeparam>
  ''' <typeparam name="T">
  ''' Channel type for the ClientBase object.
  ''' </typeparam>
  Public Class ServiceClientManager(Of C As System.ServiceModel.ClientBase(Of T), T As Class)

    Private Shared _lock As Object = New Object()
    Private _client As C
    Private _name As String = String.Empty

    ''' <summary>
    ''' Gets the client proxy object for the
    ''' specified name.
    ''' </summary>
    ''' <param name="name">Unique name for the proxy object.</param>
    ''' <returns></returns>
    Public Shared Function GetManager(ByVal name As String) As ServiceClientManager(Of C, T)
      SyncLock _lock
        Dim mgr As ServiceClientManager(Of C, T) = Nothing

        If ApplicationContext.LocalContext.Contains(name) Then
          mgr = CType(ApplicationContext.LocalContext(name), ServiceClientManager(Of C, T))
        Else
          mgr = New ServiceClientManager(Of C, T)(name)
          ApplicationContext.LocalContext(name) = mgr
        End If

        Return mgr
      End SyncLock

    End Function

    Private Sub New(ByVal name As String)
      _client = CType(Activator.CreateInstance(GetType(C)), C)
    End Sub

    ''' <summary>
    ''' Gets a reference to the current client proxy object.
    ''' </summary>
    Public ReadOnly Property Client() As C
      Get
        Return _client
      End Get
    End Property

  End Class
End Namespace

