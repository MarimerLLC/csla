Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  ''' <summary>
  ''' Implemented by classes that notify when 
  ''' a child object has changed.
  ''' </summary>
  Public Interface INotifyChildChanged
    ''' <summary>
    ''' Event indictating that a child object
    ''' has changed.
    ''' </summary>
    Event ChildChanged As EventHandler(Of ChildChangedEventArgs)
  End Interface
End Namespace