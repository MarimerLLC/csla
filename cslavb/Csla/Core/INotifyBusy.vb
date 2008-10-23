Imports System.ComponentModel

Namespace Core
  Public Interface INotifyBusy
    Inherits INotifyUnhandledAsyncException
    Event BusyChanged As BusyChangedEventHandler

    ReadOnly Property IsBusy() As Boolean
    ReadOnly Property IsSelfBusy() As Boolean
  End Interface
End Namespace