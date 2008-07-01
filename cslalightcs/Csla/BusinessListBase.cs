using System;
using System.ComponentModel;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Core;
using Csla.Properties;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Csla.DataPortalClient;

namespace Csla
{
#if TESTING
  [DebuggerNonUserCode]
#endif
  [Serializable]
  public class BusinessListBase<T, C> : Core.ExtendedBindingList<C>, 
    ICloneable, 
    IUndoableObject,
    ISavable,
    ITrackStatus
    where T: BusinessListBase<T, C>
    where C : Core.IEditableBusinessObject
  {
    #region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return Core.ObjectCloner.Clone(this);
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public T Clone()
    {
      return (T)GetClone();
    }

    #endregion

    #region Delete and Undelete child

    private List<C> _deletedList;

    /// <summary>
    /// A collection containing all child objects marked
    /// for deletion.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected List<C> DeletedList
    {
      get
      {
        if (_deletedList == null)
          _deletedList = new List<C>();
        return _deletedList;
      }
    }

    private void DeleteChild(C child)
    {
      // set child edit level
      Core.UndoableBase.ResetChildEditLevel(child, this.EditLevel, false);

      // TODO: Implement i4o
      // remove from the index
      //RemoveIndexItem(child);
      // remove from the position map
      //RemoveFromMap(child);

      // mark the object as deleted
      child.DeleteChild();
      // and add it to the deleted collection for storage
      DeletedList.Add(child);
    }

    private void UnDeleteChild(C child)
    {
      // since the object is no longer deleted, remove it from
      // the deleted collection
      DeletedList.Remove(child);

      // we are inserting an _existing_ object so
      // we need to preserve the object's editleveladded value
      // because it will be changed by the normal add process
      int saveLevel = child.EditLevelAdded;
      
      // TODO: implement i4o
      //InsertIndexItem(child);

      Add(child);
      child.EditLevelAdded = saveLevel;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the internal deleted list
    /// contains the specified child object.
    /// </summary>
    /// <param name="item">Child object to check.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool ContainsDeleted(C item)
    {
      return DeletedList.Contains(item);
    }

    #endregion

    #region Edit level tracking

    // keep track of how many edit levels we have
    private int _editLevel;

    /// <summary>
    /// Returns the current edit level of the object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int EditLevel
    {
      get { return _editLevel; }
    }

    int Core.IUndoableObject.EditLevel
    {
      get
      {
        return this.EditLevel;
      }
    }

    #endregion

    #region N-level undo

    void Core.IUndoableObject.CopyState(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        CopyState(parentEditLevel);
    }

    void Core.IUndoableObject.UndoChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        UndoChanges(parentEditLevel);
    }

    void Core.IUndoableObject.AcceptChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        AcceptChanges(parentEditLevel);
    }

    private void CopyState(int parentEditLevel)
    {
      if (this.EditLevel + 1 > parentEditLevel)
        throw new Core.UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"));

      // we are going a level deeper in editing
      _editLevel += 1;

      // JMC 6/24/08
      // This used to be a foreach loop but there appears to be a bug
      // in the silverlight runtime (SL2 B2) since calling foreach here will result
      // in SEHException with the error code -2147467259, or Error Unkown.
      // Iterating on this collection outside of this call will result in
      // behavior as expected but for some reason doing it here results in
      // an unknown exception.

      // cascade the call to all child objects
      for (int x = 0; x < this.Count; x++)
      {
        C child = this[x];
        child.CopyState(_editLevel, false);
      }

      // cascade the call to all deleted child objects
      foreach (C child in DeletedList)
        child.CopyState(_editLevel, false);
    }

    private bool _completelyRemoveChild;

    private void UndoChanges(int parentEditLevel)
    {
      C child;

      if (this.EditLevel - 1 < parentEditLevel)
        throw new Core.UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"));

      // we are coming up one edit level
      _editLevel -= 1;
      if (_editLevel < 0) _editLevel = 0;

      bool oldRLCE = this.RaiseListChangedEvents;
      this.RaiseListChangedEvents = false;
      try
      {
        // Cancel edit on all current items
        for (int index = Count - 1; index >= 0; index--)
        {
          child = this[index];

          //ACE: Important, make sure to remove the item prior to
          //     it going through undo, otherwise, it will
          //     incur a more expensive RemoveByReference operation
          //DeferredLoadIndexIfNotLoaded();
          //_indexSet.RemoveItem(child);

          child.UndoChanges(_editLevel, false);

          //ACE: Now that we have undone the changes, we can add the item
          //     back in the index.
          //_indexSet.InsertItem(child);

          // if item is below its point of addition, remove
          if (child.EditLevelAdded > _editLevel)
          {
            bool oldAllowRemove = this.AllowRemove;
            try
            {
              this.AllowRemove = true;
              _completelyRemoveChild = true;
              //RemoveIndexItem(child);
              RemoveAt(index);
            }
            finally
            {
              _completelyRemoveChild = false;
              this.AllowRemove = oldAllowRemove;
            }
          }
        }

        // cancel edit on all deleted items
        for (int index = DeletedList.Count - 1; index >= 0; index--)
        {
          child = DeletedList[index];
          child.UndoChanges(_editLevel, false);
          if (child.EditLevelAdded > _editLevel)
          {
            // if item is below its point of addition, remove
            DeletedList.RemoveAt(index);
          }
          else
          {
            // if item is no longer deleted move back to main list
            if (!child.IsDeleted) UnDeleteChild(child);
          }
        }
      }
      finally
      {
        this.RaiseListChangedEvents = oldRLCE;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }

    private void AcceptChanges(int parentEditLevel)
    {
      if (this.EditLevel - 1 < parentEditLevel)
        throw new Core.UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"));

      // we are coming up one edit level
      _editLevel -= 1;
      if (_editLevel < 0) _editLevel = 0;

      // cascade the call to all child objects
      foreach (C child in this)
      {
        child.AcceptChanges(_editLevel, false);
        // if item is below its point of addition, lower point of addition
        if (child.EditLevelAdded > _editLevel) child.EditLevelAdded = _editLevel;
      }

      // cascade the call to all deleted child objects
      for (int index = DeletedList.Count - 1; index >= 0; index--)
      {
        C child = DeletedList[index];
        child.AcceptChanges(_editLevel, false);
        // if item is below its point of addition, remove
        if (child.EditLevelAdded > _editLevel)
          DeletedList.RemoveAt(index);
      }
    }

    #endregion

    #region IsChild

    [NotUndoable()]
    private bool _isChild = false;

    /// <summary>
    /// Indicates whether this collection object is a child object.
    /// </summary>
    /// <returns>True if this is a child object.</returns>
    protected bool IsChild
    {
      get { return _isChild; }
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

    #region IsDirty, IsValid, IsSavable

    /// <summary>
    /// Gets a value indicating whether this object's data has been changed.
    /// </summary>
    bool Core.ITrackStatus.IsSelfDirty
    {
      get { return IsDirty; }
    }

    /// <summary>
    /// Gets a value indicating whether this object's data has been changed.
    /// </summary>
    public bool IsDirty
    {
      get
      {
        // any non-new deletions make us dirty
        foreach (C item in DeletedList)
          if (!item.IsNew)
            return true;

        // run through all the child objects
        // and if any are dirty then then
        // collection is dirty
        foreach (C child in this)
          if (child.IsDirty)
            return true;
        return false;
      }
    }

    bool Core.ITrackStatus.IsSelfValid
    {
      get { return IsSelfValid; }
    }

    /// <summary>
    /// Gets a value indicating whether this object is currently in
    /// a valid state (has no broken validation rules).
    /// </summary>
    protected virtual bool IsSelfValid
    {
      get { return IsValid; }
    }

    /// <summary>
    /// Gets a value indicating whether this object is currently in
    /// a valid state (has no broken validation rules).
    /// </summary>
    public virtual bool IsValid
    {
      get
      {
        // run through all the child objects
        // and if any are invalid then the
        // collection is invalid
        foreach (C child in this)
          if (!child.IsValid)
            return false;
        return true;
      }
    }

    /// <summary>
    /// Returns <see langword="true" /> if this object is both dirty and valid.
    /// </summary>
    /// <returns>A value indicating if this object is both dirty and valid.</returns>
    public virtual bool IsSavable
    {
      get
      {
        // TODO: implement authorization rules
        bool auth = true; // Csla.Security.AuthorizationRules.CanEditObject(this.GetType());
        return (IsDirty && IsValid && auth);
      }
    }

    #endregion

    #region  ITrackStatus

    bool Core.ITrackStatus.IsNew
    {
      get
      {
        return false;
      }
    }

    bool Core.ITrackStatus.IsDeleted
    {
      get
      {
        return false;
      }
    }

    #endregion

    #region  Child Data Access

    /// <summary>
    /// Initializes a new instance of the object
    /// with default values.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_Create()
    { /* do nothing - list self-initializes */ }

    /// <summary>
    /// Saves all items in the list, automatically
    /// performing insert, update or delete operations
    /// as necessary.
    /// </summary>
    /// <param name="parameters">
    /// Optional parameters passed to child update
    /// methods.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void Child_Update(params object[] parameters)
    {
      var oldRLCE = this.RaiseListChangedEvents;
      this.RaiseListChangedEvents = false;
      try
      {
        ChildDataPortal<C> dp = new ChildDataPortal<C>();
        foreach (var child in DeletedList)
          dp.Update(child, parameters);
        
        DeletedList.Clear();

        foreach (var child in this)
          dp.Update(child, parameters);
      }
      finally
      {
        this.RaiseListChangedEvents = oldRLCE;
      }
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
    /// All this is contingent on <see cref="IsDirty" />. If
    /// this value is <see langword="false"/>, no data operation occurs. 
    /// It is also contingent on <see cref="IsValid" />. If this value is 
    /// <see langword="false"/> an exception will be thrown to 
    /// indicate that the UI attempted to save an invalid object.
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
    public virtual void Save()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoSaveChildException);

      if (_editLevel > 0)
        throw new Validation.ValidationException(Resources.NoSaveEditingException);

      if (!IsValid)
        throw new Validation.ValidationException(Resources.NoSaveInvalidException);

      if (IsDirty)
      {
        DataPortal<T> dp = new DataPortal<T>();
        dp.UpdateCompleted += (o, e) => { OnSaved(e.Object); };
        dp.BeginUpdate(this);
      }
    }

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Create()
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">An object containing criteria values to identify the object.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Resources.FetchNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow update of a business
    /// object.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow immediate deletion of a business object.
    /// </summary>
    /// <param name="criteria">An object containing criteria values to identify the object.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
    }

    #endregion

    #region ISavable Members

    void ISavable.Save()
    {
      Save();
    }

    void ISavable.SaveComplete(object newObject)
    {
      OnSaved((T)newObject);
    }

    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    public event EventHandler<Csla.Core.SavedEventArgs> Saved;

    /// <summary>
    /// Raises the <see cref="Saved"/> event, indicating that the
    /// object has been saved, and providing a reference
    /// to the new object instance.
    /// </summary>
    /// <param name="newObject">The new object instance.</param>
    protected void OnSaved(T newObject)
    {
      if (Saved != null)
        Saved(this, new SavedEventArgs(newObject));
    }

    #endregion

  }
}
