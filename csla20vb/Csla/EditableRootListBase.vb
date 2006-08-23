Imports System
Imports System.ComponentModel

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
Public MustInherit Class EditableRootListBase(Of T As {Core.IEditableBusinessObject, Core.ISavable})
  Inherits Core.ExtendedBindingList(Of T)

  Implements Core.IParent

#Region " SaveItem Methods "

  Private mActivelySaving As Boolean

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
  Public Sub SaveItem(ByVal item As T)

    SaveItem(IndexOf(item))

  End Sub

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
  Public Overridable Sub SaveItem(ByVal index As Integer)

    mActivelySaving = True
    Dim raiseEvents As Boolean = Me.RaiseListChangedEvents
    Me.RaiseListChangedEvents = False

    Dim item As T = Me.Item(index)
    Dim editLevel As Integer = item.EditLevel
    ' commit all changes
    For tmp As Integer = 1 To editLevel
      item.AcceptChanges()
    Next
    ' do the save
    Me.Item(index) = DirectCast(item.Save, T)
    ' restore edit level to previous level
    For tmp As Integer = 1 To editLevel
      item.CopyState()
    Next

    Me.RaiseListChangedEvents = raiseEvents
    mActivelySaving = False

  End Sub

#End Region

#Region " Insert, Remove, Clear "

  ''' <summary>
  ''' Gives the new object a parent reference to this
  ''' list.
  ''' </summary>
  Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As T)
    item.SetParent(Me)
    MyBase.InsertItem(index, item)
  End Sub

  Protected Overrides Sub RemoveItem(ByVal index As Integer)

    ' delete item from database
    Dim item As T = Me.Item(index)
    item.Delete()
    SaveItem(index)

    ' disconnect event handler if necessary
    Dim c As System.ComponentModel.INotifyPropertyChanged = TryCast(item, System.ComponentModel.INotifyPropertyChanged)
    If c IsNot Nothing Then
      RemoveHandler c.PropertyChanged, AddressOf Child_PropertyChanged
    End If

    MyBase.RemoveItem(index)

  End Sub

#End Region

#Region " IParent Members "

  Private Sub ApplyEditChild(ByVal child As Core.IEditableBusinessObject) Implements Core.IParent.ApplyEditChild

    If Not mActivelySaving AndAlso child.EditLevel = 0 Then
      SaveItem(CType(child, T))
    End If

  End Sub

  Private Sub RemoveChild(ByVal child As Core.IEditableBusinessObject) Implements Core.IParent.RemoveChild
    ' do nothing, removal of a child is handled by
    ' the RemoveItem override
  End Sub

#End Region

#Region " Cascade Child events "

  Private Sub Child_PropertyChanged(ByVal sender As Object, _
    ByVal e As System.ComponentModel.PropertyChangedEventArgs)

    For index As Integer = 0 To Count - 1
      If ReferenceEquals(Me(index), sender) Then
        OnListChanged(New System.ComponentModel.ListChangedEventArgs( _
          ComponentModel.ListChangedType.ItemChanged, index))
        Exit For
      End If
    Next

  End Sub

#End Region

#Region " Serialization Notification "

  <OnDeserialized()> _
  Private Sub OnDeserializedHandler(ByVal context As StreamingContext)

    OnDeserialized(context)
    For Each child As Core.IEditableBusinessObject In Me
      child.SetParent(Me)
      Dim c As System.ComponentModel.INotifyPropertyChanged = TryCast(child, System.ComponentModel.INotifyPropertyChanged)
      If c IsNot Nothing Then
        AddHandler c.PropertyChanged, AddressOf Child_PropertyChanged
      End If
    Next

  End Sub

  ''' <summary>
  ''' This method is called on a newly deserialized object
  ''' after deserialization is complete.
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub OnDeserialized(ByVal context As StreamingContext)

    ' do nothing - this is here so a subclass
    ' could override if needed

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
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
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

End Class
