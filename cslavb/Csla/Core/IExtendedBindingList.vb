Imports System
Imports System.ComponentModel

Namespace Core

  ''' <summary>
  ''' Extends <see cref="IBindingList" /> by adding extra
  ''' events.
  ''' </summary>
  Public Interface IExtendedBindingList
    Inherits IBindingList

    ''' <summary>
    ''' Event indicating that an item is being
    ''' removed from the list.
    ''' </summary>
    Event RemovingItem As EventHandler(Of RemovingItemEventArgs)

  End Interface

End Namespace
