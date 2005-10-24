Imports System.ComponentModel

<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
<Serializable()> _
Public MustInherit Class BusinessListBase(Of T As Core.BusinessBase)
  Inherits System.ComponentModel.BindingList(Of T)

  Implements Core.IEditableCollection
  Implements ICloneable

#Region " Constructors "

  Protected Sub New()

  End Sub

#End Region

#Region " IsDirty, IsValid "

  ''' <summary>
  ''' Returns True if this object's data has been changed.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' When an object's data is changed, CSLA .NET makes note of that change
  ''' and considers the object to be 'dirty' or changed. This value is used to
  ''' optimize data updates, since an unchanged object does not need to be
  ''' updated into the database. All new objects are considered dirty. All objects
  ''' marked for deletion are considered dirty.
  ''' </para><para>
  ''' Once an object's data has been saved to the database (inserted or updated)
  ''' the dirty flag is cleared and the object is considered unchanged. Objects
  ''' newly loaded from the database are also considered unchanged.
  ''' </para>
  ''' <para>
  ''' If any child object within the collection is dirty then the collection
  ''' is considered to be dirty. If all child objects are unchanged, then the
  ''' collection is not dirty.
  ''' </para>
  ''' </remarks>
  ''' <returns>A value indicating if this object's data has been changed.</returns>
  Public ReadOnly Property IsDirty() As Boolean
    Get
      ' any deletions make us dirty
      If DeletedList.Count > 0 Then Return True

      ' run through all the child objects
      ' and if any are dirty then the
      ' collection is dirty
      Dim Child As T

      For Each Child In Me
        If Child.IsDirty Then Return True
      Next
      Return False
    End Get
  End Property

  ''' <summary>
  ''' Returns True if the object is currently valid, False if the
  ''' object has broken rules or is otherwise invalid.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' By default this property relies on the underling <see cref="T:Csla.BrokenRules" />
  ''' object to track whether any business rules are currently broken for this object.
  ''' </para><para>
  ''' You can override this property to provide more sophisticated
  ''' implementations of the behavior. For instance, you should always override
  ''' this method if your object has child objects, since the validity of this object
  ''' is affected by the validity of all child objects.
  ''' </para>
  ''' <para>
  ''' If any child object within the collection is invalid then the collection
  ''' is considered to be invalid. If all child objects are valid, then the
  ''' collection is valid.
  ''' </para>
  ''' </remarks>
  ''' <returns>A value indicating if the object is currently valid.</returns>
  Public Overridable ReadOnly Property IsValid() As Boolean
    Get
      ' run through all the child objects
      ' and if any are invalid then the
      ' collection is invalid
      For Each child As T In Me
        If Not child.IsValid Then Return False
      Next
      Return True
    End Get
  End Property

  ''' <summary>
  ''' Returns a String indicating why the collection
  ''' is not currently valid.
  ''' </summary>
  ''' <remarks>
  ''' This property should return an empty string
  ''' unless IsValid is returning False.
  ''' </remarks>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Public Overridable ReadOnly Property ErrorText() As String
    Get
      If IsValid Then
        Return ""

      Else
        Return "One or more child objects are in " & _
          "an invalid state"
      End If
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
  ''' can be restored by calling <see cref="M:Csla.Core.BusinessBase.CancelEdit" />
  ''' or committed by calling <see cref="M:Csla.Core.BusinessBase.ApplyEdit" />.
  ''' </para><para>
  ''' This is a nested operation. Each call to BeginEdit adds a new
  ''' snapshot of the object's state to a stack. You should ensure that 
  ''' for each call to BeginEdit there is a corresponding call to either 
  ''' CancelEdit or ApplyEdit to remove that snapshot from the stack.
  ''' </para><para>
  ''' See Chapters 2 and 4 for details on n-level undo and state stacking.
  ''' </para><para>
  ''' This method triggers the copying of all child object states.
  ''' </para>
  ''' </remarks>
  Public Sub BeginEdit()
    If Me.IsChild Then
      Throw New _
        NotSupportedException(My.Resources.NoBeginEditChildException)
    End If

    CopyState()
  End Sub

  ''' <summary>
  ''' Cancels the current edit process, restoring the object's state to
  ''' its previous values.
  ''' </summary>
  ''' <remarks>
  ''' Calling this method causes the most recently taken snapshot of the 
  ''' object's state to be restored. This resets the object's values
  ''' to the point of the last <see cref="M:Csla.BusinessCollectionBase.BeginEdit" />
  ''' call.
  ''' <para>
  ''' This method triggers an undo in all child objects.
  ''' </para>
  ''' </remarks>
  Public Sub CancelEdit()
    If Me.IsChild Then
      Throw New _
        NotSupportedException(My.Resources.NoCancelEditChildException)
    End If

    UndoChanges()

    ' make sure the child objects re-add their business rules
    For Each child As T In Me
      child.AddBusinessRules()
    Next
    For Each child As T In DeletedList
      child.AddBusinessRules()
    Next

  End Sub

  ''' <summary>
  ''' Commits the current edit process.
  ''' </summary>
  ''' <remarks>
  ''' Calling this method causes the most recently taken snapshot of the 
  ''' object's state to be discarded, thus committing any changes made
  ''' to the object's state since the last 
  ''' <see cref="M:Csla.BusinessCollectionBase.BeginEdit" /> call.
  ''' <para>
  ''' This method triggers an ApplyEdit in all child objects.
  ''' </para>
  ''' </remarks>
  Public Sub ApplyEdit()
    If Me.IsChild Then
      Throw New _
        NotSupportedException(My.Resources.NoApplyEditChildException)
    End If

    AcceptChanges()
  End Sub

#End Region

#Region " N-level undo "

  Friend Sub CopyState() Implements Core.IEditableCollection.CopyState
    Dim Child As T

    ' we are going a level deeper in editing
    mEditLevel += 1

    ' cascade the call to all child objects
    For Each Child In Me
      Child.CopyState()
    Next

    ' cascade the call to all deleted child objects
    For Each Child In DeletedList
      Child.CopyState()
    Next
  End Sub

  Friend Sub UndoChanges() Implements Core.IEditableCollection.UndoChanges
    Dim child As T
    Dim index As Integer

    ' we are coming up one edit level
    mEditLevel -= 1
    If mEditLevel < 0 Then mEditLevel = 0

    ' Cancel edit on all current items
    For index = Count - 1 To 0 Step -1
      child = Me(index)
      child.UndoChanges()
      ' if item is below its point of addition, remove
      If child.EditLevelAdded > mEditLevel Then
        'list.Remove(Child)
        RemoveAt(index)
      End If
    Next

    ' cancel edit on all deleted items
    For index = DeletedList.Count - 1 To 0 Step -1
      child = DeletedList.Item(index)
      child.UndoChanges()
      If child.EditLevelAdded > mEditLevel Then
        ' if item is below its point of addition, remove
        DeletedList.RemoveAt(index)
      Else
        ' if item is no longer deleted move back to main list
        If Not child.IsDeleted Then UnDeleteChild(child)
      End If
    Next
  End Sub

  Friend Sub AcceptChanges() Implements Core.IEditableCollection.AcceptChanges
    Dim child As T
    Dim index As Integer

    ' we are coming up one edit level
    mEditLevel -= 1
    If mEditLevel < 0 Then mEditLevel = 0

    ' cascade the call to all child objects
    For Each child In Me
      child.AcceptChanges()
      ' if item is below its point of addition, lower point of addition
      If child.EditLevelAdded > mEditLevel Then child.EditLevelAdded = mEditLevel
    Next

    ' cascade the call to all deleted child objects
    'For Each Child In deletedList
    For index = DeletedList.Count - 1 To 0 Step -1
      child = DeletedList.Item(index)
      child.AcceptChanges()
      ' if item is below its point of addition, remove
      If child.EditLevelAdded > mEditLevel Then
        DeletedList.RemoveAt(index)
      End If
    Next
  End Sub

#End Region

#Region " Delete and Undelete child "

  Private mDeletedList As New List(Of T)

  ''' <summary>
  ''' A collection containing all child objects marked
  ''' for deletion.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected ReadOnly Property DeletedList() As List(Of T)
    Get
      Return mDeletedList
    End Get
  End Property

  Private Sub DeleteChild(ByVal child As T)
    ' mark the object as deleted
    child.DeleteChild()
    ' and add it to the deleted collection for storage
    DeletedList.Add(child)
  End Sub

  Private Sub UnDeleteChild(ByVal child As T)
    ' we are inserting an _existing_ object so
    ' we need to preserve the object's editleveladded value
    ' because it will be changed by the normal add process
    Dim SaveLevel As Integer = child.EditLevelAdded
    Add(child)
    child.EditLevelAdded = SaveLevel

    ' since the object is no longer deleted, remove it from
    ' the deleted collection
    DeletedList.Remove(child)
  End Sub

#End Region

  ' commented out because BindingList(Of T) automatically
  ' cascades the child events
  '#Region " Cascade Child events "

  '  Private Sub Child_PropertyChanged(ByVal sender As Object, _
  '    ByVal e As System.ComponentModel.PropertyChangedEventArgs)

  '    For index As Integer = 0 To Count - 1
  '      If ReferenceEquals(Me(index), sender) Then
  '        OnListChanged(New System.ComponentModel.ListChangedEventArgs( _
  '          ComponentModel.ListChangedType.ItemChanged, index))
  '        Exit For
  '      End If
  '    Next

  '  End Sub

  '#End Region

#Region " Insert, Remove, Clear "

  ''' <summary>
  ''' This method is called by a child object when it
  ''' wants to be removed from the collection.
  ''' </summary>
  ''' <param name="child">The child object to remove.</param>
  Private Sub RemoveChild(ByVal child As Core.BusinessBase) _
    Implements Core.IEditableCollection.RemoveChild

    Remove(DirectCast(child, T))

  End Sub

  ''' <summary>
  ''' Sets the edit level of the child object as it is added.
  ''' </summary>
  Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As T)
    ' when an object is inserted we assume it is
    ' a new object and so the edit level when it was
    ' added must be set
    item.EditLevelAdded = mEditLevel
    item.SetParent(Me)
    'AddHandler item.PropertyChanged, _
    '  AddressOf Child_PropertyChanged
    MyBase.InsertItem(index, item)
  End Sub

  ''' <summary>
  ''' Marks the child object for deletion and moves it to
  ''' the collection of deleted objects.
  ''' </summary>
  Protected Overrides Sub RemoveItem(ByVal index As Integer)
    ' when an object is 'removed' it is really
    ' being deleted, so do the deletion work
    Dim item As T = Me(index)
    DeleteChild(item)
    'RemoveHandler item.PropertyChanged, _
    '  AddressOf Child_PropertyChanged
    MyBase.RemoveItem(index)
  End Sub

  ''' <summary>
  ''' Marks all child objects for deletion and moves them
  ''' to the collection of deleted objects.
  ''' </summary>
  Protected Overrides Sub ClearItems()
    ' when an object is 'removed' it is really
    ' being deleted, so do the deletion work
    ' for all the objects in the list
    While Count > 0
      RemoveAt(Count - 1)
    End While
    MyBase.ClearItems()
  End Sub

#End Region

#Region " Edit level tracking "

  ' keep track of how many edit levels we have
  Private mEditLevel As Integer

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

#Region " Clone "

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>A new object containing the exact data of the original object.</returns>
  Public Overridable Function Clone() As Object Implements ICloneable.Clone

    Return ObjectCloner.Clone(Me)

  End Function

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
  ''' All this is contingent on <see cref="P:Csla.BusinessCollectionBase.IsDirty" />. If
  ''' this value is False, no data operation occurs. It is also contingent on
  ''' <see cref="P:Csla.BusinessCollectionBase.IsValid" />. If this value is False an
  ''' exception will be thrown to indicate that the UI attempted to save an
  ''' invalid object.
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
  Public Overridable Function Save() As BusinessListBase(Of T)
    If Me.IsChild Then
      Throw New NotSupportedException(My.Resources.NoSaveChildException)
    End If

    If mEditLevel > 0 Then
      Throw New Validation.ValidationException(My.Resources.NoSaveEditingException)
    End If

    If Not IsValid Then
      Throw New Validation.ValidationException(My.Resources.NoSaveInvalidException)
    End If

    If IsDirty Then
      Return CType(DataPortal.Update(Me), BusinessListBase(Of T))
    Else
      Return Me
    End If

  End Function

  ''' <summary>
  ''' Override this method to load a new business object with default
  ''' values from the database.
  ''' </summary>
  ''' <param name="Criteria">An object containing criteria values.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  Protected Overridable Sub DataPortal_Create(ByVal criteria As Object)
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
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
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

End Class
