Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  Public Interface INotifyChildChanged
    Event ChildChanged As EventHandler(Of ChildChangedEventArgs)
  End Interface
End Namespace