Imports System
Imports System.ComponentModel
Imports Csla.Serialization.Mobile
Imports System.Runtime.Serialization

Namespace Core

  ''' <summary>
  ''' Extends BindingList of T by adding extra
  ''' behaviors.
  ''' </summary>
  ''' <typeparam name="T">Type of item contained in list.</typeparam>
  <Serializable()> _
  Public Class ExtendedBindingList(Of T)
    Inherits MobileBindingList(Of T)
    Implements IExtendedBindingList
    Implements IMobileObject
    Implements INotifyBusy
    Implements INotifyChildChanged
    Implements ISerializationNotification

#Region " RemovingItem event "

    <NonSerialized()> _
    Private _nonSerializableHandlers As EventHandler(Of RemovingItemEventArgs)
    Private _serializableHandlers As EventHandler(Of RemovingItemEventArgs)

    ''' <summary>
    ''' Implements a serialization-safe RemovingItem event.
    ''' </summary>
    Public Custom Event RemovingItem As EventHandler(Of RemovingItemEventArgs) Implements IExtendedBindingList.RemovingItem
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

#Region " RemoveItem "

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
      OnRemoveEventHooks(Me(index))
      MyBase.RemoveItem(index)
    End Sub

#End Region

#Region " AddRange "

    ''' <summary>
    ''' Add a range of items to the list.
    ''' </summary>
    ''' <param name="range">List of items to add.</param>
    Public Sub AddRange(ByVal range As IEnumerable(Of T))

      For Each element In range
        Me.Add(element)
      Next

    End Sub

#End Region

#Region "INotifyPropertyBusy Members"

    <NotUndoable()> _
    <NonSerialized()> _
    Private _busyChanged As BusyChangedEventHandler = Nothing

    ''' <summary>
    ''' Event indicating that the busy status of the
    ''' object has changed.
    ''' </summary>
    Public Custom Event BusyChanged As BusyChangedEventHandler Implements INotifyBusy.BusyChanged
      AddHandler(ByVal value As BusyChangedEventHandler)
        _busyChanged = CType(System.Delegate.Combine(_busyChanged, value), BusyChangedEventHandler)
      End AddHandler

      RemoveHandler(ByVal value As BusyChangedEventHandler)
        _busyChanged = CType(System.Delegate.Remove(_busyChanged, value), BusyChangedEventHandler)
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
        If _busyChanged IsNot Nothing Then
          _busyChanged.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Override this method to be notified when the
    ''' IsBusy property has changed.
    ''' </summary>
    ''' <param name="args">Event arguments.</param>    
    Protected Overridable Sub OnBusyChanged(ByVal args As BusyChangedEventArgs)
      If _busyChanged IsNot Nothing Then
        RaiseEvent BusyChanged(Me, args)
      End If
    End Sub

    ''' <summary>
    ''' Raises the BusyChanged event for a specific property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="busy">New busy value.</param>    
    Protected Sub OnBusyChanged(ByVal propertyName As String, ByVal busy As Boolean)
      OnBusyChanged(New BusyChangedEventArgs(propertyName, busy))
    End Sub

    ''' <summary>
    ''' Gets the busy status for this object and its child objects.
    ''' </summary>
    <Browsable(False)> _
    Public Overridable ReadOnly Property IsBusy() As Boolean Implements INotifyBusy.IsBusy
      Get
        Throw New NotImplementedException()
      End Get
    End Property

    ''' <summary>
    ''' Gets the busy status for this object.
    ''' </summary>    
    <Browsable(False)> _
    Public Overridable ReadOnly Property IsSelfBusy() As Boolean Implements INotifyBusy.IsSelfBusy
      Get
        Return IsBusy
      End Get
    End Property

    Private Sub busy_BusyChanged(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
      OnBusyChanged(e)
    End Sub

#End Region

#Region "INotifyUnhandledAsyncException Members"

    <NotUndoable()> _
    <NonSerialized()> _
    Private _unhandledAsyncException As EventHandler(Of ErrorEventArgs)

    ''' <summary>
    ''' Event indicating that an exception occurred during
    ''' an async operation.
    ''' </summary> 
    Public Custom Event UnhandledAsyncException As EventHandler(Of ErrorEventArgs) Implements INotifyUnhandledAsyncException.UnhandledAsyncException
      AddHandler(ByVal value As EventHandler(Of ErrorEventArgs))
        _unhandledAsyncException = CType(System.Delegate.Combine(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
      End AddHandler

      RemoveHandler(ByVal value As EventHandler(Of ErrorEventArgs))
        _unhandledAsyncException = CType(System.Delegate.Remove(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As ErrorEventArgs)
        If _unhandledAsyncException IsNot Nothing Then
          _unhandledAsyncException.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Method invoked when an unhandled async exception has
    ''' occurred.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>    
    Protected Overridable Sub OnUnhandledAsyncException(ByVal e As ErrorEventArgs)
      If _unhandledAsyncException IsNot Nothing Then
        RaiseEvent UnhandledAsyncException(Me, e)
      End If
    End Sub

    ''' <summary>
    ''' Raises the UnhandledAsyncException event.
    ''' </summary>
    ''' <param name="originalSender">Original sender of event.</param>
    ''' <param name="ex">Exception that occurred.</param>    
    Protected Sub OnUnhandledAsyncException(ByVal originalSender As Object, ByVal ex As Exception)
      OnUnhandledAsyncException(New ErrorEventArgs(originalSender, ex))
    End Sub

    Private Sub unhandled_UnhandledAsyncException(ByVal sender As Object, ByVal e As ErrorEventArgs)
      OnUnhandledAsyncException(e)
    End Sub

#End Region

#Region "AddChildHooks"

    ''' <summary>
    ''' Invoked when an item is inserted into the list.
    ''' </summary>
    ''' <param name="index">Index of new item.</param>
    ''' <param name="item">Reference to new item.</param>    
    Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As T)
      MyBase.InsertItem(index, item)
      OnAddEventHooks(item)
    End Sub

    ''' <summary>
    ''' Method invoked when events are hooked for a child
    ''' object.
    ''' </summary>
    ''' <param name="item">Reference to child object.</param>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Overridable Sub OnAddEventHooks(ByVal item As T)
      Dim busy As INotifyBusy = TryCast(item, INotifyBusy)
      If busy IsNot Nothing Then
        AddHandler busy.BusyChanged, AddressOf busy_BusyChanged
      End If

      Dim unhandled As INotifyUnhandledAsyncException = TryCast(item, INotifyUnhandledAsyncException)
      If unhandled IsNot Nothing Then
        AddHandler unhandled.UnhandledAsyncException, AddressOf unhandled_UnhandledAsyncException
      End If

      Dim c As INotifyPropertyChanged = TryCast(item, INotifyPropertyChanged)
      If c IsNot Nothing Then
        AddHandler c.PropertyChanged, AddressOf Child_PropertyChanged
      End If

      Dim child As INotifyChildChanged = TryCast(item, INotifyChildChanged)
      If child IsNot Nothing Then
        AddHandler child.ChildChanged, AddressOf Child_Changed
      End If

    End Sub

    ''' <summary>
    ''' Method invoked when events are unhooked for a child
    ''' object.
    ''' </summary>
    ''' <param name="item">Reference to child object.</param>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Overridable Sub OnRemoveEventHooks(ByVal item As T)
      Dim busy As INotifyBusy = TryCast(item, INotifyBusy)
      If busy IsNot Nothing Then
        RemoveHandler busy.BusyChanged, AddressOf busy_BusyChanged
      End If

      Dim unhandled As INotifyUnhandledAsyncException = TryCast(item, INotifyUnhandledAsyncException)
      If unhandled IsNot Nothing Then
        RemoveHandler unhandled.UnhandledAsyncException, AddressOf unhandled_UnhandledAsyncException
      End If

      Dim c As INotifyPropertyChanged = TryCast(item, INotifyPropertyChanged)
      If c IsNot Nothing Then
        RemoveHandler c.PropertyChanged, AddressOf Child_PropertyChanged
      End If

      Dim child As INotifyChildChanged = TryCast(item, INotifyChildChanged)
      If child IsNot Nothing Then
        RemoveHandler child.ChildChanged, AddressOf Child_Changed
      End If

    End Sub

#End Region

#Region "ISerializationNotification Members"

    ''' <summary>
    ''' This method is called on a newly deserialized object
    ''' after deserialization is complete.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnDeserialized()
      '  do nothing - this is here so a subclass
      '  could override if needed
    End Sub

    <OnDeserialized()> _
    Private Sub OnDeserializedHandler(ByVal context As StreamingContext)

      For Each item As T In Me
        OnAddEventHooks(item)
      Next

      OnDeserialized()
    End Sub

    Private Sub deserialized_Deserialized() Implements ISerializationNotification.Deserialized
      'don't rehook events here, because the MobileFormatter has
      'created new objects and so the lists will auto-subscribe
      'the events
      OnDeserialized()
    End Sub

#End Region

#Region "Child Change Notification"

    <NonSerialized()> _
    <NotUndoable()> _
    Private _childChangedHandlers As EventHandler(Of Csla.Core.ChildChangedEventArgs)

    ''' <summary>
    ''' Event raised when a child object has been changed.
    ''' </summary>    
    Public Custom Event ChildChanged As EventHandler(Of Csla.Core.ChildChangedEventArgs) Implements INotifyChildChanged.ChildChanged
      AddHandler(ByVal value As EventHandler(Of Csla.Core.ChildChangedEventArgs))
        _childChangedHandlers = CType(System.Delegate.Combine(_childChangedHandlers, value), EventHandler(Of Csla.Core.ChildChangedEventArgs))
      End AddHandler

      RemoveHandler(ByVal value As EventHandler(Of Csla.Core.ChildChangedEventArgs))
        _childChangedHandlers = CType(System.Delegate.Remove(_childChangedHandlers, value), EventHandler(Of Csla.Core.ChildChangedEventArgs))
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As ChildChangedEventArgs)
        If _childChangedHandlers IsNot Nothing Then
          _childChangedHandlers.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Raises the ChildChanged event, indicating that a child
    ''' object has been changed.
    ''' </summary>
    ''' <param name="e">
    ''' ChildChangedEventArgs object.
    ''' </param>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnChildChanged(ByVal e As ChildChangedEventArgs)
      If _childChangedHandlers IsNot Nothing Then
        RaiseEvent ChildChanged(Me, e)
      End If
    End Sub

    ''' <summary>
    ''' Creates a ChildChangedEventArgs and raises the event.
    ''' </summary>
    Private Sub RaiseChildChanged( _
      ByVal childObject As Object, ByVal propertyArgs As PropertyChangedEventArgs, ByVal listArgs As ListChangedEventArgs)

      Dim args As ChildChangedEventArgs = New ChildChangedEventArgs(childObject, propertyArgs, listArgs)
      OnChildChanged(args)
    End Sub

    ''' <summary>
    ''' Handles any PropertyChanged event from 
    ''' a child object and echoes it up as
    ''' a ChildChanged event.
    ''' </summary>
    ''' <param name="sender">Object that raised the event.</param>
    ''' <param name="e">Property changed args.</param>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Overridable Sub Child_PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
      RaiseChildChanged(sender, e, Nothing)
    End Sub

    ''' <summary>
    ''' Handles any ChildChanged event from
    ''' a child object and echoes it up as
    ''' a ChildChanged event.
    ''' </summary>    
    Private Sub Child_Changed(ByVal sender As Object, ByVal e As ChildChangedEventArgs)
      RaiseChildChanged(e.ChildObject, e.PropertyChangedArgs, e.ListChangedArgs)
    End Sub

#End Region

  End Class

End Namespace
