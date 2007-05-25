Imports System.ComponentModel
Imports Csla.Core

''' <summary>
''' This is the base class from which most business collections
''' or lists will be derived.
''' </summary>
''' <typeparam name="T">Type of the business object being defined.</typeparam>
''' <typeparam name="C">Type of the child objects contained in the list.</typeparam>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
<Serializable()> _
Public MustInherit Class BusinessListBase( _
  Of T As BusinessListBase(Of T, C), C As {Core.IEditableBusinessObject, Core.IUndoableObject})
  Inherits Core.ExtendedBindingList(Of C)

  Implements Core.IEditableCollection
  Implements Core.IUndoableObject
  Implements ICloneable
  Implements ISavable
  Implements IParent

#Region " Constructors "

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <remarks></remarks>
  Protected Sub New()

    Initialize()

  End Sub

#End Region

#Region " Initialize "

  ''' <summary>
  ''' Override this method to set up event handlers so user
  ''' code in a partial class can respond to events raised by
  ''' generated code.
  ''' </summary>
  Protected Overridable Sub Initialize()
    ' allows a generated class to set up events to be
    ' handled by a partial class containing user code
  End Sub

#End Region

#Region " IsDirty, IsValid, IsSavable  "

  ''' <summary>
  ''' Gets a value indicating whether this object's data has been changed.
  ''' </summary>
  Public ReadOnly Property IsDirty() As Boolean Implements IEditableCollection.IsDirty
    Get
      ' any non-new deletions make us dirty
      For Each item As C In DeletedList
        If Not item.IsNew Then
          Return True
        End If
      Next

      ' run through all the child objects
      ' and if any are dirty then the
      ' collection is dirty
      For Each Child As C In Me
        If Child.IsDirty Then Return True
      Next
      Return False
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether this object is currently in
  ''' a valid state (has no broken validation rules).
  ''' </summary>
  Public Overridable ReadOnly Property IsValid() As Boolean Implements IEditableCollection.IsValid
    Get
      ' run through all the child objects
      ' and if any are invalid then the
      ' collection is invalid
      For Each child As C In Me
        If Not child.IsValid Then Return False
      Next
      Return True
    End Get
  End Property

  ''' <summary>
  ''' Returns <see langword="true" /> if this object is both dirty and valid.
  ''' </summary>
  ''' <returns>A value indicating if this object is both dirty and valid.</returns>
  <Browsable(False)> _
  Public Overridable ReadOnly Property IsSavable() As Boolean Implements IEditableCollection.IsSavable
    Get
      Return (IsDirty AndAlso IsValid)
    End Get
  End Property

#End Region

#Region " Begin/Cancel/ApplyEdit "

  ''' <summary>
  ''' Starts a nested edit on the object.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' When this method is called the object takes a snapshot of
  ''' its current state (the values of its variables). This snapshot
  ''' can be restored by calling <see cref="CancelEdit" />
  ''' or committed by calling <see cref="ApplyEdit" />.
  ''' </para><para>
  ''' This is a nested operation. Each call to BeginEdit adds a new
  ''' snapshot of the object's state to a stack. You should ensure that 
  ''' for each call to BeginEdit there is a corresponding call to either 
  ''' CancelEdit or ApplyEdit to remove that snapshot from the stack.
  ''' </para><para>
  ''' See Chapters 2 and 3 for details on n-level undo and state stacking.
  ''' </para><para>
  ''' This method triggers the copying of all child object states.
  ''' </para>
  ''' </remarks>
  Public Sub BeginEdit() Implements IEditableCollection.BeginEdit
    If Me.IsChild Then
      Throw New  _
        NotSupportedException(My.Resources.NoBeginEditChildException)
    End If

    CopyState(Me.EditLevel + 1)
  End Sub

  ''' <summary>
  ''' Cancels the current edit process, restoring the object's state to
  ''' its previous values.
  ''' </summary>
  ''' <remarks>
  ''' Calling this method causes the most recently taken snapshot of the 
  ''' object's state to be restored. This resets the object's values
  ''' to the point of the last <see cref="BeginEdit" />
  ''' call.
  ''' <para>
  ''' This method triggers an undo in all child objects.
  ''' </para>
  ''' </remarks>
  Public Sub CancelEdit() Implements IEditableCollection.CancelEdit
    If Me.IsChild Then
      Throw New  _
        NotSupportedException(My.Resources.NoCancelEditChildException)
    End If

    UndoChanges(Me.EditLevel - 1)

  End Sub

  ''' <summary>
  ''' Commits the current edit process.
  ''' </summary>
  ''' <remarks>
  ''' Calling this method causes the most recently taken snapshot of the 
  ''' object's state to be discarded, thus committing any changes made
  ''' to the object's state since the last 
  ''' <see cref="BeginEdit" /> call.
  ''' <para>
  ''' This method triggers an <see cref="Core.BusinessBase.ApplyEdit"/>
  '''  in all child objects.
  ''' </para>
  ''' </remarks>
  Public Sub ApplyEdit() Implements IEditableCollection.ApplyEdit
    If Me.IsChild Then
      Throw New  _
        NotSupportedException(My.Resources.NoApplyEditChildException)
    End If

    AcceptChanges(Me.EditLevel - 1)
  End Sub

  ''' <summary>
  ''' Override this method to be notified when a child object's
  ''' <see cref="Core.BusinessBase.ApplyEdit" /> method has
  ''' completed.
  ''' </summary>
  ''' <param name="child">The child object that was edited.</param>
  Protected Overridable Sub EditChildComplete( _
    ByVal child As Core.IEditableBusinessObject) _
    Implements Core.IParent.ApplyEditChild

    ' do nothing, we don't really care
    ' when a child has its edits applied
  End Sub

#End Region

#Region " N-level undo "

  Private Sub CopyState(ByVal parentEditLevel As Integer) Implements Core.IUndoableObject.CopyState
    Dim Child As C

    If Me.EditLevel + 1 > parentEditLevel Then
      Throw New UndoException( _
        String.Format(My.Resources.EditLevelMismatchException, "CopyState"))
    End If

    ' we are going a level deeper in editing
    mEditLevel += 1

    ' cascade the call to all child objects
    For Each Child In Me
      Child.CopyState(mEditLevel)
    Next

    ' cascade the call to all deleted child objects
    For Each Child In DeletedList
      Child.CopyState(mEditLevel)
    Next
  End Sub

  Private mCompletelyRemoveChild As Boolean

  Private Sub UndoChanges(ByVal parentEditLevel As Integer) Implements Core.IUndoableObject.UndoChanges
    Dim child As C
    Dim index As Integer

    If Me.EditLevel - 1 < parentEditLevel Then
      Throw New UndoException( _
        String.Format(My.Resources.EditLevelMismatchException, "UndoChanges"))
    End If

    ' we are coming up one edit level
    mEditLevel -= 1
    If mEditLevel < 0 Then mEditLevel = 0

    ' Cancel edit on all current items
    For index = Count - 1 To 0 Step -1
      child = Me(index)
      child.UndoChanges(mEditLevel)
      ' if item is below its point of addition, remove
      If child.EditLevelAdded > mEditLevel Then
        Dim oldAllowRemove As Boolean = Me.AllowRemove
        Try
          Me.AllowRemove = True
          mCompletelyRemoveChild = True
          RemoveAt(index)
        Finally
          mCompletelyRemoveChild = False
          Me.AllowRemove = oldAllowRemove
        End Try
      End If
    Next

    ' cancel edit on all deleted items
    For index = DeletedList.Count - 1 To 0 Step -1
      child = DeletedList.Item(index)
      child.UndoChanges(mEditLevel)
      If child.EditLevelAdded > mEditLevel Then
        ' if item is below its point of addition, remove
        DeletedList.RemoveAt(index)
      Else
        ' if item is no longer deleted move back to main list
        If Not child.IsDeleted Then UnDeleteChild(child)
      End If
    Next
  End Sub

  Private Sub AcceptChanges(ByVal parentEditLevel As Integer) _
    Implements Core.IUndoableObject.AcceptChanges
    Dim child As C
    Dim index As Integer

    If Me.EditLevel - 1 < parentEditLevel Then
      Throw New UndoException( _
        String.Format(My.Resources.EditLevelMismatchException, "AcceptChanges"))
    End If

    ' we are coming up one edit level
    mEditLevel -= 1
    If mEditLevel < 0 Then mEditLevel = 0

    ' cascade the call to all child objects
    For Each child In Me
      child.AcceptChanges(mEditLevel)
      ' if item is below its point of addition, lower point of addition
      If child.EditLevelAdded > mEditLevel Then child.EditLevelAdded = mEditLevel
    Next

    ' cascade the call to all deleted child objects
    'For Each Child In deletedList
    For index = DeletedList.Count - 1 To 0 Step -1
      child = DeletedList.Item(index)
      child.AcceptChanges(mEditLevel)
      ' if item is below its point of addition, remove
      If child.EditLevelAdded > mEditLevel Then
        DeletedList.RemoveAt(index)
      End If
    Next
  End Sub

#End Region

#Region " Delete and Undelete child "

  Private mDeletedList As List(Of C)

  ''' <summary>
  ''' A collection containing all child objects marked
  ''' for deletion.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected ReadOnly Property DeletedList() As List(Of C)
    Get
      If mDeletedList Is Nothing Then
        mDeletedList = New List(Of C)
      End If
      Return mDeletedList
    End Get
  End Property

  Private Sub DeleteChild(ByVal child As C)
    ' set child edit level
    ResetChildEditLevel(child, Me.EditLevel)
    ' mark the object as deleted
    child.DeleteChild()
    ' and add it to the deleted collection for storage
    DeletedList.Add(child)
  End Sub

  Private Sub UnDeleteChild(ByVal child As C)

    ' since the object is no longer deleted, remove it from
    ' the deleted collection
    DeletedList.Remove(child)

    ' we are inserting an _existing_ object so
    ' we need to preserve the object's editleveladded value
    ' because it will be changed by the normal add process
    Dim SaveLevel As Integer = child.EditLevelAdded
    Add(child)
    child.EditLevelAdded = SaveLevel

  End Sub

  ''' <summary>
  ''' Returns <see langword="true"/> if the internal deleted list
  ''' contains the specified child object.
  ''' </summary>
  ''' <param name="item">Child object to check.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Public Function ContainsDeleted(ByVal item As C) As Boolean

    Return DeletedList.Contains(item)

  End Function

#End Region

#Region " Insert, Remove, Clear "

  ''' <summary>
  ''' This method is called by a child object when it
  ''' wants to be removed from the collection.
  ''' </summary>
  ''' <param name="child">The child object to remove.</param>
  Private Sub RemoveChild(ByVal child As Core.IEditableBusinessObject) _
    Implements Core.IEditableCollection.RemoveChild, IParent.RemoveChild

    Remove(DirectCast(child, C))

  End Sub

  ''' <summary>
  ''' Sets the edit level of the child object as it is added.
  ''' </summary>
  ''' <param name="index">Index of the item to insert.</param>
  ''' <param name="item">Item to insert.</param>
  Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As C)

    ' set parent reference
    item.SetParent(Me)
    ' set child edit level
    ResetChildEditLevel(item, Me.EditLevel)
    ' when an object is inserted we assume it is
    ' a new object and so the edit level when it was
    ' added must be set
    item.EditLevelAdded = mEditLevel
    MyBase.InsertItem(index, item)
  End Sub

  ''' <summary>
  ''' Marks the child object for deletion and moves it to
  ''' the collection of deleted objects.
  ''' </summary>
  ''' <param name="index">Index of the item to remove.</param>
  Protected Overrides Sub RemoveItem(ByVal index As Integer)
    ' when an object is 'removed' it is really
    ' being deleted, so do the deletion work
    Dim child As C = Me(index)
    Dim oldRaiseListChangedEvents As Boolean = _
      Me.RaiseListChangedEvents
    Try
      Me.RaiseListChangedEvents = False
      MyBase.RemoveItem(index)

    Finally
      Me.RaiseListChangedEvents = oldRaiseListChangedEvents
    End Try
    If Not mCompletelyRemoveChild Then
      ' the child shouldn't be completely removed,
      ' so copy it to the deleted list
      CopyToDeletedList(child)
    End If
    OnListChanged(New ListChangedEventArgs(ListChangedType.ItemDeleted, index))
  End Sub

  Private Sub CopyToDeletedList(ByVal child As C)

    DeleteChild(child)
    Dim c As System.ComponentModel.INotifyPropertyChanged = TryCast(child, System.ComponentModel.INotifyPropertyChanged)
    If c IsNot Nothing Then
      RemoveHandler c.PropertyChanged, AddressOf Child_PropertyChanged
    End If

  End Sub

  ''' <summary>
  ''' Clears the collection, moving all active
  ''' items to the deleted list.
  ''' </summary>
  Protected Overrides Sub ClearItems()
    While MyBase.Count > 0
      RemoveItem(0)
    End While
    MyBase.ClearItems()
  End Sub

  ''' <summary>
  ''' Replaces the item at the specified index with
  ''' the specified item, first moving the original
  ''' item to the deleted list.
  ''' </summary>
  ''' <param name="index">The zero-based index of the item to replace.</param>
  ''' <param name="item">
  ''' The new value for the item at the specified index. 
  ''' The value can be null for reference types.
  ''' </param>
  ''' <remarks></remarks>
  Protected Overrides Sub SetItem(ByVal index As Integer, ByVal item As C)
    ' copy the original object to the deleted list,
    ' marking as deleted, etc.
    Dim child As C = Nothing
    If Not ReferenceEquals(DirectCast(Me(index), C), item) Then
      child = Me(index)
    End If
    ' replace the original object with this new
    ' object
    Dim oldRaiseListChangedEvents As Boolean = _
      Me.RaiseListChangedEvents
    Try
      Me.RaiseListChangedEvents = False
      item.SetParent(Me)
      ' set child edit level
      ResetChildEditLevel(item, Me.EditLevel)
      ' reset EditLevelAdded 
      item.EditLevelAdded = Me.EditLevel
      ' add to list
      MyBase.SetItem(index, item)

    Finally
      Me.RaiseListChangedEvents = oldRaiseListChangedEvents
    End Try
    If child IsNot Nothing Then
      CopyToDeletedList(child)
    End If
    OnListChanged(New ListChangedEventArgs(ListChangedType.ItemChanged, index))
  End Sub

  Private Sub ResetChildEditLevel(ByVal child As C, ByVal parentEditLevel As Integer)

    ' if item's edit level is too high,
    ' reduce it to match list
    While child.EditLevel > parentEditLevel
      child.AcceptChanges(parentEditLevel)
    End While
    ' if item's edit level is too low,
    ' increase it to match list
    While child.EditLevel < parentEditLevel
      child.CopyState(parentEditLevel)
    End While

  End Sub

#End Region

#Region " Edit level tracking "

  ' keep track of how many edit levels we have
  Private mEditLevel As Integer

  ''' <summary>
  ''' Returns the current edit level of the object.
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Protected ReadOnly Property EditLevel() As Integer
    Get
      Return mEditLevel
    End Get
  End Property

#End Region

#Region " IsChild "

  <NotUndoable()> _
  Private mIsChild As Boolean = False

  ''' <summary>
  ''' Indicates whether this collection object is a child object.
  ''' </summary>
  ''' <returns>True if this is a child object.</returns>
  Protected ReadOnly Property IsChild() As Boolean
    Get
      Return mIsChild
    End Get
  End Property

  ''' <summary>
  ''' Marks the object as being a child object.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' By default all business objects are 'parent' objects. This means
  ''' that they can be directly retrieved and updated into the database.
  ''' </para><para>
  ''' We often also need child objects. These are objects which are contained
  ''' within other objects. For instance, a parent Invoice object will contain
  ''' child LineItem objects.
  ''' </para><para>
  ''' To create a child object, the MarkAsChild method must be called as the
  ''' object is created. Please see Chapter 7 for details on the use of the
  ''' MarkAsChild method.
  ''' </para>
  ''' </remarks>
  Protected Sub MarkAsChild()
    mIsChild = True
  End Sub

#End Region

#Region " ICloneable "

  Private Function ICloneable_Clone() As Object Implements ICloneable.Clone

    Return GetClone()

  End Function

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Function GetClone() As Object

    Return ObjectCloner.Clone(Me)

  End Function

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  Public Overloads Function Clone() As T

    Return DirectCast(GetClone(), T)

  End Function

#End Region

#Region " Cascade Child events "

  Private Sub Child_PropertyChanged(ByVal sender As Object, _
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
    For Each child As Core.IEditableBusinessObject In DeletedList
      child.SetParent(Me)
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

  ''' <summary>
  ''' Saves the object to the database.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' Calling this method starts the save operation, causing the all child
  ''' objects to be inserted, updated or deleted within the database based on the
  ''' each object's current state.
  ''' </para><para>
  ''' All this is contingent on <see cref="IsDirty" />. If
  ''' this value is <see langword="false"/>, no data operation occurs. 
  ''' It is also contingent on <see cref="IsValid" />. If this value is 
  ''' <see langword="false"/> an exception will be thrown to 
  ''' indicate that the UI attempted to save an invalid object.
  ''' </para><para>
  ''' It is important to note that this method returns a new version of the
  ''' business collection that contains any data updated during the save operation.
  ''' You MUST update all object references to use this new version of the
  ''' business collection in order to have access to the correct object data.
  ''' </para><para>
  ''' You can override this method to add your own custom behaviors to the save
  ''' operation. For instance, you may add some security checks to make sure
  ''' the user can save the object. If all security checks pass, you would then
  ''' invoke the base Save method via <c>MyBase.Save()</c>.
  ''' </para>
  ''' </remarks>
  ''' <returns>A new object containing the saved values.</returns>
  Public Overridable Function Save() As T

    If Me.IsChild Then
      Throw New NotSupportedException(My.Resources.NoSaveChildException)
    End If

    If mEditLevel > 0 Then
      Throw New Validation.ValidationException(My.Resources.NoSaveEditingException)
    End If

    If Not IsValid Then
      Throw New Validation.ValidationException(My.Resources.NoSaveInvalidException)
    End If

    Dim result As T
    If IsDirty Then
      result = DirectCast(DataPortal.Update(Me), T)

    Else
      result = DirectCast(Me, T)
    End If

    OnSaved(result)
    Return result

  End Function

  ''' <summary>
  ''' Override this method to load a new business object with default
  ''' values from the database.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  Protected Overridable Sub DataPortal_Create()
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

  ''' <summary>
  ''' Override this method to allow insert, update or deletion of a business
  ''' object.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  Protected Overridable Sub DataPortal_Update()
    Throw New NotSupportedException(My.Resources.UpdateNotSupportedException)
  End Sub

  ''' <summary>
  ''' Override this method to allow immediate deletion of a business object.
  ''' </summary>
  ''' <param name="criteria">An object containing criteria values to identify the object.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  Protected Overridable Sub DataPortal_Delete(ByVal criteria As Object)
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

#Region " ISavable implementation "

  Private Function ISavable_Save() As Object Implements ISavable.Save
    Return Save()
  End Function

  <NonSerialized()> _
  <NotUndoable()> _
  Private mNonSerializableSavedHandlers As EventHandler(Of Csla.Core.SavedEventArgs)
  <NotUndoable()> _
  Private mSerializableSavedHandlers As EventHandler(Of Csla.Core.SavedEventArgs)

  ''' <summary>
  ''' Event raised when an object has been saved.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  Public Custom Event Saved As EventHandler(Of Csla.Core.SavedEventArgs) Implements Core.ISavable.Saved
    AddHandler(ByVal value As EventHandler(Of Csla.Core.SavedEventArgs))
      If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
        mSerializableSavedHandlers = CType(System.Delegate.Combine(mSerializableSavedHandlers, value), EventHandler(Of Csla.Core.SavedEventArgs))
      Else
        mNonSerializableSavedHandlers = CType(System.Delegate.Combine(mNonSerializableSavedHandlers, value), EventHandler(Of Csla.Core.SavedEventArgs))
      End If
    End AddHandler
    RemoveHandler(ByVal value As EventHandler(Of Csla.Core.SavedEventArgs))
      If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
        mSerializableSavedHandlers = CType(System.Delegate.Remove(mSerializableSavedHandlers, value), EventHandler(Of Csla.Core.SavedEventArgs))
      Else
        mNonSerializableSavedHandlers = CType(System.Delegate.Remove(mNonSerializableSavedHandlers, value), EventHandler(Of Csla.Core.SavedEventArgs))
      End If
    End RemoveHandler
    RaiseEvent(ByVal sender As System.Object, ByVal e As Csla.Core.SavedEventArgs)
      If Not mNonSerializableSavedHandlers Is Nothing Then
        mNonSerializableSavedHandlers.Invoke(Me, e)
      End If
      If Not mSerializableSavedHandlers Is Nothing Then
        mSerializableSavedHandlers.Invoke(Me, e)
      End If
    End RaiseEvent
  End Event

  ''' <summary>
  ''' Raises the <see cref="Saved"/> event, indicating that the
  ''' object has been saved, and providing a reference
  ''' to the new object instance.
  ''' </summary>
  ''' <param name="newObject">The new object instance.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Sub OnSaved(ByVal newObject As T)

    RaiseEvent Saved(Me, New Csla.Core.SavedEventArgs(newObject))

  End Sub

#End Region

End Class
