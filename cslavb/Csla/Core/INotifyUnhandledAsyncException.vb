Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  Public Interface INotifyUnhandledAsyncException
    Event UnhandledAsyncException As EventHandler(Of ErrorEventArgs)
  End Interface
End Namespace