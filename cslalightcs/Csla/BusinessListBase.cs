using System;
using System.ComponentModel;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Core;
using Csla.Properties;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Csla.DataPortalClient;
using Csla.Serialization.Mobile;

namespace Csla
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public abstract class BusinessListBase<T, C> : ExtendedBindingList<C>,
    ICloneable,
    IUndoableObject,
    ISavable,
    ITrackStatus,
    IDataPortalTarget,
    IParent,
    IEditableCollection
    where T : BusinessListBase<T, C>
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

    private MobileList<C> _deletedList;

    /// <summary>
    /// A collection containing all child objects marked
    /// for deletion.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected MobileList<C> DeletedList
    {
      get
      {
        if (_deletedList == null)
          _deletedList = new MobileList<C>();
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

    #region Begin/Cancel/ApplyEdit

    /// <summary>
    /// Starts a nested edit on the object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When this method is called the object takes a snapshot of
    /// its current state (the values of its variables). This snapshot
    /// can be restored by calling <see cref="CancelEdit" />
    /// or committed by calling <see cref="ApplyEdit" />.
    /// </para><para>
    /// This is a nested operation. Each call to BeginEdit adds a new
    /// snapshot of the object's state to a stack. You should ensure that 
    /// for each call to BeginEdit there is a corresponding call to either 
    /// CancelEdit or ApplyEdit to remove that snapshot from the stack.
    /// </para><para>
    /// See Chapters 2 and 3 for details on n-level undo and state stacking.
    /// </para><para>
    /// This method triggers the copying of all child object states.
    /// </para>
    /// </remarks>
    public void BeginEdit()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoBeginEditChildException);

      CopyState(this.EditLevel + 1);
    }

    /// <summary>
    /// Cancels the current edit process, restoring the object's state to
    /// its previous values.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be restored. This resets the object's values
    /// to the point of the last <see cref="BeginEdit" />
    /// call.
    /// <para>
    /// This method triggers an undo in all child objects.
    /// </para>
    /// </remarks>
    public void CancelEdit()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoCancelEditChildException);

      UndoChanges(this.EditLevel - 1);
    }

    /// <summary>
    /// Commits the current edit process.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be discarded, thus committing any changes made
    /// to the object's state since the last 
    /// <see cref="BeginEdit" /> call.
    /// <para>
    /// This method triggers an <see cref="Core.BusinessBase.ApplyEdit"/>
    ///  in all child objects.
    /// </para>
    /// </remarks>
    public void ApplyEdit()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoApplyEditChildException);

      AcceptChanges(this.EditLevel - 1);
    }

    void Core.IParent.ApplyEditChild(Core.IEditableBusinessObject child)
    {
      EditChildComplete(child);
    }

    /// <summary>
    /// Override this method to be notified when a child object's
    /// <see cref="Core.BusinessBase.ApplyEdit" /> method has
    /// completed.
    /// </summary>
    /// <param name="child">The child object that was edited.</param>
    protected virtual void EditChildComplete(Core.IEditableBusinessObject child)
    {

      // do nothing, we don't really care
      // when a child has its edits applied
    }

    #endregion

    #region Insert, Remove, Clear

    //[EditorBrowsable(EditorBrowsableState.Advanced)]
    //protected virtual void OnChildPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{ }

    //private void Child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //  OnChildPropertyChanged(sender, e);
    //}

    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    void IEditableCollection.RemoveChild(IEditableBusinessObject child)
    {
      Remove((C)child);
    }

    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    void Core.IParent.RemoveChild(Csla.Core.IEditableBusinessObject child)
    {
      Remove((C)child);
    }

    /// <summary>
    /// Sets the edit level of the child object as it is added.
    /// </summary>
    /// <param name="index">Index of the item to insert.</param>
    /// <param name="item">Item to insert.</param>
    protected override void InsertItem(int index, C item)
    {
      // set parent reference
      item.SetParent(this);
      // set child edit level
      Core.UndoableBase.ResetChildEditLevel(item, this.EditLevel, false);
      // when an object is inserted we assume it is
      // a new object and so the edit level when it was
      // added must be set
      item.EditLevelAdded = _editLevel;
      base.InsertItem(index, item);
    }

    /// <summary>
    /// Marks the child object for deletion and moves it to
    /// the collection of deleted objects.
    /// </summary>
    /// <param name="index">Index of the item to remove.</param>
    protected override void RemoveItem(int index)
    {
      // when an object is 'removed' it is really
      // being deleted, so do the deletion work
      C child = this[index];
      bool oldRaiseListChangedEvents = this.RaiseListChangedEvents;
      try
      {
        this.RaiseListChangedEvents = false;
        base.RemoveItem(index);
      }
      finally
      {
        this.RaiseListChangedEvents = oldRaiseListChangedEvents;
      }
      if (!_completelyRemoveChild)
      {
        // the child shouldn't be completely removed,
        // so copy it to the deleted list
        DeleteChild(child);
      }
      if (RaiseListChangedEvents)
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, child, index));
    }

    /// <summary>
    /// Replaces the item at the specified index with
    /// the specified item, first moving the original
    /// item to the deleted list.
    /// </summary>
    /// <param name="index">The zero-based index of the item to replace.</param>
    /// <param name="item">
    /// The new value for the item at the specified index. 
    /// The value can be null for reference types.
    /// </param>
    /// <remarks></remarks>
    protected override void SetItem(int index, C item)
    {
      C child = default(C);
      if (!(ReferenceEquals((C)(this[index]), item)))
        child = this[index];
      // replace the original object with this new
      // object
      bool oldRaiseListChangedEvents = this.RaiseListChangedEvents;
      try
      {
        this.RaiseListChangedEvents = false;
        // set parent reference
        item.SetParent(this);
        // set child edit level
        Core.UndoableBase.ResetChildEditLevel(item, this.EditLevel, false);
        // reset EditLevelAdded 
        item.EditLevelAdded = this.EditLevel;
        // add to list
        base.SetItem(index, item);
      }
      finally
      {
        this.RaiseListChangedEvents = oldRaiseListChangedEvents;
      }
      if (child != null)
        DeleteChild(child);
      if (RaiseListChangedEvents)
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, child, index));
    }

    /// <summary>
    /// Clears the collection, moving all active
    /// items to the deleted list.
    /// </summary>
    protected override void ClearItems()
    {
      while (base.Count > 0) RemoveItem(0);
      //DeferredLoadIndexIfNotLoaded();
      //_indexSet.ClearIndexes();
      //DeferredLoadPositionMapIfNotLoaded();
      //_positionMap.ClearMap();
      base.ClearItems();
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

    #region Mobile Object overrides

    protected override void OnGetState(SerializationInfo info)
    {
      info.AddValue("Csla.BusinessListBase._isChild", _isChild);
      info.AddValue("Csla.BusinessListBase._editLevel", _editLevel);
      base.OnGetState(info);
    }

    protected override void OnSetState(SerializationInfo info)
    {
      _isChild = info.GetValue<bool>("Csla.BusinessListBase._isChild");
      _editLevel = info.GetValue<int>("Csla.BusinessListBase._editLevel");
      base.OnSetState(info);
    }

    protected override void OnGetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      base.OnGetChildren(info, formatter);
      if (_deletedList != null)
      {
        var fieldManagerInfo = formatter.SerializeObject(_deletedList);
        info.AddChild("_deletedList", fieldManagerInfo.ReferenceId);
      }
    }

    protected override void OnSetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      if (info.Children.ContainsKey("_deletedList"))
      {
        var childData = info.Children["_deletedList"];
        _deletedList = (MobileList<C>)formatter.GetObject(childData.ReferenceId);
      }
      base.OnSetChildren(info, formatter);
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
        bool auth = Csla.Security.AuthorizationRules.CanEditObject(this.GetType());
        return (IsDirty && IsValid && auth);
      }
    }

    public override bool IsBusy
    {
      get
      {
        // any non-new deletions make us dirty
        foreach (C item in DeletedList)
          if (item.IsBusy)
            return true;

        // run through all the child objects
        // and if any are dirty then then
        // collection is dirty
        foreach (C child in this)
          if (child.IsBusy)
            return true;

        return false;
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

    #region Serialization Notification

    protected internal override void OnDeserializedInternal()
    {
      foreach (Core.IEditableBusinessObject child in this)
        child.SetParent(this);

      foreach (Core.IEditableBusinessObject child in DeletedList)
        child.SetParent(this);
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
    public void BeginSave()
    {
      BeginSave(null, null);
    }

    public void BeginSave(object userState)
    {
      BeginSave(null, userState);
    }

    public void BeginSave(EventHandler<SavedEventArgs> handler)
    {
      BeginSave(handler, null);
    }

    public virtual void BeginSave(EventHandler<SavedEventArgs> handler, object userState)
    {
      if (this.IsChild)
      {
        NotSupportedException error = new NotSupportedException(Resources.NoSaveChildException);
        OnSaved(null, error, userState);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error, userState));
      }
      else if (EditLevel > 0)
      {
        Validation.ValidationException error = new Validation.ValidationException(Resources.NoSaveEditingException);
        OnSaved(null, error, userState);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error, userState));
      }
      else if (!IsValid)
      {
        Validation.ValidationException error = new Validation.ValidationException(Resources.NoSaveEditingException);
        OnSaved(null, error, userState);
        if (handler != null)
          handler(this, new SavedEventArgs(null, error, userState));
      }
      else
      {
        if (IsDirty)
        {
          if (userState == null)
          {
            DataPortal.BeginUpdate<T>(this, (o, e) =>
            {
              T result = e.Object;
              OnSaved(result, e.Error, userState);
              if (handler != null)
                handler(result, new SavedEventArgs(result, e.Error, userState));
            });
          }
          else
          {
            DataPortal.BeginUpdate<T>(this, (o, e) =>
            {
              T result = e.Object;
              OnSaved(result, e.Error, e.UserState);
              if (handler != null)
                handler(result, new SavedEventArgs(result, e.Error, e.UserState));
            }, userState);
          }
        }
        else
        {
          OnSaved((T)this, null, userState);
          if (handler != null)
            handler(this, new SavedEventArgs(this, null, userState));
        }
      }
    }

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Create(Csla.DataPortalClient.LocalProxy<T>.CompletedHandler handler)
    {
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

    void ISavable.BeginSave()
    {
      BeginSave();
    }

    void ISavable.SaveComplete(object newObject, Exception error, object userState)
    {
      OnSaved((T)newObject, error, userState);
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
    protected void OnSaved(T newObject, Exception error, object userState)
    {
      if (Saved != null)
        Saved(this, new SavedEventArgs(newObject, error, userState));
    }

    #endregion

    #region  Parent/Child link

    [NotUndoable(), NonSerialized()]
    private Core.IParent _parent;

    /// <summary>
    /// Provide access to the parent reference for use
    /// in child object code.
    /// </summary>
    /// <remarks>
    /// This value will be Nothing for root objects.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected Core.IParent Parent
    {
      get
      {
        return _parent;
      }
    }

    /// <summary>
    /// Used by BusinessListBase as a child object is 
    /// created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    void Core.IEditableCollection.SetParent(Core.IParent parent)
    {
      _parent = parent;
    }

    #endregion

    #region IDataPortalTarget Members

    void IDataPortalTarget.MarkAsChild()
    {
      this.MarkAsChild();
    }

    void IDataPortalTarget.MarkNew()
    { }

    void IDataPortalTarget.MarkOld()
    { }

    void IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.DataPortal_OnDataPortalException(e, ex);
    }

    void IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.Child_OnDataPortalException(e, ex);
    }

    #endregion

  }
}
