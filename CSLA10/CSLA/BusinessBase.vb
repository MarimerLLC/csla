Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.ComponentModel
Imports System.Configuration

''' <summary>
''' This is the base class from which most business objects
''' will be derived.
''' </summary>
''' <remarks>
''' <para>
''' This class is the core of the CSLA .NET framework. To create
''' a business object, inherit from this class.
''' </para><para>
''' Please refer to 'Expert One-on-One VB.NET Business Objects' for
''' full details on the use of this base class to create business
''' objects.
''' </para>
''' </remarks>
<Serializable()> _
Public MustInherit Class BusinessBase
  Inherits Core.UndoableBase

  Implements IEditableObject
  Implements ICloneable

#Region " IsNew, IsDeleted, IsDirty "

  ' keep track of whether we are new, deleted or dirty
  Private mIsNew As Boolean = True
  Private mIsDeleted As Boolean = False
  Private mIsDirty As Boolean = True

  ''' <summary>
  ''' Returns True if this is a new object, False if it is a pre-existing object.
  ''' </summary>
  ''' <remarks>
  ''' An object is considered to be new if its data doesn't correspond to
  ''' data in the database. In other words, if the data values in this particular
  ''' object have not yet been saved to the database the object is considered to
  ''' be new. Likewise, if the object's data has been deleted from the database
  ''' then the object is considered to be new.
  ''' </remarks>
  ''' <returns>A value indicating if this object is new.</returns>
  Public ReadOnly Property IsNew() As Boolean
    Get
      Return mIsNew
    End Get
  End Property

  ''' <summary>
  ''' Returns True if this object is marked for deletion.
  ''' </summary>
  ''' <remarks>
  ''' CSLA .NET supports both immediate and deferred deletion of objects. This
  ''' property is part of the support for deferred deletion, where an object
  ''' can be marked for deletion, but isn't actually deleted until the object
  ''' is saved to the database. This property indicates whether or not the
  ''' current object has been marked for deletion. If it is True, the object will
  ''' be deleted when it is saved to the database, otherwise it will be inserted
  ''' or updated by the save operation.
  ''' </remarks>
  ''' <returns>A value indicating if this object is marked for deletion.</returns>
  Public ReadOnly Property IsDeleted() As Boolean
    Get
      Return mIsDeleted
    End Get
  End Property

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
  ''' </remarks>
  ''' <returns>A value indicating if this object's data has been changed.</returns>
  Public Overridable ReadOnly Property IsDirty() As Boolean
    Get
      Return mIsDirty
    End Get
  End Property

  ''' <summary>
  ''' Marks the object as being a new object. This also marks the object
  ''' as being dirty and ensures that it is not marked for deletion.
  ''' </summary>
  ''' <remarks>
  ''' Newly created objects are marked new by default. You should call
  ''' this method in the implementation of DataPortal_Update when the
  ''' object is deleted (due to being marked for deletion) to indicate
  ''' that the object no longer reflects data in the database.
  ''' </remarks>
  Protected Sub MarkNew()
    mIsNew = True
    mIsDeleted = False
    MarkDirty()
  End Sub

  ''' <summary>
  ''' Marks the object as being an old (not new) object. This also
  ''' marks the object as being unchanged (not dirty).
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' You should call this method in the implementation of
  ''' DataPortal_Fetch to indicate that an existing object has been
  ''' successfully retrieved from the database.
  ''' </para><para>
  ''' You should call this method in the implementation of 
  ''' DataPortal_Update to indicate that a new object has been successfully
  ''' inserted into the database.
  ''' </para>
  ''' </remarks>
  Protected Sub MarkOld()
    mIsNew = False
    MarkClean()
  End Sub

  ''' <summary>
  ''' Marks an object for deletion. This also marks the object
  ''' as being dirty.
  ''' </summary>
  ''' <remarks>
  ''' You should call this method in your business logic in the
  ''' case that you want to have the object deleted when it is
  ''' saved to the database.
  ''' </remarks>
  Protected Sub MarkDeleted()
    mIsDeleted = True
    MarkDirty()
  End Sub

  ''' <summary>
  ''' Marks an object as being dirty, or changed.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' You should call this method in your business logic any time
  ''' the object's internal data changes. Any time any instance
  ''' variable changes within the object, this method should be called
  ''' to tell CSLA .NET that the object's data has been changed.
  ''' </para><para>
  ''' Marking an object as dirty does two things. First it ensures
  ''' that CSLA .NET will properly save the object as appropriate. Second,
  ''' it causes CSLA .NET to tell Windows Forms data binding that the
  ''' object's data has changed so any bound controls will update to
  ''' reflect the new values.
  ''' </para>
  ''' </remarks>
  Protected Sub MarkDirty()
    mIsDirty = True
    OnIsDirtyChanged()
  End Sub

  Private Sub MarkClean()
    mIsDirty = False
    OnIsDirtyChanged()
  End Sub

#End Region

#Region " Begin/Cancel/ApplyEdit "

  <NotUndoable()> _
  Private mBindingEdit As Boolean = False

  ' allow data binding to start a nested edit on the object
  Private Sub BindingBeginEdit() Implements IEditableObject.BeginEdit
    If Not mBindingEdit Then
      BeginEdit()
    End If
  End Sub

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
  ''' </para>
  ''' </remarks>
  Public Sub BeginEdit()
    mBindingEdit = True
    CopyState()
  End Sub

  ''' <summary>
  ''' Cancels the current edit process, restoring the object's state to
  ''' its previous values.
  ''' </summary>
  ''' <remarks>
  ''' Calling this method causes the most recently taken snapshot of the 
  ''' object's state to be restored. This resets the object's values
  ''' to the point of the last <see cref="M:CSLA.BusinessBase.BeginEdit" />
  ''' call.
  ''' </remarks>
  Public Sub CancelEdit() Implements IEditableObject.CancelEdit
    mBindingEdit = False
    UndoChanges()
  End Sub

  ''' <summary>
  ''' Commits the current edit process.
  ''' </summary>
  ''' <remarks>
  ''' Calling this method causes the most recently taken snapshot of the 
  ''' object's state to be discarded, thus committing any changes made
  ''' to the object's state since the last <see cref="M:CSLA.BusinessBase.BeginEdit" />
  ''' call.
  ''' </remarks>
  Public Sub ApplyEdit() Implements IEditableObject.EndEdit
    mBindingEdit = False
    AcceptChanges()
  End Sub

#End Region

#Region " IsChild "

  <NotUndoable()> _
  Private mIsChild As Boolean = False

  Friend ReadOnly Property IsChild() As Boolean
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

#Region " Delete "

  ''' <summary>
  ''' Marks the object for deletion. The object will be deleted as part of the
  ''' next save operation.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' CSLA .NET supports both immediate and deferred deletion of objects. This
  ''' method is part of the support for deferred deletion, where an object
  ''' can be marked for deletion, but isn't actually deleted until the object
  ''' is saved to the database. This method is called by the UI developer to
  ''' mark the object for deletion.
  ''' </para><para>
  ''' To 'undelete' an object, use <see cref="M:CSLA.BusinessBase.BeginEdit" /> before
  ''' calling the Delete method. You can then use <see cref="M:CSLA.BusinessBase.CancelEdit" />
  ''' later to reset the object's state to its original values. This will include resetting
  ''' the deleted flag to False.
  ''' </para>
  ''' </remarks>
  Public Sub Delete()
    If Me.IsChild Then
      Throw New NotSupportedException("Can not directly mark a child object for deletion - use its parent collection")
    End If

    MarkDeleted()

  End Sub

  ' allow the parent object to delete us
  ' (Friend scope)
  Friend Sub DeleteChild()
    If Not Me.IsChild Then
      Throw New NotSupportedException("Invalid for root objects - use Delete instead")
    End If

    MarkDeleted()

  End Sub

#End Region

#Region " Edit Level Tracking (child only) "

  ' we need to keep track of the edit
  ' level when we were added so if the user
  ' cancels below that level we can be destroyed
  Private mEditLevelAdded As Integer

  ' allow the collection object to use the
  ' edit level as needed (Friend scope)
  Friend Property EditLevelAdded() As Integer
    Get
      Return mEditLevelAdded
    End Get
    Set(ByVal Value As Integer)
      mEditLevelAdded = Value
    End Set
  End Property

#End Region

#Region " Clone "

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>A new object containing the exact data of the original object.</returns>
  Public Function Clone() As Object _
    Implements ICloneable.Clone

    Dim buffer As New MemoryStream()
    Dim formatter As New BinaryFormatter()

    formatter.Serialize(buffer, Me)
    buffer.Position = 0
    Return formatter.Deserialize(buffer)
  End Function

#End Region

#Region " BrokenRules, IsValid "

  ' keep a list of broken rules
  Private mBrokenRules As New BrokenRules

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
  ''' </remarks>
  ''' <returns>A value indicating if the object is currently valid.</returns>
  Public Overridable ReadOnly Property IsValid() As Boolean
    Get
      Return mBrokenRules.IsValid
    End Get
  End Property

  ''' <summary>
  ''' Provides access to the readonly collection of broken business rules
  ''' for this object.
  ''' </summary>
  ''' <returns>A <see cref="T:CSLA.BrokenRules.RulesCollection" /> object.</returns>
  Public Function GetBrokenRulesCollection() As BrokenRules.RulesCollection
    Return mBrokenRules.GetBrokenRules
  End Function

  ''' <summary>
  ''' Provides access to a text representation of all the descriptions of
  ''' the currently broken business rules for this object.
  ''' </summary>
  ''' <returns>Text containing the descriptions of the broken business rules.</returns>
  Public Function GetBrokenRulesString() As String
    Return mBrokenRules.ToString
  End Function

  ''' <summary>
  ''' Provides access to the broken rules functionality.
  ''' </summary>
  ''' <remarks>
  ''' This property is used within your business logic so you can
  ''' easily call the <see cref="M:CSLA.BrokenRules.Assert(System.String,System.String,System.Boolean)" /> 
  ''' method to mark rules as broken and unbroken.
  ''' </remarks>
  Protected ReadOnly Property BrokenRules() As BrokenRules
    Get
      Return mBrokenRules
    End Get
  End Property

#End Region

#Region " Data Access "

  ''' <summary>
  ''' Saves the object to the database.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' Calling this method starts the save operation, causing the object
  ''' to be inserted, updated or deleted within the database based on the
  ''' object's current state.
  ''' </para><para>
  ''' If <see cref="P:CSLA.BusinessBase.IsDeleted" /> is True the object
  ''' will be deleted. Otherwise, if <see cref="P:CSLA.BusinessBase.IsNew" /> 
  ''' is True the object will be inserted. Otherwise the object's data will 
  ''' be updated in the database.
  ''' </para><para>
  ''' All this is contingent on <see cref="P:CSLA.BusinessBase.IsDirty" />. If
  ''' this value is False, no data operation occurs. It is also contingent on
  ''' <see cref="P:CSLA.BusinessBase.IsValid" />. If this value is False an
  ''' exception will be thrown to indicate that the UI attempted to save an
  ''' invalid object.
  ''' </para><para>
  ''' It is important to note that this method returns a new version of the
  ''' business object that contains any data updated during the save operation.
  ''' You MUST update all object references to use this new version of the
  ''' business object in order to have access to the correct object data.
  ''' </para><para>
  ''' You can override this method to add your own custom behaviors to the save
  ''' operation. For instance, you may add some security checks to make sure
  ''' the user can save the object. If all security checks pass, you would then
  ''' invoke the base Save method via <c>MyBase.Save()</c>.
  ''' </para>
  ''' </remarks>
  ''' <returns>A new object containing the saved values.</returns>
  Public Overridable Function Save() As BusinessBase
    If Me.IsChild Then
      Throw New NotSupportedException("Can not directly save a child object")
    End If

    If EditLevel > 0 Then
      Throw New Exception("Object is still being edited and can not be saved")
    End If

    If Not IsValid Then
      Throw New Exception("Object is not valid and can not be saved")
    End If

    If IsDirty Then
      Return CType(DataPortal.Update(Me), BusinessBase)
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
    Throw New NotSupportedException("Invalid operation - create not allowed")
  End Sub

  ''' <summary>
  ''' Override this method to allow retrieval of an existing business
  ''' object based on data in the database.
  ''' </summary>
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
  Protected Overridable Sub DataPortal_Fetch(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - fetch not allowed")
  End Sub

  ''' <summary>
  ''' Override this method to allow insert, update or deletion of a business
  ''' object.
  ''' </summary>
  Protected Overridable Sub DataPortal_Update()
    Throw New NotSupportedException("Invalid operation - update not allowed")
  End Sub

  ''' <summary>
  ''' Override this method to allow immediate deletion of a business object.
  ''' </summary>
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
  Protected Overridable Sub DataPortal_Delete(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - delete not allowed")
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

End Class
