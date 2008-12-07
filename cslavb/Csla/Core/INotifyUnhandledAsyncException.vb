Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core

  ''' <summary>
  ''' Implemented by an object that perfoms asynchronous
  ''' operations that may raise exceptions.
  ''' </summary>
  Public Interface INotifyUnhandledAsyncException
    ''' <summary>
    ''' Event indicating that an exception occurred during
    ''' an asynchronous operation.
    ''' </summary>
    Event UnhandledAsyncException As EventHandler(Of ErrorEventArgs)
  End Interface
End Namespace