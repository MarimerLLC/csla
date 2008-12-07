Imports System.ComponentModel

Namespace Core

  ''' <summary>
  ''' Interface defining an object that notifies when it
  ''' is busy executing an asynchronous operation.
  ''' </summary>
  Public Interface INotifyBusy
    Inherits INotifyUnhandledAsyncException

    ''' <summary>
    ''' Event raised when the object's busy
    ''' status changes.
    ''' </summary>
    Event BusyChanged As BusyChangedEventHandler

    ''' <summary>
    ''' Gets a value indicating whether the object,
    ''' or any of the object's child objects, are
    ''' busy running an asynchronous operation.
    ''' </summary>
    ReadOnly Property IsBusy() As Boolean
    ''' <summary>
    ''' Gets a value indicating whether the object
    ''' is busy running an asynchronous operation.
    ''' </summary>
    ReadOnly Property IsSelfBusy() As Boolean
  End Interface
End Namespace