Imports System
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Csla.Serialization

Namespace Core.LoadManager
  Friend Class AsyncLoadManager
    Implements INotifyBusy
    Private _loading As New ObservableCollection(Of AsyncLoader)()

    Public Sub New()
    End Sub

    Public ReadOnly Property IsLoading() As Boolean
      Get
        SyncLock _loading
          Return _loading.Count > 0
        End SyncLock
      End Get
    End Property

    Public Sub BeginLoad(ByVal loader As AsyncLoader, ByVal complete As [Delegate])
      Dim isAlreadyBusy As Boolean = False
      SyncLock _loading
        isAlreadyBusy = (From l In _loading _
            Where l.[Property] Is loader.[Property] _
            Select l).Count() > 0
        _loading.Add(loader)
      End SyncLock

      AddHandler loader.Complete, AddressOf loader_Complete

      If Not isAlreadyBusy Then
        OnPropertyBusy(loader.[Property].Name, True)
      End If

      loader.Load(complete)
    End Sub

    Private Sub loader_Complete(ByVal sender As Object, ByVal e As ErrorEventArgs)
      Dim loader As AsyncLoader = DirectCast(sender, AsyncLoader)
      AddHandler loader.Complete, AddressOf loader_Complete

      Dim isStillBusy As Boolean = False
      SyncLock _loading
        _loading.Remove(loader)
        isStillBusy = (From l In _loading _
            Where l.[Property] Is loader.[Property] _
            Select l).Count() > 0
      End SyncLock

      If Not isStillBusy Then
        OnPropertyBusy(loader.[Property].Name, False)
      End If

      If e.[Error] IsNot Nothing Then
        OnUnhandledAsyncException(Me, e.[Error])
      End If
    End Sub

#Region "INotifyPropertyBusy Members"

    Public Event BusyChanged As BusyChangedEventHandler Implements INotifyBusy.BusyChanged

    Protected Sub OnPropertyBusy(ByVal propertyName As String, ByVal busy As Boolean)
      RaiseEvent BusyChanged(Me, New BusyChangedEventArgs(propertyName, busy))
    End Sub

#End Region

#Region "INotifyBusy Members"

    Private ReadOnly Property IsBusy() As Boolean Implements INotifyBusy.IsBusy
      Get
        Return TryCast(Me, INotifyBusy).IsSelfBusy
      End Get
    End Property

    Private ReadOnly Property IsSelfBusy() As Boolean Implements INotifyBusy.IsSelfBusy
      Get
        Return IsLoading
      End Get
    End Property

#End Region

#Region "INotifyUnhandledAsyncException Members"

    <NotUndoable()> _
    Private _unhandledAsyncException As EventHandler(Of ErrorEventArgs)

    Public Custom Event UnhandledAsyncException As EventHandler(Of ErrorEventArgs) Implements INotifyUnhandledAsyncException.UnhandledAsyncException
      AddHandler(ByVal value As EventHandler(Of ErrorEventArgs))
        _unhandledAsyncException = DirectCast([Delegate].Combine(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
      End AddHandler
      RemoveHandler(ByVal value As EventHandler(Of ErrorEventArgs))
        _unhandledAsyncException = DirectCast([Delegate].Combine(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
      End RemoveHandler
      RaiseEvent(ByVal sender As Object, ByVal e As ErrorEventArgs)
        If _unhandledAsyncException IsNot Nothing Then
          _unhandledAsyncException.Invoke(sender, e)
        End If

      End RaiseEvent
    End Event

    Protected Overridable Sub OnUnhandledAsyncException(ByVal [error] As ErrorEventArgs)
      RaiseEvent UnhandledAsyncException(Me, [error])
    End Sub

    Protected Sub OnUnhandledAsyncException(ByVal originalSender As Object, ByVal [error] As Exception)
      OnUnhandledAsyncException(New ErrorEventArgs(originalSender, [error]))
    End Sub

#End Region
  End Class
End Namespace