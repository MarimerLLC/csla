using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Configuration;

namespace CSLA
{
  /// <summary>
  /// This is the base class from which most business objects
  /// will be derived.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class is the core of the CSLA .NET framework. To create
  /// a business object, inherit from this class.
  /// </para><para>
  /// Please refer to 'Expert One-on-One VB.NET Business Objects' for
  /// full details on the use of this base class to create business
  /// objects.
  /// </para>
  /// </remarks>
  [Serializable()]
  abstract public class BusinessBase : Core.UndoableBase, IEditableObject, ICloneable
  {
#region IsNew, IsDeleted, IsDirty

    // keep track of whether we are new, deleted or dirty
    bool _isNew = true;
    bool _isDeleted = false;
    bool _isDirty = true;

    /// <summary>
    /// Returns True if this is a new object, False if it is a pre-existing object.
    /// </summary>
    /// <remarks>
    /// An object is considered to be new if its data doesn't correspond to
    /// data in the database. In other words, if the data values in this particular
    /// object have not yet been saved to the database the object is considered to
    /// be new. Likewise, if the object's data has been deleted from the database
    /// then the object is considered to be new.
    /// </remarks>
    /// <returns>A value indicating if this object is new.</returns>
    public bool IsNew
    {
      get
      {
        return _isNew;
      }
    }

    /// <summary>
    /// Returns True if this object is marked for deletion.
    /// </summary>
    /// <remarks>
    /// CSLA .NET supports both immediate and deferred deletion of objects. This
    /// property is part of the support for deferred deletion, where an object
    /// can be marked for deletion, but isn't actually deleted until the object
    /// is saved to the database. This property indicates whether or not the
    /// current object has been marked for deletion. If it is True, the object will
    /// be deleted when it is saved to the database, otherwise it will be inserted
    /// or updated by the save operation.
    /// </remarks>
    /// <returns>A value indicating if this object is marked for deletion.</returns>
    public bool IsDeleted
    {
      get
      {
        return _isDeleted;
      }
    }

    /// <summary>
    /// Returns True if this object's data has been changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an object's data is changed, CSLA .NET makes note of that change
    /// and considers the object to be 'dirty' or changed. This value is used to
    /// optimize data updates, since an unchanged object does not need to be
    /// updated into the database. All new objects are considered dirty. All objects
    /// marked for deletion are considered dirty.
    /// </para><para>
    /// Once an object's data has been saved to the database (inserted or updated)
    /// the dirty flag is cleared and the object is considered unchanged. Objects
    /// newly loaded from the database are also considered unchanged.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if this object's data has been changed.</returns>
    virtual public bool IsDirty
    {
      get
      {
        return _isDirty;
      }
    }

    /// <summary>
    /// Marks the object as being a new object. This also marks the object
    /// as being dirty and ensures that it is not marked for deletion.
    /// </summary>
    /// <remarks>
    /// Newly created objects are marked new by default. You should call
    /// this method in the implementation of DataPortal_Update when the
    /// object is deleted (due to being marked for deletion) to indicate
    /// that the object no longer reflects data in the database.
    /// </remarks>
    protected void MarkNew()
    {
      _isNew = true;
      _isDeleted = false;
      MarkDirty();
    }

    /// <summary>
    /// Marks the object as being an old (not new) object. This also
    /// marks the object as being unchanged (not dirty).
    /// </summary>
    /// <remarks>
    /// <para>
    /// You should call this method in the implementation of
    /// DataPortal_Fetch to indicate that an existing object has been
    /// successfully retrieved from the database.
    /// </para><para>
    /// You should call this method in the implementation of 
    /// DataPortal_Update to indicate that a new object has been successfully
    /// inserted into the database.
    /// </para>
    /// </remarks>
    protected void MarkOld()
    {
      _isNew = false;
      MarkClean();
    }

    /// <summary>
    /// Marks an object for deletion. This also marks the object
    /// as being dirty.
    /// </summary>
    /// <remarks>
    /// You should call this method in your business logic in the
    /// case that you want to have the object deleted when it is
    /// saved to the database.
    /// </remarks>
    protected void MarkDeleted()
    {
      _isDeleted = true;
      MarkDirty();
    }

    /// <summary>
    /// Marks an object as being dirty, or changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You should call this method in your business logic any time
    /// the object's internal data changes. Any time any instance
    /// variable changes within the object, this method should be called
    /// to tell CSLA .NET that the object's data has been changed.
    /// </para><para>
    /// Marking an object as dirty does two things. First it ensures
    /// that CSLA .NET will properly save the object as appropriate. Second,
    /// it causes CSLA .NET to tell Windows Forms data binding that the
    /// object's data has changed so any bound controls will update to
    /// reflect the new values.
    /// </para>
    /// </remarks>
    protected void MarkDirty()
    {
      _isDirty = true;
      OnIsDirtyChanged();
    }

    private void MarkClean()
    {
      _isDirty = false;
      OnIsDirtyChanged();
    }

#endregion

#region IEditableObject

    [NotUndoable()]
    BusinessCollectionBase _parent;
    [NotUndoable()]
    bool _bindingEdit = false;
    bool _neverCommitted = true;

    /// <summary>
    /// Used by <see cref="T:CSLA.BusinessCollectionBase" /> as a
    /// child object is created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    internal void SetParent(BusinessCollectionBase parent)
    {
      if(!IsChild)
        throw new Exception("Parent value can only be set for child objects");
      _parent = parent;
    }

    /// <summary>
    /// Allow data binding to start a nested edit on the object.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call should be honored, so we have extra code to detect this
    /// and do nothing for subsquent calls.
    /// </remarks>
    void IEditableObject.BeginEdit()
    {
      if(!_bindingEdit)
        BeginEdit();
    }

    /// <summary>
    /// Allow data binding to cancel the current edit.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call to either IEditableObject.CancelEdit or 
    /// <see cref="M:CSLA.BusinessBase.IEditableObject_EndEdit">IEditableObject.EndEdit</see>
    /// should be honored. We include extra code to detect this and do
    /// nothing for subsequent calls.
    /// </remarks>
    void IEditableObject.CancelEdit()
    {
      if(_bindingEdit)
      {
        CancelEdit();
        if(IsNew && _neverCommitted && EditLevel <= EditLevelAdded)
        {
          // we're new and no EndEdit or ApplyEdit has ever been
          // called on us, and now we've been canceled back to 
          // where we were added so we should have ourselves  
          // removed from the parent collection
          if(!(_parent == null))
            _parent.RemoveChild(this);
        }
      }
    }

    /// <summary>
    /// Allow data binding to apply the current edit.
    /// </summary>
    /// <remarks>
    /// Data binding may call this method many times. Only the first
    /// call to either IEditableObject.EndEdit or 
    /// <see cref="M:CSLA.BusinessBase.IEditableObject_CancelEdit">
    /// IEditableObject.CancelEdit</see>
    /// should be honored. We include extra code to detect this and do
    /// nothing for subsequent calls.
    /// </remarks>
    void IEditableObject.EndEdit()
    {
      if(_bindingEdit)
        ApplyEdit();
    }

#endregion

#region Begin/Cancel/ApplyEdit

    /// <summary>
    /// Starts a nested edit on the object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When this method is called the object takes a snapshot of
    /// its current state (the values of its variables). This snapshot
    /// can be restored by calling <see cref="M:CSLA.BusinessBase.CancelEdit" />
    /// or committed by calling <see cref="M:CSLA.BusinessBase.ApplyEdit" />.
    /// </para><para>
    /// This is a nested operation. Each call to BeginEdit adds a new
    /// snapshot of the object's state to a stack. You should ensure that 
    /// for each call to BeginEdit there is a corresponding call to either 
    /// CancelEdit or ApplyEdit to remove that snapshot from the stack.
    /// </para><para>
    /// See Chapters 2 and 4 for details on n-level undo and state stacking.
    /// </para>
    /// </remarks>
    public void BeginEdit()
    {
      _bindingEdit = true;
      CopyState();
    }

    /// <summary>
    /// Cancels the current edit process, restoring the object's state to
    /// its previous values.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be restored. This resets the object's values
    /// to the point of the last <see cref="M:CSLA.BusinessBase.BeginEdit" />
    /// call.
    /// </remarks>
    public void CancelEdit()
    {
      _bindingEdit = false;
      UndoChanges();
    }

    /// <summary>
    /// Commits the current edit process.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be discarded, thus committing any changes made
    /// to the object's state since the last <see cref="M:CSLA.BusinessBase.BeginEdit" />
    /// call.
    /// </remarks>
    public void ApplyEdit()
    {
      _bindingEdit = false;
      _neverCommitted = false;
      AcceptChanges();
    }

#endregion

#region IsChild

    [NotUndoable()]
    bool _isChild = false;

    internal bool IsChild
    {
      get
      {
        return _isChild;
      }
    }

    /// <summary>
    /// Marks the object as being a child object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default all business objects are 'parent' objects. This means
    /// that they can be directly retrieved and updated into the database.
    /// </para><para>
    /// We often also need child objects. These are objects which are contained
    /// within other objects. For instance, a parent Invoice object will contain
    /// child LineItem objects.
    /// </para><para>
    /// To create a child object, the MarkAsChild method must be called as the
    /// object is created. Please see Chapter 7 for details on the use of the
    /// MarkAsChild method.
    /// </para>
    /// </remarks>
    protected void MarkAsChild()
    {
      _isChild = true;
    }

#endregion

#region Delete

    /// <summary>
    /// Marks the object for deletion. The object will be deleted as part of the
    /// next save operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// CSLA .NET supports both immediate and deferred deletion of objects. This
    /// method is part of the support for deferred deletion, where an object
    /// can be marked for deletion, but isn't actually deleted until the object
    /// is saved to the database. This method is called by the UI developer to
    /// mark the object for deletion.
    /// </para><para>
    /// To 'undelete' an object, use <see cref="M:CSLA.BusinessBase.BeginEdit" /> before
    /// calling the Delete method. You can then use <see cref="M:CSLA.BusinessBase.CancelEdit" />
    /// later to reset the object's state to its original values. This will include resetting
    /// the deleted flag to False.
    /// </para>
    /// </remarks>
    public void Delete()
    {
      if(this.IsChild)
        throw new NotSupportedException(
          "Can not directly mark a child object for deletion - use its parent collection");

      MarkDeleted();
    }

    // allow the parent object to delete us
    // (internal scope)
    internal void DeleteChild()
    {
      if(!this.IsChild)
        throw new NotSupportedException("Invalid for root objects - use Delete instead");
    
      MarkDeleted();
    }

#endregion

#region Edit Level Tracking (child only)

    // we need to keep track of the edit
    // level when we were added so if the user
    // cancels below that level we can be destroyed
    int _editLevelAdded;

    // allow the collection object to use the
    // edit level as needed (internal scope)
    internal int EditLevelAdded
    {
      get
      {
        return _editLevelAdded;
      }
      set
      {
        _editLevelAdded = value;
      }
    }

#endregion

#region ICloneable

  /// <summary>
  /// Creates a clone of the object.
  /// </summary>
  /// <returns>A new object containing the exact data of the original object.</returns>
  public Object Clone()
{

  MemoryStream buffer = new MemoryStream();
  BinaryFormatter formatter = new BinaryFormatter();

  formatter.Serialize(buffer, this);
  buffer.Position = 0;
  return formatter.Deserialize(buffer);
}

#endregion

#region BrokenRules, IsValid

    // keep a list of broken rules
    BrokenRules _brokenRules = new BrokenRules();

    /// <summary>
    /// Returns True if the object is currently valid, False if the
    /// object has broken rules or is otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling <see cref="T:CSLA.BrokenRules" />
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior. For instance, you should always override
    /// this method if your object has child objects, since the validity of this object
    /// is affected by the validity of all child objects.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    virtual public bool IsValid
    {
      get
      {
        return _brokenRules.IsValid;
      }
    }

    /// <summary>
    /// Provides access to the readonly collection of broken business rules
    /// for this object.
    /// </summary>
    /// <returns>A <see cref="T:CSLA.BrokenRules.RulesCollection" /> object.</returns>
    public BrokenRules.RulesCollection GetBrokenRulesCollection()
    {
      return _brokenRules.GetBrokenRules();
    }

    /// <summary>
    /// Provides access to a text representation of all the descriptions of
    /// the currently broken business rules for this object.
    /// </summary>
    /// <returns>Text containing the descriptions of the broken business rules.</returns>
    public string GetBrokenRulesString()
    {
      return _brokenRules.ToString();
    }

    /// <summary>
    /// Provides access to the broken rules functionality.
    /// </summary>
    /// <remarks>
    /// This property is used within your business logic so you can
    /// easily call the 
    /// <see cref="M:CSLA.BrokenRules.Assert(System.String,System.String,System.Boolean)" /> 
    /// method to mark rules as broken and unbroken.
    /// </remarks>
    protected BrokenRules BrokenRules
    {
      get
      {
        return _brokenRules;
      }
    }

#endregion

#region Data Access

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Calling this method starts the save operation, causing the object
    /// to be inserted, updated or deleted within the database based on the
    /// object's current state.
    /// </para><para>
    /// If <see cref="P:CSLA.BusinessBase.IsDeleted" /> is True the object
    /// will be deleted. Otherwise, if <see cref="P:CSLA.BusinessBase.IsNew" /> 
    /// is True the object will be inserted. Otherwise the object's data will 
    /// be updated in the database.
    /// </para><para>
    /// All this is contingent on <see cref="P:CSLA.BusinessBase.IsDirty" />. If
    /// this value is False, no data operation occurs. It is also contingent on
    /// <see cref="P:CSLA.BusinessBase.IsValid" />. If this value is False an
    /// exception will be thrown to indicate that the UI attempted to save an
    /// invalid object.
    /// </para><para>
    /// It is important to note that this method returns a new version of the
    /// business object that contains any data updated during the save operation.
    /// You MUST update all object references to use this new version of the
    /// business object in order to have access to the correct object data.
    /// </para><para>
    /// You can override this method to add your own custom behaviors to the save
    /// operation. For instance, you may add some security checks to make sure
    /// the user can save the object. If all security checks pass, you would then
    /// invoke the base Save method via <c>MyBase.Save()</c>.
    /// </para>
    /// </remarks>
    /// <returns>A new object containing the saved values.</returns>
    virtual public BusinessBase Save()
    {
      if(this.IsChild)
        throw new NotSupportedException("Can not directly save a child object");

      if(EditLevel > 0)
        throw new Exception("Object is still being edited and can not be saved");

      if(!IsValid)
        throw new Exception("Object is not valid and can not be saved");

      if(IsDirty)
        return (BusinessBase)DataPortal.Update(this);
      else
        return this;
    }

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">An object containing criteria values.</param>
    virtual protected void DataPortal_Create(Object criteria)
    {
      throw new NotSupportedException("Invalid operation - create not allowed");
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">
    /// An object containing criteria values to identify the object.</param>
    virtual protected void DataPortal_Fetch(Object criteria)
    {
      throw new NotSupportedException("Invalid operation - fetch not allowed");
    }

    /// <summary>
    /// Override this method to allow insert, update or deletion of a business
    /// object.
    /// </summary>
    virtual protected void DataPortal_Update()
    {
      throw new NotSupportedException("Invalid operation - update not allowed");
    }

    /// <summary>
    /// Override this method to allow immediate deletion of a business object.
    /// </summary>
    /// <param name="criteria">
    /// An object containing criteria values to identify the object.</param>
    virtual protected void DataPortal_Delete(Object criteria)
    {
      throw new NotSupportedException("Invalid operation - delete not allowed");
    }

    /// <summary>
    /// Returns the specified database connection string from the application
    /// configuration file.
    /// </summary>
    /// <remarks>
    /// The database connection string must be in the <c>appSettings</c> section
    /// of the application configuration file. The database name should be
    /// prefixed with 'DB:'. For instance, <c>DB:mydatabase</c>.
    /// </remarks>
    /// <param name="databaseName">Name of the database.</param>
    /// <returns>A database connection string.</returns>
    protected string DB(string databaseName)
    {
      string val = ConfigurationSettings.AppSettings["DB:" + databaseName];
      if(val == null)
        return string.Empty;
      else
        return val;
    }

#endregion

  }
}
