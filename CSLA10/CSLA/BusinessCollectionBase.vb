Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration

<Serializable()> _
Public MustInherit Class BusinessCollectionBase
  Inherits CSLA.Core.BindableCollectionBase

  Implements ICloneable

#Region " Contains "

  Public Function Contains(ByVal Item As BusinessBase) As Boolean
    Return list.Contains(Item)
  End Function

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

  Public Sub BeginEdit()
    If Me.IsChild Then
      Throw New _
        NotSupportedException("BeginEdit is not valid on a child object")
    End If

    CopyState()
  End Sub

  Public Sub CancelEdit()
    If Me.IsChild Then
      Throw New _
        NotSupportedException("CancelEdit is not valid on a child object")
    End If

    UndoChanges()
  End Sub

  Public Sub ApplyEdit()
    If Me.IsChild Then
      Throw New _
        NotSupportedException("ApplyEdit is not valid on a child object")
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
      If Child.EditLevelAdded > mEditLevel Then list.Remove(Child)
    Next

    ' cancel edit on all deleted items
    For Index = deletedList.Count - 1 To 0 Step -1
      Child = deletedList.Item(Index)
      Child.UndoChanges()
      ' if item is below its point of addition, remove
      If Child.EditLevelAdded > mEditLevel Then deletedList.Remove(Child)
      ' if item is no longer deleted move back to main list
      If Not Child.IsDeleted Then UnDeleteChild(Child)
    Next
  End Sub

  Friend Sub AcceptChanges()
    Dim Child As BusinessBase

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
    For Each Child In deletedList
      Child.AcceptChanges()
      ' if item is below its point of addition, lower point of addition
      If Child.EditLevelAdded > mEditLevel Then Child.EditLevelAdded = mEditLevel
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

  ' here's the list of deleted child objects
  Protected deletedList As New DeletedCollection

  ' this is a simple collection to store all of
  ' the child objects that get deleted
  <Serializable()> _
  Protected Class DeletedCollection
    Inherits CollectionBase

    Public Sub Add(ByVal Child As BusinessBase)
      list.Add(Child)
    End Sub

    Public Sub Remove(ByVal Child As BusinessBase)
      list.Remove(Child)
    End Sub

    Default Public ReadOnly Property Item(ByVal index As Integer) As BusinessBase
      Get
        Return CType(list.Item(index), BusinessBase)
      End Get
    End Property
  End Class

#End Region

#Region " Insert, Remove, Clear "

  Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)
    ' when an object is inserted we assume it is
    ' a new object and so the edit level when it was
    ' added must be set
    CType(value, BusinessBase).EditLevelAdded = mEditLevel
  End Sub

  Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
    ' when an object is 'removed' it is really
    ' being deleted, so do the deletion work
    DeleteChild(CType(value, BusinessBase))
  End Sub

  Protected Overrides Sub OnClear()
    ' when an object is 'removed' it is really
    ' being deleted, so do the deletion work
    ' for all the objects in the list
    While list.Count > 0
      DeleteChild(CType(list(0), BusinessBase))
    End While
  End Sub

#End Region

#Region " Edit level tracking "

  ' keep track of how many edit levels we have
  Private mEditLevel As Integer

#End Region

#Region " IsChild "

  Private mIsChild As Boolean = False

  Protected ReadOnly Property IsChild() As Boolean
    Get
      Return mIsChild
    End Get
  End Property

  Protected Sub MarkAsChild()
    mIsChild = True
  End Sub

#End Region

#Region " Clone "

  ' all business objects _must_ be serializable
  ' and thus can be cloned - this just clinches
  ' the deal
  Public Function Clone() As Object Implements ICloneable.Clone
    Dim buffer As New MemoryStream
    Dim formatter As New BinaryFormatter

    formatter.Serialize(buffer, Me)
    buffer.Position = 0
    Return formatter.Deserialize(buffer)
  End Function

#End Region

#Region " Data Access "

  ' add/save object
  Public Overridable Function Save() As BusinessCollectionBase
    If Me.IsChild Then
      Throw New NotSupportedException("Can not directly save a child object")
    End If

    If mEditLevel > 0 Then
      Throw New Exception("Object is still being edited and can not be saved")
    End If

    If Not IsValid Then
      Throw New Exception("Object is not valid and can not be saved")
    End If

    If IsDirty Then
      Return CType(DataPortal.Update(Me), BusinessCollectionBase)
    Else
      Return Me
    End If

  End Function

  Protected Overridable Sub DataPortal_Create(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - create not allowed")
  End Sub

  Protected Overridable Sub DataPortal_Fetch(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - fetch not allowed")
  End Sub

  Protected Overridable Sub DataPortal_Update()
    Throw New NotSupportedException("Invalid operation - update not allowed")
  End Sub

  Protected Overridable Sub DataPortal_Delete(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - delete not allowed")
  End Sub

  Protected Function DB(ByVal DatabaseName As String) As String
    Return ConfigurationSettings.AppSettings("DB:" & DatabaseName)
  End Function

#End Region

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

End Class
