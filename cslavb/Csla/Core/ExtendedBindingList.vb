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
    Private mNonSerializableHandlers As EventHandler(Of RemovingItemEventArgs)
    Private mSerializableHandlers As EventHandler(Of RemovingItemEventArgs)

    ''' <summary>
    ''' Implements a serialization-safe RemovingItem event.
    ''' </summary>
    Public Custom Event RemovingItem As EventHandler(Of RemovingItemEventArgs) _
          Implements IExtendedBindingList.RemovingItem
      AddHandler(ByVal value As EventHandler(Of RemovingItemEventArgs))
        If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
          mSerializableHandlers = CType(System.Delegate.Combine(mSerializableHandlers, value), EventHandler(Of RemovingItemEventArgs))
        Else
          mNonSerializableHandlers = CType(System.Delegate.Combine(mNonSerializableHandlers, value), EventHandler(Of RemovingItemEventArgs))
        End If
      End AddHandler

      RemoveHandler(ByVal value As EventHandler(Of RemovingItemEventArgs))
        If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
          mSerializableHandlers = CType(System.Delegate.Remove(mSerializableHandlers, value), EventHandler(Of RemovingItemEventArgs))
        Else
          mNonSerializableHandlers = CType(System.Delegate.Remove(mNonSerializableHandlers, value), EventHandler(Of RemovingItemEventArgs))
        End If
      End RemoveHandler

      RaiseEvent(ByVal sender As System.Object, ByVal e As RemovingItemEventArgs)
        If mNonSerializableHandlers IsNot Nothing Then
          mNonSerializableHandlers.Invoke(sender, e)
        End If
        If mSerializableHandlers IsNot Nothing Then
          mSerializableHandlers.Invoke(sender, e)
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
