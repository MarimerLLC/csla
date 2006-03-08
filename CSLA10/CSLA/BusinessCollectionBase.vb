Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration

''' <summary>
''' This is the base class from which most business collection
''' objects will be derived.
''' </summary>
''' <remarks>
''' <para>
''' To create a collection of business objects, inherit from this 
''' class. The business objects contained in this collection must
''' inherit from <see cref="T:CSLA.BusinessBase" />, and the objects
''' must be marked as child objects.
''' </para><para>
''' Please refer to 'Expert One-on-One VB.NET Business Objects' for
''' full details on the use of this base class to create business
''' collections.
''' </para>
''' </remarks>
<Serializable()> _
Public MustInherit Class BusinessCollectionBase
  Inherits CSLA.Core.SortableCollectionBase

  Implements ICloneable
  Implements Serialization.ISerializationNotification

#Region " Contains "

  ''' <summary>
  ''' Used to see if the collection contains a specific child object.
  ''' </summary>
  ''' <remarks>
  ''' Only the 'active' list of child objects is checked. 
  ''' Business collections also contain deleted objects, which are
  ''' not checked by this call.
  ''' </remarks>
  ''' <param name="Item">A reference to the object.</param>
  ''' <returns>True if the collection contains the object.</returns>
  Public Function Contains(ByVal Item As BusinessBase) As Boolean
    'Return list.Contains(Item)
    Dim element As BusinessBase

    For Each element In list
      If element.Equals(Item) Then
        Return True
      End If
    Next
    Return False

  End Function

  ''' <summary>
  ''' Used to see if the collection contains a reference to a
  ''' child object that is marked for deletion.
  ''' </summary>
  ''' <remarks>
  ''' This scans the list of child objects that have been marked
  ''' for deletion. If this object is in that list, the method
  ''' returns True.
  ''' </remarks>
  ''' <param name="Item">A reference to the object.</param>
  ''' <returns>True if the collection contains the object.</returns>
  Public Function ContainsDeleted(ByVal Item As BusinessBase) As Boolean
    Dim element As BusinessBase

    For Each element In deletedList
      If element.Equals(Item) Then
        Return True
      End If
    Next
    Return False
  End Function

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
      If deletedList.Count > 0 Then Return True

      ' run through all the child objects
      ' and if any are dirty then the
      ' collection is dirty
      Dim Child As BusinessBase

      For Each Child In list
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
  ''' By default this property relies on the underling <see cref="T:CSLA.BrokenRules" />
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
  Public ReadOnly Property IsValid() As Boolean
    Get
      ' run through all the child objects
      ' and if any are invalid then the
      ' collection is invalid
      Dim Child As BusinessBase

      For Each Child In list
        If Not Child.IsValid Then Return False
      Next
      Return True
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
  ''' can be restored by calling <see cref="M:CSLA.BusinessBase.CancelEdit" />
  ''' or committed by calling <see cref="M:CSLA.BusinessBase.ApplyEdit" />.
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
        NotSupportedException(GetResourceString("NoBeginEditChildException"))
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
  ''' to the point of the last <see cref="M:CSLA.BusinessCollectionBase.BeginEdit" />
  ''' call.
  ''' <para>
  ''' This method triggers an undo in all child objects.
  ''' </para>
  ''' </remarks>
  Public Sub CancelEdit()
    If Me.IsChild Then
      Throw New _
        NotSupportedException(GetResourceString("NoCancelEditChildException"))
    End If

    UndoChanges()

  End Sub

  ''' <summary>
  ''' Commits the current edit process.
  ''' </summary>
  ''' <remarks>
  ''' Calling this method causes the most recently taken snapshot of the 
  ''' object's state to be discarded, thus committing any changes made
  ''' to the object's state since the last 
  ''' <see cref="M:CSLA.BusinessCollectionBase.BeginEdit" /> call.
  ''' <para>
  ''' This method triggers an ApplyEdit in all child objects.
  ''' </para>
  ''' </remarks>
  Public Sub ApplyEdit()
    If Me.IsChild Then
      Throw New _
        NotSupportedException(GetResourceString("NoApplyEditChildException"))
    End If

    AcceptChanges()
  End Sub

#End Region

#Region " N-level undo "

  Friend Sub CopyState()
    Dim Child As BusinessBase

    ' we are going a level deeper in editing
    mEditLevel += 1

    ' cascade the call to all child objects
    For Each Child In list
      Child.CopyState()
    Next

    ' cascade the call to all deleted child objects
    For Each Child In deletedList
      Child.CopyState()
    Next
  End Sub

  Friend Sub UndoChanges()
    Dim Child As BusinessBase
    Dim Index As Integer

    ' we are coming up one edit level
    mEditLevel -= 1
    If mEditLevel < 0 Then mEditLevel = 0

    ' Cancel edit on all current items
    For Index = List.Count - 1 To 0 Step -1
      Child = CType(list.Item(Index), BusinessBase)
      Child.UndoChanges()
      ' if item is below its point of addition, remove
      If Child.EditLevelAdded > mEditLevel Then
        'list.Remove(Child)
        list.RemoveAt(Index)
      End If
    Next

    ' cancel edit on all deleted items
    For Index = deletedList.Count - 1 To 0 Step -1
      Child = deletedList.Item(Index)
      Child.UndoChanges()
      If Child.EditLevelAdded > mEditLevel Then
        ' if item is below its point of addition, remove
        'deletedList.Remove(Child)
        deletedList.RemoveAt(Index)
      Else
        ' if item is no longer deleted move back to main list
        If Not Child.IsDeleted Then UnDeleteChild(Child)
      End If
    Next
  End Sub

  Friend Sub AcceptChanges()
    Dim Child As BusinessBase
    Dim Index As Integer

    ' we are coming up one edit level
    mEditLevel -= 1
    If mEditLevel < 0 Then mEditLevel = 0

    ' cascade the call to all child objects
    For Each Child In list
      Child.AcceptChanges()
      ' if item is below its point of addition, lower point of addition
      If Child.EditLevelAdded > mEditLevel Then Child.EditLevelAdded = mEditLevel
    Next

    ' cascade the call to all deleted child objects
    'For Each Child In deletedList
    For Index = deletedList.Count - 1 To 0 Step -1
      Child = deletedList.Item(Index)
      Child.AcceptChanges()
      '' if item is below its point of addition, lower point of addition
      'If Child.EditLevelAdded > mEditLevel Then Child.EditLevelAdded = mEditLevel
      ' if item is below its point of addition, remove
      If Child.EditLevelAdded > mEditLevel Then
        'deletedList.Remove(Child)
        deletedList.RemoveAt(Index)
      End If
    Next
  End Sub

#End Region

#Region " Delete and Undelete child "

  Private Sub DeleteChild(ByVal Child As BusinessBase)
    ' mark the object as deleted
    Child.DeleteChild()
    ' and add it to the deleted collection for storage
    deletedList.Add(Child)
  End Sub

  Private Sub UnDeleteChild(ByVal Child As BusinessBase)
    ' we are inserting an _existing_ object so
    ' we need to preserve the object's editleveladded value
    ' because it will be changed by the normal add process
    Dim SaveLevel As Integer = Child.EditLevelAdded
    list.Add(Child)
    Child.EditLevelAdded = SaveLevel

    ' since the object is no longer deleted, remove it from
    ' the deleted collection
    deletedList.Remove(Child)
  End Sub

#End Region

#Region " DeletedCollection "

  ''' <summary>
  ''' A collection containing all child objects marked
  ''' for deletion.
  ''' </summary>
  Protected deletedList As New DeletedCollection()

  ''' <summary>
  ''' Defines a strongly-typed collection to store all
  ''' child objects marked for deletion.
  ''' </summary>
  <Serializable()> _
  Protected Class DeletedCollection
    Inherits CollectionBase

    ''' <summary>
    ''' Adds a child object to the collection.
    ''' </summary>
    ''' <param name="Child">The child object to be added.</param>
    Public Sub Add(ByVal Child As BusinessBase)
      list.Add(Child)
    End Sub

    ''' <summary>
    ''' Removes a child object from the collection.
    ''' </summary>
    ''' <param name="Child">The child object to be removed.</param>
    Public Sub Remove(ByVal Child As BusinessBase)
      list.Remove(Child)
    End Sub

    ''' <summary>
    ''' Returns a reference to a child object in the collection.
    ''' </summary>
    ''' <param name="index">The positional index of the item in the collection.</param>
    ''' <returns>The specified child object.</returns>
    Default Public ReadOnly Property Item(ByVal index As Integer) As BusinessBase
      Get
        Return CType(list.Item(index), BusinessBase)
      End Get
    End Property
  End Class

#End Region

#Region " OnIsDirtyChanged Event Handling "

  Private Sub OnChildIsDirty(ByVal sender As Object, ByVal e As EventArgs)

    Dim pos As Integer = 0
    For Each item As BusinessBase In List
      If ReferenceEquals(sender, item) Then
        Exit For
      End If
      pos += 1
    Next
    Me.OnListChanged(New System.ComponentModel.ListChangedEventArgs( _
      System.ComponentModel.ListChangedType.ItemChanged, pos))

  End Sub

#End Region

#Region " Insert, Remove, Clear "

      ''' <summary>
      ''' This method is called by a child object when it
      ''' wants to be removed from the collection.
      ''' </summary>
      ''' <param name="child">The child object to remove.</param>
  Friend Sub RemoveChild(ByVal child As BusinessBase)
    list.Remove(child)
  End Sub

  ''' <summary>
  ''' Sets the edit level of the child object as it is added.
  ''' </summary>
  Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)
    If Not ActivelySorting Then
      ' when an object is inserted we assume it is
      ' a new object and so the edit level when it was
      ' added must be set
      CType(value, BusinessBase).EditLevelAdded = mEditLevel
      CType(value, BusinessBase).SetParent(Me)
      AddHandler CType(value, BusinessBase).IsDirtyChanged, AddressOf OnChildIsDirty
      MyBase.OnInsert(index, value)
    End If
  End Sub

  ''' <summary>
  ''' Marks the child object for deletion and moves it to
  ''' the collection of deleted objects.
  ''' </summary>
  Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
    If Not ActivelySorting Then
      ' when an object is 'removed' it is really
      ' being deleted, so do the deletion work
      DeleteChild(CType(value, BusinessBase))
      RemoveHandler CType(value, BusinessBase).IsDirtyChanged, AddressOf OnChildIsDirty
      MyBase.OnRemove(index, value)
    End If
  End Sub

  ''' <summary>
  ''' Marks all child objects for deletion and moves them
  ''' to the collection of deleted objects.
  ''' </summary>
  Protected Overrides Sub OnClear()
    If Not ActivelySorting Then
      ' when an object is 'removed' it is really
      ' being deleted, so do the deletion work
      ' for all the objects in the list
      While list.Count > 0
        list.RemoveAt(list.Count - 1)
      End While
      MyBase.OnClear()
    End If
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
  Public Function Clone() As Object Implements ICloneable.Clone

    Dim buffer As New MemoryStream
    Dim formatter As New BinaryFormatter

    Serialization.SerializationNotification.OnSerializing(Me)
    formatter.Serialize(buffer, Me)
    Serialization.SerializationNotification.OnSerialized(Me)
    buffer.Position = 0
    Dim temp As Object = formatter.Deserialize(buffer)
    Serialization.SerializationNotification.OnDeserialized(temp)
    Return temp

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
  ''' All this is contingent on <see cref="P:CSLA.BusinessCollectionBase.IsDirty" />. If
  ''' this value is False, no data operation occurs. It is also contingent on
  ''' <see cref="P:CSLA.BusinessCollectionBase.IsValid" />. If this value is False an
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
  Public Overridable Function Save() As BusinessCollectionBase
    If Me.IsChild Then
      Throw New NotSupportedException(GetResourceString("NoSaveChildException"))
    End If

    If mEditLevel > 0 Then
      Throw New Exception(GetResourceString("NoSaveEditingException"))
    End If

    If Not IsValid Then
      Throw New Exception(GetResourceString("NoSaveInvalidException"))
    End If

    If IsDirty Then
      Return CType(DataPortal.Update(Me), BusinessCollectionBase)
    Else
      Return Me
    End If

  End Function

  ''' <summary>
  ''' Override this method to load a new business object with default
  ''' values from the database.
  ''' </summary>
  ''' <param name="Criteria">An object containing criteria values.</param>
  Protected Overridable Sub DataPortal_Create(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("CreateNotSupportedException"))
  End Sub

  ''' <summary>
  ''' Override this method to allow retrieval of an existing business
  ''' object based on data in the database.
  ''' </summary>
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
  Protected Overridable Sub DataPortal_Fetch(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("FetchNotSupportedException"))
  End Sub

  ''' <summary>
  ''' Override this method to allow insert, update or deletion of a business
  ''' object.
  ''' </summary>
  Protected Overridable Sub DataPortal_Update()
    Throw New NotSupportedException(GetResourceString("UpdateNotSupportedException"))
  End Sub

  ''' <summary>
  ''' Override this method to allow immediate deletion of a business object.
  ''' </summary>
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
  Protected Overridable Sub DataPortal_Delete(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("DeleteNotSupportedException"))
  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  Protected Overridable Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Returns the specified database connection string from the application
  ''' configuration file.
  ''' </summary>
  ''' <remarks>
  ''' The database connection string must be in the <c>appSettings</c> section
  ''' of the application configuration file. The database name should be
  ''' prefixed with 'DB:'. For instance, <c>DB:mydatabase</c>.
  ''' </remarks>
  ''' <param name="DatabaseName">Name of the database.</param>
  ''' <returns>A database connection string.</returns>
  Protected Function DB(ByVal DatabaseName As String) As String
    Return ConfigurationSettings.AppSettings("DB:" & DatabaseName)
  End Function

#End Region

#Region " DumpState "

  Friend Sub DumpState()
    Dim Child As BusinessBase

    Debug.WriteLine("BusinessCollectionBase!Count:" & list.Count)
    Debug.WriteLine("BusinessCollectionBase!DeletedCount:" & deletedList.Count)
    Debug.WriteLine("BusinessCollectionBase!mIsChild:" & mIsChild)
    Debug.WriteLine("BusinessCollectionBase!mEditLevel:" & mEditLevel)
    Debug.Indent()

    For Each Child In list
      Child.DumpState()
    Next
    Debug.Unindent()

  End Sub

#End Region

#Region " ISerializationNotification "

  ''' <summary>
  ''' This method is called on a newly deserialized object
  ''' after deserialization is complete.
  ''' </summary>
  Protected Overridable Sub Deserialized() _
    Implements CSLA.Serialization.ISerializationNotification.Deserialized

    For Each child As Serialization.ISerializationNotification In list
      child.Deserialized()
      DirectCast(child, BusinessBase).SetParent(Me)
      AddHandler CType(child, BusinessBase).IsDirtyChanged, AddressOf OnChildIsDirty
    Next
    For Each child As Serialization.ISerializationNotification In deletedList
      child.Deserialized()
      DirectCast(child, BusinessBase).SetParent(Me)
    Next
  End Sub

  ''' <summary>
  ''' This method is called on the original instance of the
  ''' object after it has been serialized.
  ''' </summary>
  Protected Overridable Sub Serialized() _
    Implements CSLA.Serialization.ISerializationNotification.Serialized

    Dim child As Serialization.ISerializationNotification
    For Each child In list
      child.Serialized()
    Next
    For Each child In deletedList
      child.Serialized()
    Next
  End Sub

  ''' <summary>
  ''' This method is called before an object is serialized.
  ''' </summary>
  Protected Overridable Sub Serializing() _
    Implements CSLA.Serialization.ISerializationNotification.Serializing

    Dim child As Serialization.ISerializationNotification
    For Each child In list
      child.Serializing()
    Next
    For Each child In deletedList
      child.Serializing()
    Next
  End Sub

#End Region

End Class
