using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;

namespace CSLA
{
  /// <summary>
  /// This is the base class from which most business collection
  /// objects will be derived.
  /// </summary>
  /// <remarks>
  /// <para>
  /// To create a collection of business objects, inherit from this 
  /// class. The business objects contained in this collection must
  /// inherit from <see cref="T:CSLA.BusinessBase" />, and the objects
  /// must be marked as child objects.
  /// </para><para>
  /// Please refer to 'Expert One-on-One VB.NET Business Objects' for
  /// full details on the use of this base class to create business
  /// collections.
  /// </para>
  /// </remarks>
  [Serializable()]
  abstract public class BusinessCollectionBase : CSLA.Core.BindableCollectionBase, 
                                                 ICloneable
  {
    #region Contains

    /// <summary>
    /// Used to see if the collection contains a specific child object.
    /// </summary>
    /// <remarks>
    /// Only the 'active' list of child objects is checked. 
    /// Business collections also contain deleted objects, which are
    /// not checked by this call.
    /// </remarks>
    /// <param name="Item">A reference to the object.</param>
    /// <returns>True if the collection contains the object.</returns>
    public bool Contains(BusinessBase item)
    {
      return List.Contains(item);
    }

    /// <summary>
    /// Used to see if the collection contains a reference to a
    /// child object that is marked for deletion.
    /// </summary>
    /// <remarks>
    /// This scans the list of child objects that have been marked
    /// for deletion. If this object is in that list, the method
    /// returns True.
    /// </remarks>
    /// <param name="Item">A reference to the object.</param>
    /// <returns>True if the collection contains the object.</returns>
    public bool ContainsDeleted(BusinessBase item)
    {
      foreach(BusinessBase element in deletedList)
        if(element.Equals(item))
          return true;
      return false;
    }

    #endregion

    #region IsDirty, IsValid

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
    /// <para>
    /// If any child object within the collection is dirty then the collection
    /// is considered to be dirty. If all child objects are unchanged, then the
    /// collection is not dirty.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if this object's data has been changed.</returns>
    public bool IsDirty
    {
      get
      {
        // any deletions make us dirty
        if(deletedList.Count > 0) 
          return true;

        // run through all the child objects
        // and if any are dirty then the
        // collection is dirty
        foreach(BusinessBase child in List)
          if(child.IsDirty)
            return true;
        return false;
      }
    }

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
    /// <para>
    /// If any child object within the collection is invalid then the collection
    /// is considered to be invalid. If all child objects are valid, then the
    /// collection is valid.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    public bool IsValid
    {
      get
      {
        // run through all the child objects
        // and if any are invalid then the
        // collection is invalid
        foreach(BusinessBase child in List)
          if(!child.IsValid)
            return false;
        return true;
      }
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
    /// </para><para>
    /// This method triggers the copying of all child object states.
    /// </para>
    /// </remarks>
    public void BeginEdit()
    {
      if(this.IsChild)
        throw new NotSupportedException(
                                "BeginEdit is not valid on a child object");

      CopyState();
    }

    /// <summary>
    /// Cancels the current edit process, restoring the object's state to
    /// its previous values.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be restored. This resets the object's values
    /// to the point of the last <see cref="M:CSLA.BusinessCollectionBase.BeginEdit" />
    /// call.
    /// <para>
    /// This method triggers an undo in all child objects.
    /// </para>
    /// </remarks>
    public void CancelEdit()
    {
      if(this.IsChild)
        throw new NotSupportedException(
                                "CancelEdit is not valid on a child object");

      UndoChanges();
    }

    /// <summary>
    /// Commits the current edit process.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be discarded, thus committing any changes made
    /// to the object's state since the last 
    /// <see cref="M:CSLA.BusinessCollectionBase.BeginEdit" /> call.
    /// <para>
    /// This method triggers an ApplyEdit in all child objects.
    /// </para>
    /// </remarks>
    public void ApplyEdit()
    {
      if(this.IsChild)
        throw new NotSupportedException(
                                "ApplyEdit is not valid on a child object");

      AcceptChanges();
    }

    #endregion

    #region N-level undo

    internal void CopyState()
    {
      // we are going a level deeper in editing
      _EditLevel += 1;

      // cascade the call to all child objects
      foreach(BusinessBase child in List)
        child.CopyState();

      // cascade the call to all deleted child objects
      foreach(BusinessBase child in deletedList)
        child.CopyState();
    }

    internal void UndoChanges()
    {
      BusinessBase child;

      // we are coming up one edit level
      _EditLevel -= 1;
      if(_EditLevel < 0)
        _EditLevel = 0;

      // Cancel edit on all current items
      for(int index = List.Count - 1; index > 0; index--)
      {
        child = (BusinessBase)List[index];
        child.UndoChanges();
        // if item is below its point of addition, remove
        if(child.EditLevelAdded > _EditLevel)
          List.Remove(child);
      }

      // cancel edit on all deleted items
      for(int index = deletedList.Count - 1; index > 0; index--)
      {
        child = (BusinessBase)deletedList[index];
        child.UndoChanges();
        // if item is below its point of addition, remove
        if(child.EditLevelAdded > _EditLevel)
          deletedList.Remove(child);
        // if item is no longer deleted move back to main list
        if(!child.IsDeleted)
          UnDeleteChild(child);
      }
    }

    internal void AcceptChanges()
    {
      // we are coming up one edit level
      _EditLevel -= 1;
      if(_EditLevel < 0)
        _EditLevel = 0;

      // cascade the call to all child objects
      foreach(BusinessBase child in List)
      {
        child.AcceptChanges();
        // if item is below its point of addition, lower point of addition
        if(child.EditLevelAdded > _EditLevel)
          child.EditLevelAdded = _EditLevel;
      }

      // cascade the call to all deleted child objects
      foreach(BusinessBase child in deletedList)
      {
        child.AcceptChanges();
        // if item is below its point of addition, lower point of addition
        if(child.EditLevelAdded > _EditLevel)
          child.EditLevelAdded = _EditLevel;
      }
    }

    #endregion

    #region Delete and Undelete child

    private void DeleteChild(BusinessBase child)
    {
      // mark the object as deleted
      child.DeleteChild();
      // and add it to the deleted collection for storage
      deletedList.Add(child);
    }

    private void UnDeleteChild(BusinessBase child)
    {
      // we are inserting an _existing_ object so
      // we need to preserve the object's editleveladded value
      // because it will be changed by the normal add process
      int saveLevel = child.EditLevelAdded;
      List.Add(child);
      child.EditLevelAdded = saveLevel;

      // since the object is no longer deleted, remove it from
      // the deleted collection
      deletedList.Remove(child);
    }

    #endregion

    #region DeletedCollection

    /// <summary>
    /// A collection containing all child objects marked
    /// for deletion.
    /// </summary>
    protected DeletedCollection deletedList = new DeletedCollection();

    /// <summary>
    /// Defines a strongly-typed collection to store all
    /// child objects marked for deletion.
    /// </summary>
    [Serializable()]
      protected class DeletedCollection : CollectionBase
    {
      /// <summary>
      /// Adds a child object to the collection.
      /// </summary>
      /// <param name="Child">The child object to be added.</param>
      public void Add(BusinessBase child)
      {
        List.Add(child);
      }

      /// <summary>
      /// Removes a child object from the collection.
      /// </summary>
      /// <param name="Child">The child object to be removed.</param>
      public void Remove(BusinessBase child)
      {
        List.Remove(child);
      }

      /// <summary>
      /// Returns a reference to a child object in the collection.
      /// </summary>
      /// <param name="index">The positional index of the item in the collection.</param>
      /// <returns>The specified child object.</returns>
      public BusinessBase this [int index]
      {
        get 
        { 
          return (BusinessBase)List[index]; 
        }
      }
    }

    #endregion

    #region Insert, Remove, Clear

    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    internal void RemoveChild(BusinessBase child)
    {
      List.Remove(child);
    }

    /// <summary>
    /// Sets the edit level of the child object as it is added.
    /// </summary>
    protected override void OnInsert(int index, object val)
    {
      // when an object is inserted we assume it is
      // a new object and so the edit level when it was
      // added must be set
      ((BusinessBase)val).EditLevelAdded = _EditLevel;
      ((BusinessBase)val).SetParent(this);
      base.OnInsert(index, val);
    }

    /// <summary>
    /// Marks the child object for deletion and moves it to
    /// the collection of deleted objects.
    /// </summary>
    protected override void OnRemove(int index, object val)
    {
      // when an object is 'removed' it is really
      // being deleted, so do the deletion work
      DeleteChild((BusinessBase)val);
      base.OnRemove(index, val);
    }

    /// <summary>
    /// Marks all child objects for deletion and moves them
    /// to the collection of deleted objects.
    /// </summary>
    protected override void OnClear()
    {
      // when an object is 'removed' it is really
      // being deleted, so do the deletion work
      // for all the objects in the list
      while(List.Count > 0)
        List.RemoveAt(0);
      base.OnClear();
    }

    #endregion

    #region Edit level tracking

    // keep track of how many edit levels we have
    int _EditLevel;

    #endregion

    #region IsChild

    bool _IsChild = false;

    /// <summary>
    /// Indicates whether this collection object is a child object.
    /// </summary>
    /// <returns>True if this is a child object.</returns>
    protected bool IsChild
    {
      get
      {
        return _IsChild;
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
      _IsChild = true;
    }

    #endregion

    #region ICloneable

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public object Clone()
    {

      MemoryStream buffer = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();

      formatter.Serialize(buffer, this);
      buffer.Position = 0;
      return formatter.Deserialize(buffer);
    }

    #endregion

    #region Data Access

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Calling this method starts the save operation, causing the all child
    /// objects to be inserted, updated or deleted within the database based on the
    /// each object's current state.
    /// </para><para>
    /// All this is contingent on <see cref="P:CSLA.BusinessCollectionBase.IsDirty" />. If
    /// this value is False, no data operation occurs. It is also contingent on
    /// <see cref="P:CSLA.BusinessCollectionBase.IsValid" />. If this value is False an
    /// exception will be thrown to indicate that the UI attempted to save an
    /// invalid object.
    /// </para><para>
    /// It is important to note that this method returns a new version of the
    /// business collection that contains any data updated during the save operation.
    /// You MUST update all object references to use this new version of the
    /// business collection in order to have access to the correct object data.
    /// </para><para>
    /// You can override this method to add your own custom behaviors to the save
    /// operation. For instance, you may add some security checks to make sure
    /// the user can save the object. If all security checks pass, you would then
    /// invoke the base Save method via <c>MyBase.Save()</c>.
    /// </para>
    /// </remarks>
    /// <returns>A new object containing the saved values.</returns>
    virtual public BusinessCollectionBase Save()
    {
      if(this.IsChild)
        throw new NotSupportedException("Can not directly save a child object");

      if(_EditLevel > 0)
        throw new Exception("Object is still being edited and can not be saved");

      if(!IsValid)
        throw new Exception("Object is not valid and can not be saved");

      if(IsDirty)
        return (BusinessCollectionBase)DataPortal.Update(this);
      else
        return this;
    }

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">An object containing criteria values.</param>
    virtual protected void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException("Invalid operation - create not allowed");
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">
    /// An object containing criteria values to identify the object.</param>
    virtual protected void DataPortal_Fetch(object criteria)
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
    virtual protected void DataPortal_Delete(object criteria)
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
    /// <param name="DatabaseName">Name of the database.</param>
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
