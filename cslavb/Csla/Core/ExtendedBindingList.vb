Imports System.ComponentModel

Namespace Core

  ''' <summary>
  ''' Extends BindingList of T by adding extra
  ''' events.
  ''' </summary>
  ''' <typeparam name="T">Type of item contained in list.</typeparam>
  <Serializable()> _
  Public Class ExtendedBindingList(Of T)
    Inherits BindingList(Of T)

    Implements IExtendedBindingList

#Region " RemovingItem event "

    <NonSerialized()> _
    Private _nonSerializableHandlers As EventHandler(Of RemovingItemEventArgs)
    Private _serializableHandlers As EventHandler(Of RemovingItemEventArgs)

    ''' <summary>
    ''' Implements a serialization-safe RemovingItem event.
    ''' </summary>
    Public Custom Event RemovingItem As EventHandler(Of RemovingItemEventArgs) _
          Implements IExtendedBindingList.RemovingItem
      AddHandler(ByVal value As EventHandler(Of RemovingItemEventArgs))
        If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
          _serializableHandlers = CType(System.Delegate.Combine(_serializableHandlers, value), EventHandler(Of RemovingItemEventArgs))
        Else
          _nonSerializableHandlers = CType(System.Delegate.Combine(_nonSerializableHandlers, value), EventHandler(Of RemovingItemEventArgs))
        End If
      End AddHandler

      RemoveHandler(ByVal value As EventHandler(Of RemovingItemEventArgs))
        If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
          _serializableHandlers = CType(System.Delegate.Remove(_serializableHandlers, value), EventHandler(Of RemovingItemEventArgs))
        Else
          _nonSerializableHandlers = CType(System.Delegate.Remove(_nonSerializableHandlers, value), EventHandler(Of RemovingItemEventArgs))
        End If
      End RemoveHandler

      RaiseEvent(ByVal sender As System.Object, ByVal e As RemovingItemEventArgs)
        If _nonSerializableHandlers IsNot Nothing Then
          _nonSerializableHandlers.Invoke(sender, e)
        End If
        If _serializableHandlers IsNot Nothing Then
          _serializableHandlers.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Raise the RemovingItem event.
    ''' </summary>
    ''' <param name="removedItem">
    ''' A reference to the item that 
    ''' is being removed.
    ''' </param>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Sub OnRemovingItem(ByVal removedItem As T)

      RaiseEvent RemovingItem(Me, New RemovingItemEventArgs(removedItem))

    End Sub

#End Region

    ''' <summary>
    ''' Remove the item at the
    ''' specified index.
    ''' </summary>
    ''' <param name="index">
    ''' The zero-based index of the item
    ''' to remove.
    ''' </param>
    Protected Overrides Sub RemoveItem(ByVal index As Integer)
      OnRemovingItem(Me(index))
      MyBase.RemoveItem(index)
    End Sub

  End Class

End Namespace
