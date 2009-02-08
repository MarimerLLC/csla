Imports System
Imports System.Collections.Generic
Imports System.Runtime.Serialization
Imports System.ComponentModel
Imports Csla.Core

''' <summary>
''' This is the base class from which collections
''' of editable root business objects should be
''' derived.
''' </summary>
''' <typeparam name="T">
''' Type of editable root object to contain within
''' the collection.
''' </typeparam>
''' <remarks>
''' <para>
''' Your subclass should implement a factory method
''' and should override or overload
''' DataPortal_Fetch() to implement data retrieval.
''' </para><para>
''' Saving (inserts or updates) of items in the collection
''' should be handled through the SaveItem() method on
''' the collection. 
''' </para><para>
''' Removing an item from the collection
''' through Remove() or RemoveAt() causes immediate deletion
''' of the object, by calling the object's Delete() and
''' Save() methods.
''' </para>
''' </remarks>
<Serializable()> _
Public MustInherit Class EditableRootListBase(Of T As {Core.IEditableBusinessObject, Core.IUndoableObject, Core.ISavable})
  Inherits Core.ExtendedBindingList(Of T)

  Implements Core.IParent
  Implements Server.IDataPortalTarget

#Region " SaveItem Methods "

  Private _activelySaving As Boolean

  ''' <summary>
  ''' Saves the specified item in the list.
  ''' </summary>
  ''' <param name="item">
  ''' Reference to the item to be saved.
  ''' </param>
  ''' <remarks>
  ''' This method properly saves the child item,
  ''' by making sure the item in the collection
  ''' is properly replaced by the result of the
  ''' Save() method call.
  ''' </remarks>
  Public Function SaveItem(ByVal item As T) As T

    Return SaveItem(IndexOf(item))

  End Function

  ''' <summary>
  ''' Saves the specified item in the list.
  ''' </summary>
  ''' <param name="index">
  ''' Index of the item to be saved.
  ''' </param>
  ''' <remarks>
  ''' This method properly saves the child item,
  ''' by making sure the item in the collection
  ''' is properly replaced by the result of the
  ''' Save() method call.
  ''' </remarks>
  Public Overridable Function SaveItem(ByVal index As Integer) As T

    Dim raiseEvents As Boolean = Me.RaiseListChangedEvents
    Me.RaiseListChangedEvents = False

    _activelySaving = True
    Dim item As T = Nothing
    Dim result As T = Nothing
    Try
      item = Me.Item(index)
      result = item
      Dim savable As T = item

      ' clone the object if possible
      Dim clonable As ICloneable = TryCast(savable, ICloneable)
      If Not clonable Is Nothing Then
        savable = CType(clonable.Clone(), T)
      End If

      ' commit all changes
      Dim editLevel As Integer = savable.EditLevel
      For tmp As Integer = 1 To editLevel
        item.AcceptChanges(editLevel - tmp, False)
      Next

      ' do the save
      result = DirectCast(savable.Save, T)

      If Not ReferenceEquals(result, item) Then
        ' restore edit level to previous level
        For tmp As Integer = 1 To editLevel
          result.CopyState(tmp, False)
        Next
        'put result into collection
        Me.Item(index) = result
      End If

      If Not ReferenceEquals(savable, item) Then
        ' raise Saved event from original object
        Dim original As Core.ISavable = TryCast(item, Core.ISavable)
        If original IsNot Nothing Then
          original.SaveComplete(Me.Item(index))
        End If
      End If

      OnSaved(result, Nothing)

    Finally

      _activelySaving = False
      Me.RaiseListChangedEvents = raiseEvents
    End Try
    Me.OnListChanged(New ListChangedEventArgs(ListChangedType.ItemChanged, index))
    Return result

  End Function

#End Region

#Region " Saved Event "
  <NonSerialized()> _
  <NotUndoable()> _
  Private _nonSerializableSavedHandlers As EventHandler(Of Core.SavedEventArgs)

  <NotUndoable()> _
  Private _serializableSavedHandlers As EventHandler(Of Core.SavedEventArgs)

  ''' <summary>
  ''' Event raised when an object has been saved.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  Public Custom Event Saved As EventHandler(Of Core.SavedEventArgs)
    AddHandler(ByVal value As EventHandler(Of Core.SavedEventArgs))
      If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
        _serializableSavedHandlers = CType(System.Delegate.Combine(_serializableSavedHandlers, value), EventHandler(Of Core.SavedEventArgs))
      Else
        _nonSerializableSavedHandlers = CType(System.Delegate.Combine(_nonSerializableSavedHandlers, value), EventHandler(Of Core.SavedEventArgs))
      End If
    End AddHandler

    RemoveHandler(ByVal value As EventHandler(Of Core.SavedEventArgs))
      If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
        _serializableSavedHandlers = CType(System.Delegate.Remove(_serializableSavedHandlers, value), EventHandler(Of Core.SavedEventArgs))
      Else
        _nonSerializableSavedHandlers = CType(System.Delegate.Remove(_nonSerializableSavedHandlers, value), EventHandler(Of Core.SavedEventArgs))
      End If
    End RemoveHandler

    RaiseEvent(ByVal sender As Object, ByVal e As Core.SavedEventArgs)
      If _serializableSavedHandlers IsNot Nothing Then
        _serializableSavedHandlers.Invoke(Me, e)
      End If
      If _nonSerializableSavedHandlers IsNot Nothing Then
        _nonSerializableSavedHandlers.Invoke(Me, e)
      End If
    End RaiseEvent
  End Event

  ''' <summary>
  ''' Raises the Saved event.
  ''' </summary>
  ''' <param name="newObject">
  ''' Reference to object returned from the save.
  ''' </param>
  ''' <param name="e">
  ''' Reference to any exception that occurred during
  ''' the save.
  ''' </param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Sub OnSaved(ByVal newObject As T, ByVal e As Exception)
    Dim args As Core.SavedEventArgs = New Core.SavedEventArgs(newObject, e, Nothing)
    RaiseEvent Saved(Me, args)
  End Sub

#End Region

#Region " Insert, Remove, Clear "

  ''' <summary>
  ''' Gives the new object a parent reference to this
  ''' list.
  ''' </summary>
  ''' <param name="index">Index at which to insert the item.</param>
  ''' <param name="item">Item to insert.</param>
  Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As T)
    item.SetParent(Me)
    MyBase.InsertItem(index, item)
  End Sub

  ''' <summary>
  ''' Removes an item from the list.
  ''' </summary>
  ''' <param name="index">Index of the item
  ''' to be removed.</param>
  Protected Overrides Sub RemoveItem(ByVal index As Integer)

    ' delete item from database
    Dim item As T = Me.Item(index)

    ' only delete/save the item if it is not new
    Dim raiseEventForNewItem As Boolean = False
    If Not item.IsNew Then
      item.Delete()
      SaveItem(index)
    Else
      raiseEventForNewItem = True
    End If
    MyBase.RemoveItem(index)

    If raiseEventForNewItem Then
      OnSaved(item, Nothing)
    End If

  End Sub

  ''' <summary>
  ''' Replaces an item in the list.
  ''' </summary>
  ''' <param name="index">Index at which the item was replaced.</param>
  ''' <param name="item">New item.</param>
  Protected Overrides Sub SetItem(ByVal index As Integer, ByVal item As T)
    item.SetParent(Me)
    MyBase.SetItem(index, item)
  End Sub

#End Region

#Region " IParent Members "

  Private Sub ApplyEditChild(ByVal child As Core.IEditableBusinessObject) Implements Core.IParent.ApplyEditChild

    If Not _activelySaving AndAlso child.EditLevel = 0 Then
      SaveItem(CType(child, T))
    End If

  End Sub

  Private Sub RemoveChild(ByVal child As Core.IEditableBusinessObject) Implements Core.IParent.RemoveChild
    ' do nothing, removal of a child is handled by
    ' the RemoveItem override
  End Sub

#End Region

#Region " Cascade Child events "

  ''' <summary>
  ''' Handles any PropertyChanged event from 
  ''' a child object and echoes it up as
  ''' a ChildChanged event.
  ''' </summary>
  ''' <param name="sender">Object that raised the event.</param>
  ''' <param name="e">Property changed args.</param>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Protected Overrides Sub Child_PropertyChanged(ByVal sender As Object, _
    ByVal e As System.ComponentModel.PropertyChangedEventArgs)

    For index As Integer = 0 To Count - 1
      If ReferenceEquals(Me(index), sender) Then
        Dim descriptor As PropertyDescriptor = GetPropertyDescriptor(e.PropertyName)
        If descriptor IsNot Nothing Then
          OnListChanged(New System.ComponentModel.ListChangedEventArgs( _
            ComponentModel.ListChangedType.ItemChanged, index, descriptor))

        Else
          OnListChanged(New System.ComponentModel.ListChangedEventArgs( _
            ComponentModel.ListChangedType.ItemChanged, index))
        End If
        Exit For
      End If
    Next
    OnChildPropertyChanged(sender, e)
    'TODO why can I noy see MyBase.Child_PropertyChanged which seems to be there?

  End Sub

  Private Sub Child_BusyChanged(ByVal sender As Object, ByVal e As Core.BusyChangedEventArgs)
    OnBusyChanged(e)
  End Sub

  ''' <summary>
  ''' Override this method to be notified when a child object
  ''' has been changed.
  ''' </summary>
  ''' <param name="sender">
  ''' Child object where the PropertyChanged event originated.
  ''' </param>
  ''' <param name="e">
  ''' PropertyChangedEventArgs from the child object.
  ''' </param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub OnChildPropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs)

  End Sub

  Private Shared mPropertyDescriptors As PropertyDescriptorCollection

  Private Function GetPropertyDescriptor(ByVal propertyName As String) As PropertyDescriptor

    If mPropertyDescriptors Is Nothing Then
      mPropertyDescriptors = TypeDescriptor.GetProperties(Me.GetType)
    End If
    Dim result As PropertyDescriptor = Nothing
    For Each desc As PropertyDescriptor In mPropertyDescriptors
      If desc.Name = propertyName Then
        result = desc
        Exit For
      End If
    Next
    Return result

  End Function

#End Region

#Region " Serialization Notification "

  ''' <summary>
  ''' This method is called on a newly deserialized object
  ''' after deserialization is complete.
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overrides Sub OnDeserialized()

    For Each child As Core.IEditableBusinessObject In Me
      child.SetParent(Me)
      Dim c As INotifyPropertyChanged = DirectCast(child, INotifyPropertyChanged)
      If c IsNot Nothing Then
        AddHandler c.PropertyChanged, AddressOf Child_PropertyChanged
      End If
    Next
    MyBase.OnDeserialized()

  End Sub

#End Region

#Region " Data Access "

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  Private Sub DataPortal_Create(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.CreateNotSupportedException)
  End Sub

  ''' <summary>
  ''' Override this method to allow retrieval of an existing business
  ''' object based on data in the database.
  ''' </summary>
  ''' <param name="criteria">An object containing criteria values to identify the object.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  Protected Overridable Sub DataPortal_Fetch(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.FetchNotSupportedException)
  End Sub

  Private Sub DataPortal_Update()
    Throw New NotSupportedException(My.Resources.UpdateNotSupportedException)
  End Sub

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  Private Sub DataPortal_Delete(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.DeleteNotSupportedException)
  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during data access.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during data access.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception)

  End Sub

#End Region

#Region " ToArray "

  ''' <summary>
  ''' Get an array containing all items in the list.
  ''' </summary>
  Public Function ToArray() As T()

    Dim result As New List(Of T)
    For Each Item As T In Me
      result.Add(Item)
    Next
    Return result.ToArray

  End Function

#End Region

#Region " IDataPortalTarget Members "

  Private Sub CheckRules() Implements Server.IDataPortalTarget.CheckRules

  End Sub

  Private Sub MarkAsChild() Implements Server.IDataPortalTarget.MarkAsChild

  End Sub

  Private Sub MarkNew() Implements Server.IDataPortalTarget.MarkNew

  End Sub

  Private Sub MarkOld() Implements Server.IDataPortalTarget.MarkOld

  End Sub

  Private Sub IDataPortalTarget_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke
    Me.DataPortal_OnDataPortalInvoke(e)
  End Sub

  Private Sub IDataPortalTarget_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete
    Me.DataPortal_OnDataPortalInvokeComplete(e)
  End Sub

  Private Sub IDataPortalTarget_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalException
    Me.DataPortal_OnDataPortalException(e, ex)
  End Sub

  Private Sub Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As System.Exception) Implements Server.IDataPortalTarget.Child_OnDataPortalException

  End Sub

  Private Sub Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvoke

  End Sub

  Private Sub Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete

  End Sub

#End Region

#Region " IsBusy "

  ''' <summary>
  ''' Gets a value indicating whether this object
  ''' is currently running an async operation.
  ''' </summary>
  Public Overrides ReadOnly Property IsBusy() As Boolean
    Get
      ' run through all the child objects
      ' and if any are dirty then then
      ' collection is dirty
      For Each child As T In Me
        If child.IsBusy Then
          Return True
        End If
      Next

      Return False
    End Get
  End Property

#End Region

End Class
