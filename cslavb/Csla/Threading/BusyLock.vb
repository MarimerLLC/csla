Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla.Core
Imports System.ComponentModel
Imports System.Threading

Namespace Csla.Threading
  ''' <summary>
  ''' Implementation of a lock that waits while
  ''' a target object is busy.
  ''' </summary>
  ''' <remarks>
  ''' Do not call this from a Silverlight UI thread, as it will block
  ''' the entire browser.
  ''' </remarks>
  Public NotInheritable Class BusyLock
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Wait until the specified object is not busy 
    ''' (IsBusy is false).
    ''' </summary>
    ''' <param name="obj">Target object.</param>
    Public Shared Sub WaitOne(ByVal obj As INotifyBusy)
      Dim locker As New BusyLocker(obj, TimeSpan.FromMilliseconds(Timeout.Infinite))
      locker.WaitOne()
    End Sub

    ''' <summary>
    ''' Wait until the specified object is not busy 
    ''' (IsBusy is false).
    ''' </summary>
    ''' <param name="obj">Target object.</param>
    ''' <param name="timeout">Timeout value.</param>
    Public Shared Sub WaitOne(ByVal obj As INotifyBusy, ByVal timeout As TimeSpan)

      Dim locker As New BusyLocker(obj, timeout)
      locker.WaitOne()
    End Sub
  End Class

  ''' <summary>
  ''' Implementation of a lock that waits while
  ''' a target object is busy.
  ''' </summary>
  Public Class BusyLocker
    Implements IDisposable

    Private _event As New ManualResetEvent(False)
    Private _target As INotifyBusy
    Private _timeout As TimeSpan

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="target">Target object.</param>
    ''' <param name="timeout">Timeout value.</param>
    Public Sub New(ByVal target As INotifyBusy, ByVal timeout As TimeSpan)

      _event.Reset() ' set the event to non-signaled by default.
      _target = target
      _timeout = timeout
    End Sub

    ''' <summary>
    ''' Waits for the target object to become not busy.
    ''' </summary>
    Public Sub WaitOne()
      Try
        AddHandler _target.BusyChanged, AddressOf notify_BusyChanged

        ' Do nothing if this object is not currently busy
        ' otherwise wait for the event to be signaled.
        If _target.IsBusy Then

#If SILVERLIGHT Then
          _event.WaitOne(_timeout)
#Else
          _event.WaitOne(_timeout, False)
#End If
        End If
      Finally
        RemoveHandler _target.BusyChanged, AddressOf notify_BusyChanged
        _event.Close()
      End Try
    End Sub

    Private Sub notify_BusyChanged(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
      ' If the object is not busy then trigger 
      ' the event to unblock the calling thread.
      If Not _target.IsBusy Then _event.Set()
    End Sub

    ''' <summary>
    ''' Disposes the object.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
      WaitOne()
    End Sub
  End Class
End Namespace
  