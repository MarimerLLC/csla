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
Imports System.Collections.Generic
Imports System.Reflection

Namespace Data

  ''' <summary>
  ''' Provides an automated way to reuse 
  ''' an ADO.NET Data Services context object within 
  ''' the context of a single data portal operation.
  ''' </summary>
  ''' <typeparam name="C">
  ''' Type of context object to use.
  ''' </typeparam>
  Public Class DataServiceContextManager(Of C As System.Data.Services.Client.DataServiceContext)
    Private Shared _lock As New Object()
    Private _context As C

    ''' <summary>
    ''' Gets the DataServiceContext object for the 
    ''' specified URI.
    ''' </summary>
    ''' <param name="path">
    ''' URI to the server-side services.
    ''' </param>
    Public Shared Function GetManager(ByVal path As Uri) As DataServiceContextManager(Of C)
      SyncLock _lock
        Dim mgr As DataServiceContextManager(Of C) = Nothing

        If ApplicationContext.LocalContext.Contains("__ctx:" & path.ToString()) Then
          mgr = DirectCast(ApplicationContext.LocalContext("__ctx:" & path.ToString()), DataServiceContextManager(Of C))
        Else
          mgr = New DataServiceContextManager(Of C)(path)
          ApplicationContext.LocalContext("__ctx:" & path.ToString()) = mgr
        End If

        Return mgr
      End SyncLock
    End Function

    Private Sub New(ByVal path As Uri)
      _context = DirectCast(Activator.CreateInstance(GetType(C), path), C)
    End Sub

    ''' <summary>
    ''' Gets the DataServiceContext object.
    ''' </summary>
    Public ReadOnly Property DataServiceContext() As C
      Get
        Return _context
      End Get
    End Property

    ''' <summary>
    ''' Gets a list of the entities of the
    ''' specified type from the context.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Type of entity.
    ''' </typeparam>
    ''' <returns></returns>
    Public Function GetEntities(Of T)() As List(Of T)
      Dim returnValue As New List(Of T)()

      For Each oneEntityDescriptor In _context.Entities
        If TypeOf oneEntityDescriptor Is T Then
          returnValue.Add(DirectCast(oneEntityDescriptor.Entity, T))
        End If
      Next

      Return returnValue
    End Function

    ''' <summary>
    ''' Gets a list of the entities by key.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Type of entity.
    ''' </typeparam>
    ''' <param name="keyPropertyName">
    ''' Name of the key property.
    ''' </param>
    ''' <param name="keyPropertyValue">
    ''' Key value to match.
    ''' </param>
    Public Function GetEntity(Of T)(ByVal keyPropertyName As String, ByVal keyPropertyValue As Object) As T
      Dim returnValue As T = Nothing

      For Each oneEntity As T In GetEntities(Of T)()
        If keyPropertyValue.Equals(Csla.Reflection.MethodCaller.callpropertygetter(oneEntity, keyPropertyName)) Then
          returnValue = oneEntity
          Exit For
        End If
      Next

      Return returnValue
    End Function
  End Class
End Namespace