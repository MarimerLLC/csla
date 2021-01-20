//-----------------------------------------------------------------------
// <copyright file="BusinessBindingListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which most business collections</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Csla.Properties;
using Csla.Core;
using System.Threading.Tasks;

namespace Csla
{
  /// <summary>
  /// This is the base class from which most business collections
  /// or lists will be derived.
  /// </summary>
  /// <typeparam name="T">Type of the business object being defined.</typeparam>
  /// <typeparam name="C">Type of the child objects contained in the list.</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable()]
  public abstract class BusinessBindingListBase<T, C> :
      Core.ExtendedBindingList<C>,
      Core.IEditableCollection, Core.IUndoableObject, ICloneable,
      Core.ISavable, Core.ISavable<T>, Core.IParent, Server.IDataPortalTarget,
      INotifyBusy
    where T : BusinessBindingListBase<T, C>
    where C : Core.IEditableBusinessObject
  {

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected BusinessBindingListBase()
    {
      InitializeIdentity();
      Initialize();
      this.AllowNew = true;
    }


#region Initialize

    /// <summary>
    /// Override this method to set up event handlers so user
    /// code in a partial class can respond to events raised by
    /// generated code.
    /// </summary>
    protected virtual void Initialize()
    { /* allows subclass to initialize events before any other activity occurs */ }

    #endregion

    #region Identity

    private int _identity = -1;

    int IBusinessObject.Identity
    {
      get { return _identity; }
    }

    private void InitializeIdentity()
    {
      _identity = ((IParent)this).GetNextIdentity(_identity);
    }

    [NonSerialized]
    [NotUndoable]
    private IdentityManager _identityManager;

    int IParent.GetNextIdentity(int current)
    {
      if (this.Parent != null)
      {
        return this.Parent.GetNextIdentity(current);
      }
      else
      {
        if (_identityManager == null)
          _identityManager = new IdentityManager();
        return _identityManager.GetNextIdentity(current);
      }
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
    [Browsable(false)]
    [System.ComponentModel.DataAnnotations.Display(AutoGenerateField = false)]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
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
    [Browsable(false)]
    [System.ComponentModel.DataAnnotations.Display(AutoGenerateField = false)]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
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
    /// Returns true if this object is both dirty and valid.
    /// </summary>
    /// <returns>A value indicating if this object is both dirty and valid.</returns>
    [Browsable(false)]
    [System.ComponentModel.DataAnnotations.Display(AutoGenerateField = false)]
    public virtual bool IsSavable
    {
      get 
      {
        bool auth = Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, this);
        return (IsDirty && IsValid && auth && !IsBusy);
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

    IParent Csla.Core.IParent.Parent
    {
      get { return this.Parent; }
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
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"), this.GetType().Name, _parent != null ? _parent.GetType().Name : null, this.EditLevel, parentEditLevel - 1);

      // we are going a level deeper in editing
      _editLevel += 1;

      // cascade the call to all child objects
      foreach (C child in this)
        child.CopyState(_editLevel, false);

      // cascade the call to all deleted child objects
      foreach (C child in DeletedList)
        child.CopyState(_editLevel, false);
    }

    private bool _completelyRemoveChild;

    private void UndoChanges(int parentEditLevel)
    {
      C child;

      if (this.EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"), this.GetType().Name, _parent != null ? _parent.GetType().Name : null, this.EditLevel, parentEditLevel + 1);

      // we are coming up one edit level
      _editLevel -= 1;
      if (_editLevel < 0) _editLevel = 0;

      try
      {
        using (LoadListMode)
        {
          // Cancel edit on all current items
          for (int index = Count - 1; index >= 0; index--)
          {
            child = this[index];

            child.UndoChanges(_editLevel, false);

            // if item is below its point of addition, remove
            if (child.EditLevelAdded > _editLevel)
            {
              bool oldAllowRemove = this.AllowRemove;
              try
              {
                this.AllowRemove = true;
                _completelyRemoveChild = true;
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
      }
      finally
      {
        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
      }
    }

    private void AcceptChanges(int parentEditLevel)
    {
      if (this.EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"), this.GetType().Name, _parent != null ? _parent.GetType().Name : null, this.EditLevel, parentEditLevel + 1);

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
      Add(child);
      child.EditLevelAdded = saveLevel;
    }

    /// <summary>
    /// Returns true if the internal deleted list
    /// contains the specified child object.
    /// </summary>
    /// <param name="item">Child object to check.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool ContainsDeleted(C item)
    {
      return DeletedList.Contains(item);
    }

#endregion

#region Insert, Remove, Clear

    /// <summary>
    /// Override this method to create a new object that is added
    /// to the collection. 
    /// </summary>
    protected override object AddNewCore()
    {
      var item = DataPortal.CreateChild<C>();
      Add(item);
      return item;
    }

    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    void Core.IEditableCollection.RemoveChild(Csla.Core.IEditableBusinessObject child)
    {
      Remove((C)child);
    }

    object IEditableCollection.GetDeletedList()
    {
      return DeletedList;
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
      using (LoadListMode)
      {
        base.RemoveItem(index);
      }
      if (!_completelyRemoveChild)
      {
        // the child shouldn't be completely removed,
        // so copy it to the deleted list
        DeleteChild(child);
      }
      if (RaiseListChangedEvents)
        OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
    }

    /// <summary>
    /// Clears the collection, moving all active
    /// items to the deleted list.
    /// </summary>
    protected override void ClearItems()
    {
      while (base.Count > 0) 
        RemoveItem(0);
      base.ClearItems();
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
      using (LoadListMode)
      {
        // set parent reference
        item.SetParent(this);
        // set child edit level
        Core.UndoableBase.ResetChildEditLevel(item, this.EditLevel, false);
        // reset EditLevelAdded 
        item.EditLevelAdded = this.EditLevel;

        // add to list
        base.SetItem(index, item);
      }
      if (child != null)
        DeleteChild(child);
      if (RaiseListChangedEvents)
        OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
    }

#endregion

#region Cascade child events

    /// <summary>
    /// Handles any PropertyChanged event from 
    /// a child object and echoes it up as
    /// a ListChanged event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (_deserialized && RaiseListChangedEvents && e != null)
      {
        for (int index = 0; index < Count; index++)
        {
          if (ReferenceEquals(this[index], sender))
          {
            PropertyDescriptor descriptor = GetPropertyDescriptor(e.PropertyName);
            if (descriptor != null)
              OnListChanged(new ListChangedEventArgs(
                ListChangedType.ItemChanged, index, descriptor));
            else
              OnListChanged(new ListChangedEventArgs(
                ListChangedType.ItemChanged, index));
          }
        }
      }
      base.Child_PropertyChanged(sender, e);
    }

    private static PropertyDescriptorCollection _propertyDescriptors;

    private PropertyDescriptor GetPropertyDescriptor(string propertyName)
    {
      if (_propertyDescriptors == null)
        _propertyDescriptors = TypeDescriptor.GetProperties(typeof(C));
      PropertyDescriptor result = null;
      foreach (PropertyDescriptor desc in _propertyDescriptors)
        if (desc.Name == propertyName)
        {
          result = desc;
          break;
        }
      return result;
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

#region IsChild

    [NotUndoable()]
    private bool _isChild = false;

    /// <summary>
    /// Indicates whether this collection object is a child object.
    /// </summary>
    /// <returns>True if this is a child object.</returns>
    [Browsable(false)]
    [System.ComponentModel.DataAnnotations.Display(AutoGenerateField = false)]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
    public bool IsChild
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
      _identity = -1;
      _isChild = true;
    }

#endregion

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

#region Serialization Notification

    [NonSerialized]
    [NotUndoable]
    private bool _deserialized = false;

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnDeserialized()
    {
      _deserialized = true;
      base.OnDeserialized();

      foreach (Core.IEditableBusinessObject child in this)
      {
        child.SetParent(this);
      }
      
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
    protected virtual void Child_Update(params object[] parameters)
    {
      using (LoadListMode)
      {
        foreach (var child in DeletedList)
          DataPortal.UpdateChild(child, parameters);
        DeletedList.Clear();

        foreach (var child in this)
          if (child.IsDirty) DataPortal.UpdateChild(child, parameters);
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
    /// this value is false, no data operation occurs. 
    /// It is also contingent on <see cref="IsValid" />. If this value is 
    /// false an exception will be thrown to 
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
    public T Save()
    {
      try
      {
        return SaveAsync(null, true).Result;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          throw ex.InnerExceptions[0];
        else
          throw;
      }
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    public async Task<T> SaveAsync()
    {
      return await SaveAsync(null, false);
    }

    /// <summary>
    /// Saves the object to the database, merging
    /// any resulting updates into the existing
    /// object graph.
    /// </summary>
    public Task SaveAndMergeAsync()
    {
      throw new NotSupportedException(nameof(SaveAndMergeAsync));
    }

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <param name="userState">User state data.</param>
    /// <param name="isSync">True if the save operation should be synchronous.</param>
    protected virtual async Task<T> SaveAsync(object userState, bool isSync)
    {
      T result;
      if (this.IsChild)
        throw new InvalidOperationException(Resources.NoSaveChildException);

      if (_editLevel > 0)
        throw new InvalidOperationException(Resources.NoSaveEditingException);

      if (!IsValid)
        throw new Rules.ValidationException(Resources.NoSaveInvalidException);

      if (IsBusy)
        throw new InvalidOperationException(Resources.BusyObjectsMayNotBeSaved);

      if (IsDirty)
      {
        if (isSync)
        {
          result = DataPortal.Update<T>((T)this);
        }
        else
        {
          result = await DataPortal.UpdateAsync<T>((T)this);
        }
      }
      else
      {
        result = (T)this;
      }
      OnSaved(result, null, userState);
      return result;
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    [Obsolete]
    public void BeginSave()
    {
      BeginSave(null, null);
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public void BeginSave(object userState)
    {
      BeginSave(null, userState);
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    /// <param name="handler">
    /// Method called when the operation is complete.
    /// </param>
    [Obsolete]
    public void BeginSave(EventHandler<SavedEventArgs> handler)
    {
      BeginSave(handler, null);
    }

    /// <summary>
    /// Starts an async operation to save the object to the database.
    /// </summary>
    /// <param name="handler">
    /// Method called when the operation is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    [Obsolete]
    public async void BeginSave(EventHandler<SavedEventArgs> handler, object userState)
    {
      T result = default(T);
      Exception error = null;
      try
      {
        result = await SaveAsync(userState, false);
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          error = ex.InnerExceptions[0];
        else
          error = ex;
      }
      catch (Exception ex)
      {
        error = ex;
      }
      if (handler != null)
        handler(this, new SavedEventArgs(result, error, userState));
    }

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Create()
    { }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Resources.FetchNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow update of a business
    /// object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow immediate deletion of a business object.
    /// </summary>
    /// <param name="criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
    }

#endregion

#region ISavable Members

    object Csla.Core.ISavable.Save()
    {
      return Save();
    }

    object Csla.Core.ISavable.Save(bool forceUpdate)
    {
      return Save();
    }

    async Task<object> ISavable.SaveAsync()
    {
      return await SaveAsync();
    }

    async Task<object> ISavable.SaveAsync(bool forceUpdate)
    {
      return await SaveAsync();
    }

    async Task ISavable.SaveAndMergeAsync(bool forceUpdate)
    {
      await SaveAndMergeAsync();
    }

    void Csla.Core.ISavable.SaveComplete(object newObject)
    {
      OnSaved((T)newObject, null, null);
    }

    T Csla.Core.ISavable<T>.Save(bool forceUpdate)
    {
      return Save();
    }

    async Task<T> ISavable<T>.SaveAsync(bool forceUpdate)
    {
      return await SaveAsync();
    }

    async Task ISavable<T>.SaveAndMergeAsync(bool forceUpdate)
    {
      await SaveAndMergeAsync();
    }

    void Csla.Core.ISavable<T>.SaveComplete(T newObject)
    {
      OnSaved(newObject, null, null);
    }

    [NonSerialized()]
    [NotUndoable]
    private EventHandler<Csla.Core.SavedEventArgs> _nonSerializableSavedHandlers;
    [NotUndoable]
    private EventHandler<Csla.Core.SavedEventArgs> _serializableSavedHandlers;

    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event EventHandler<Csla.Core.SavedEventArgs> Saved
    {
      add
      {
        if (value.Method.IsPublic &&
           (value.Method.DeclaringType.IsSerializable ||
            value.Method.IsStatic))
          _serializableSavedHandlers = (EventHandler<Csla.Core.SavedEventArgs>)
            System.Delegate.Combine(_serializableSavedHandlers, value);
        else
          _nonSerializableSavedHandlers = (EventHandler<Csla.Core.SavedEventArgs>)
            System.Delegate.Combine(_nonSerializableSavedHandlers, value);
      }
      remove
      {
        if (value.Method.IsPublic &&
           (value.Method.DeclaringType.IsSerializable ||
            value.Method.IsStatic))
          _serializableSavedHandlers = (EventHandler<Csla.Core.SavedEventArgs>)
            System.Delegate.Remove(_serializableSavedHandlers, value);
        else
          _nonSerializableSavedHandlers = (EventHandler<Csla.Core.SavedEventArgs>)
            System.Delegate.Remove(_nonSerializableSavedHandlers, value);
      }
    }

    /// <summary>
    /// Raises the <see cref="Saved"/> event, indicating that the
    /// object has been saved, and providing a reference
    /// to the new object instance.
    /// </summary>
    /// <param name="newObject">The new object instance.</param>
    /// <param name="e">Execption that occurred during the operation.</param>
    /// <param name="userState">User state object.</param>
    [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSaved(T newObject, Exception e, object userState)
    {
      Csla.Core.SavedEventArgs args = new Csla.Core.SavedEventArgs(newObject, e, userState);
      if (_nonSerializableSavedHandlers != null)
        _nonSerializableSavedHandlers.Invoke(this, args);
      if (_serializableSavedHandlers != null)
        _serializableSavedHandlers.Invoke(this, args);
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
    [Browsable(false)]
    [System.ComponentModel.DataAnnotations.Display(AutoGenerateField = false)]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Core.IParent Parent
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
    protected virtual void SetParent(Core.IParent parent)
    {
      _parent = parent;
      _identityManager = null;
      InitializeIdentity();
    }

    /// <summary>
    /// Used by BusinessListBase as a child object is 
    /// created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    void Core.IEditableCollection.SetParent(Core.IParent parent)
    {
      this.SetParent(parent);
    }

#endregion

#region ToArray

    /// <summary>
    /// Get an array containing all items in the list.
    /// </summary>
    public C[] ToArray()
    {
      List<C> result = new List<C>();
      foreach (C item in this)
        result.Add(item);
      return result.ToArray();
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

    /// <summary>
    /// Gets the busy status for this object and its child objects.
    /// </summary>
    [Browsable(false)]
    [System.ComponentModel.DataAnnotations.Display(AutoGenerateField = false)]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
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

#region IDataPortalTarget Members

    void Csla.Server.IDataPortalTarget.CheckRules()
    { }

    void Csla.Server.IDataPortalTarget.MarkAsChild()
    {
      this.MarkAsChild();
    }

    void Csla.Server.IDataPortalTarget.MarkNew()
    {    }

    void Csla.Server.IDataPortalTarget.MarkOld()
    {    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvoke(e);
    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvokeComplete(e);
    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.DataPortal_OnDataPortalException(e, ex);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvoke(e);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvokeComplete(e);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.Child_OnDataPortalException(e, ex);
    }

#endregion

#region Mobile object overrides

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      _isChild = info.GetValue<bool>("Csla.BusinessListBase._isChild");
      _editLevel = info.GetValue<int>("Csla.BusinessListBase._editLevel");
      _identity = info.GetValue<int>("Csla.Core.BusinessBase._identity");
      base.OnSetState(info);
    }

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      info.AddValue("Csla.BusinessListBase._isChild", _isChild);
      info.AddValue("Csla.BusinessListBase._editLevel", _editLevel);
      info.AddValue("Csla.Core.BusinessBase._identity", _identity);
      base.OnGetState(info);
    }

    /// <summary>
    /// Override this method to insert child objects
    /// into the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to the current SerializationFormatterFactory.GetFormatter().
    /// </param>
    [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnGetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      base.OnGetChildren(info, formatter);
      if (_deletedList != null)
      {
        var fieldManagerInfo = formatter.SerializeObject(_deletedList);
        info.AddChild("_deletedList", fieldManagerInfo.ReferenceId);
      }
    }

    /// <summary>
    /// Override this method to get child objects
    /// from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the current SerializationFormatterFactory.GetFormatter().
    /// </param>
    [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Advanced)]
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
  }
}
