//-----------------------------------------------------------------------
// <copyright file="BusinessListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which most business collections</summary>
//-----------------------------------------------------------------------

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Properties;
using Csla.Serialization.Mobile;
using Csla.Server;

namespace Csla
{
  /// <summary>
  /// This is the base class from which most business collections
  /// or lists will be derived.
  /// </summary>
  /// <typeparam name="T">Type of the business object being defined.</typeparam>
  /// <typeparam name="C">Type of the child objects contained in the list.</typeparam>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public abstract class BusinessListBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] C> :
      ObservableBindingList<C>,
      IContainsDeletedList,
      ISavable<T>,
      IDataPortalTarget,
      IBusinessListBase<C>,
      IUseApplicationContext
    where T : BusinessListBase<T, C>
    where C : IEditableBusinessObject
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    protected BusinessListBase()
    {
      ApplicationContext = default!;
    }

    /// <summary>
    /// Gets the current ApplicationContext
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }

    /// <inheritdoc />
    ApplicationContext IUseApplicationContext.ApplicationContext
    {
      get => ApplicationContext;
      set
      {
        ApplicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext));
        InitializeIdentity();
        Initialize();
        AllowNew = true;
      }
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
    private IdentityManager? _identityManager;

    int IParent.GetNextIdentity(int current)
    {
      if (Parent != null)
      {
        return Parent.GetNextIdentity(current);
      }
      else
      {
        if (_identityManager == null)
          _identityManager = new IdentityManager();
        return _identityManager.GetNextIdentity(current);
      }
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
      return ObjectCloner.GetInstance(ApplicationContext).Clone(this)!;
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

    private MobileList<C>? _deletedList;

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    IEnumerable<IEditableBusinessObject> IContainsDeletedList.DeletedList => (IEnumerable<IEditableBusinessObject>)DeletedList;

    private void DeleteChild(C child)
    {
      // set child edit level
      UndoableBase.ResetChildEditLevel(child, EditLevel, false);

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
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool ContainsDeleted(C item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

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
      if (IsChild)
        throw new NotSupportedException(Resources.NoBeginEditChildException);

      CopyState(EditLevel + 1);
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
      if (IsChild)
        throw new NotSupportedException(Resources.NoCancelEditChildException);

      UndoChanges(EditLevel - 1);
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
      if (IsChild)
        throw new NotSupportedException(Resources.NoApplyEditChildException);

      AcceptChanges(EditLevel - 1);
    }

    /// <inheritdoc />
    Task IParent.ApplyEditChild(IEditableBusinessObject child)
    {
      if (child is null)
        throw new ArgumentNullException(nameof(child));

      EditChildComplete(child);
      return Task.CompletedTask;
    }

    IParent? IParent.Parent
    {
      get { return Parent; }
    }

    /// <summary>
    /// Override this method to be notified when a child object's
    /// <see cref="Core.BusinessBase.ApplyEdit" /> method has
    /// completed.
    /// </summary>
    /// <param name="child">The child object that was edited.</param>
    protected virtual void EditChildComplete(IEditableBusinessObject child)
    {

      // do nothing, we don't really care
      // when a child has its edits applied
    }

    #endregion

    #region Insert, Remove, Clear

    /// <summary>
    /// Override this method to create a new object that is added
    /// to the collection. 
    /// </summary>
    protected override C AddNewCore()
    {
      var dp = ApplicationContext.CreateInstanceDI<DataPortal<C>>();
      var item = dp.CreateChild();
      Add(item);
      return item;
    }

    /// <summary>
    /// Override this method to create a new object that is added
    /// to the collection. 
    /// </summary>
    protected override async Task<C> AddNewCoreAsync()
    {
      var dp = ApplicationContext.CreateInstanceDI<DataPortal<C>>();
      var item = await dp.CreateChildAsync();
      Add(item);
      return item;
    }

    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    /// <exception cref="ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
    void IEditableCollection.RemoveChild(IEditableBusinessObject child)
    {
      if (child is null)
        throw new ArgumentNullException(nameof(child));

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
    /// <exception cref="ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
    Task IParent.RemoveChild(IEditableBusinessObject child)
    {
      if (child is null)
        throw new ArgumentNullException(nameof(child));

      Remove((C)child);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the edit level of the child object as it is added.
    /// </summary>
    /// <param name="index">Index of the item to insert.</param>
    /// <param name="item">Item to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    protected override void InsertItem(int index, C item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      if (item.IsChild)
      {
        IdentityManager.EnsureNextIdentityValueIsUnique(this, this);

        // set parent reference
        item.SetParent(this);
        // ensure child uses same context as parent
        if (item is IUseApplicationContext iuac)
          iuac.ApplicationContext = ApplicationContext;
        // set child edit level
        UndoableBase.ResetChildEditLevel(item, EditLevel, false);
        // when an object is inserted we assume it is
        // a new object and so the edit level when it was
        // added must be set
        item.EditLevelAdded = EditLevel;
        base.InsertItem(index, item);
      }
      else
      {
        // item must be marked as a child object
        throw new InvalidOperationException(Resources.ListItemNotAChildException);
      }
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
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    protected override void SetItem(int index, C item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      C? child = default;
      if (!(ReferenceEquals(this[index], item)))
        child = this[index];
      // replace the original object with this new
      // object
      using (LoadListMode)
      {
        if (child != null)
          DeleteChild(child);
      }

      // set parent reference
      item.SetParent(this);
      // set child edit level
      UndoableBase.ResetChildEditLevel(item, EditLevel, false);
      // reset EditLevelAdded 
      item.EditLevelAdded = EditLevel;
      // add to list and raise list changed as appropriate
      base.SetItem(index, item);
    }

    /// <summary>
    /// Clears the collection, moving all active
    /// items to the deleted list.
    /// </summary>
    protected override void ClearItems()
    {
      while (Count > 0) RemoveItem(0);
      //DeferredLoadIndexIfNotLoaded();
      //_indexSet.ClearIndexes();
      //DeferredLoadPositionMapIfNotLoaded();
      //_positionMap.ClearMap();
      base.ClearItems();
    }

    #endregion

    #region Edit level tracking

    // keep track of how many edit levels we have

    /// <summary>
    /// Returns the current edit level of the object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int EditLevel { get; private set; }

    int IUndoableObject.EditLevel
    {
      get
      {
        return EditLevel;
      }
    }

    #endregion

    #region N-level undo

    void IUndoableObject.CopyState(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        CopyState(parentEditLevel);
    }

    void IUndoableObject.UndoChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        UndoChanges(parentEditLevel);
    }

    void IUndoableObject.AcceptChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        AcceptChanges(parentEditLevel);
    }

    private void CopyState(int parentEditLevel)
    {
      if (EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"), GetType().Name, _parent?.GetType().Name, EditLevel, parentEditLevel - 1);

      // we are going a level deeper in editing
      EditLevel += 1;

      // cascade the call to all child objects
      for (int x = 0; x < Count; x++)
      {
        C child = this[x];
        child.CopyState(EditLevel, false);
      }

      // cascade the call to all deleted child objects
      foreach (C child in DeletedList)
        child.CopyState(EditLevel, false);
    }

    private bool _completelyRemoveChild;

    private void UndoChanges(int parentEditLevel)
    {
      C child;

      if (EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"), GetType().Name, _parent?.GetType().Name, EditLevel, parentEditLevel + 1);

      // we are coming up one edit level
      EditLevel -= 1;
      if (EditLevel < 0) EditLevel = 0;

      using (LoadListMode)
      {
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

            child.UndoChanges(EditLevel, false);

            //ACE: Now that we have undone the changes, we can add the item
            //     back in the index.
            //_indexSet.InsertItem(child);

            // if item is below its point of addition, remove
            if (child.EditLevelAdded > EditLevel)
            {
              bool oldAllowRemove = AllowRemove;
              try
              {
                AllowRemove = true;
                _completelyRemoveChild = true;
                //RemoveIndexItem(child);
                RemoveAt(index);
              }
              finally
              {
                _completelyRemoveChild = false;
                AllowRemove = oldAllowRemove;
              }
            }
          }

          // cancel edit on all deleted items
          for (int index = DeletedList.Count - 1; index >= 0; index--)
          {
            child = DeletedList[index];
            child.UndoChanges(EditLevel, false);
            if (child.EditLevelAdded > EditLevel)
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
          OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
      }
    }

    private void AcceptChanges(int parentEditLevel)
    {
      if (EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"), GetType().Name, _parent?.GetType().Name, EditLevel, parentEditLevel + 1);

      // we are coming up one edit level
      EditLevel -= 1;

      // cascade the call to all child objects
      foreach (C child in this)
      {
        child.AcceptChanges(EditLevel, false);
        // if item is below its point of addition, lower point of addition
        if (child.EditLevelAdded > EditLevel) child.EditLevelAdded = EditLevel;
      }

      // cascade the call to all deleted child objects
      for (int index = DeletedList.Count - 1; index >= 0; index--)
      {
        C child = DeletedList[index];
        child.AcceptChanges(EditLevel, false);
        // if item is below its point of addition, remove
        if (child.EditLevelAdded > EditLevel)
          DeletedList.RemoveAt(index);
      }
      if (EditLevel < 0) EditLevel = 0;
    }

    #endregion

    #region Mobile Object overrides

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should serialize its data. The data should be
    /// serialized into the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object to contain the serialized data.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
    protected override void OnGetState(SerializationInfo info)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      info.AddValue("Csla.BusinessListBase._isChild", _isChild);
      info.AddValue("Csla.BusinessListBase._editLevel", EditLevel);
      info.AddValue("Csla.Core.BusinessBase._identity", _identity);
      base.OnGetState(info);
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should be deserialized. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
    protected override void OnSetState(SerializationInfo info)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      _isChild = info.GetValue<bool>("Csla.BusinessListBase._isChild");
      EditLevel = info.GetValue<int>("Csla.BusinessListBase._editLevel");
      _identity = info.GetValue<int>("Csla.Core.BusinessBase._identity");
      base.OnSetState(info);
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should serialize its child references. The data should be
    /// serialized into the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object to contain the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the formatter performing the serialization.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> or <paramref name="formatter"/> is <see langword="null"/>.</exception>
    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));
      if (formatter is null)
        throw new ArgumentNullException(nameof(formatter));

      base.OnGetChildren(info, formatter);
      if (_deletedList != null)
      {
        var fieldManagerInfo = formatter.SerializeObject(_deletedList);
        info.AddChild("_deletedList", fieldManagerInfo.ReferenceId);
      }
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should deserialize its child references. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the formatter performing the deserialization.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> or <paramref name="formatter"/> is <see langword="null"/>.</exception>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));
      if (formatter is null)
        throw new ArgumentNullException(nameof(formatter));

      if (info.Children.TryGetValue("_deletedList", out var child))
      {
        _deletedList = (MobileList<C>?)formatter.GetObject(child.ReferenceId);
      }
      base.OnSetChildren(info, formatter);
    }

    #endregion

    #region IsChild

    [NotUndoable]
    private bool _isChild = false;

    /// <summary>
    /// Indicates whether this collection object is a child object.
    /// </summary>
    /// <returns>True if this is a child object.</returns>
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

    #region IsDirty, IsValid, IsSavable

    /// <summary>
    /// Await this method to ensure business object is not busy.
    /// </summary>
    public async Task WaitForIdle()
    {
      var cslaOptions = ApplicationContext.GetRequiredService<Configuration.CslaOptions>();
      await WaitForIdle(TimeSpan.FromSeconds(cslaOptions.DefaultWaitForIdleTimeoutInSeconds)).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a value indicating whether this object's data has been changed.
    /// </summary>
    bool ITrackStatus.IsSelfDirty
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

    bool ITrackStatus.IsSelfValid
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
    /// Returns true if this object has changes, is valid,
    /// the user is authorized and the object is not busy.
    /// </summary>
    public virtual bool IsSavable
    {
      get
      {
        var result = IsDirty && IsValid && !IsBusy;
        if (result)
          result = Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, this);

        return result;
      }
    }

    /// <summary>
    /// Gets the busy status for this object and its child objects.
    /// </summary>
    public override bool IsBusy
    {
      get
      {
        // run through all the child objects
        // and if any are busy then then
        // collection is busy
        foreach (C item in DeletedList)
          if (item.IsBusy)
            return true;

        foreach (C child in this)
          if (child.IsBusy)
            return true;

        return false;
      }
    }

    #endregion

    #region  ITrackStatus

    bool ITrackStatus.IsNew
    {
      get
      {
        return false;
      }
    }

    bool ITrackStatus.IsDeleted
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
    /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_Update(params object?[] parameters)
    {
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      using (LoadListMode)
      {
        var dp = ApplicationContext.CreateInstanceDI<DataPortal<C>>();
        foreach (var child in DeletedList)
          dp.UpdateChild(child, parameters);
        DeletedList.Clear();

        foreach (var child in this)
          if (child.IsDirty) dp.UpdateChild(child, parameters);
      }
    }

    /// <summary>
    /// Asynchronously saves all items in the list, automatically
    /// performing insert, update or delete operations as necessary.
    /// </summary>
    /// <param name="parameters">
    /// Optional parameters passed to child update
    /// methods.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [UpdateChild]
    protected virtual async Task Child_UpdateAsync(params object?[] parameters)
    {
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      using (LoadListMode)
      {
        var dp = ApplicationContext.CreateInstanceDI<DataPortal<C>>();
        foreach (var child in DeletedList)
          await dp.UpdateChildAsync(child, parameters).ConfigureAwait(false);
        DeletedList.Clear();

        foreach (var child in this)
          if (child.IsDirty) await dp.UpdateChildAsync(child, parameters).ConfigureAwait(false);
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
    /// Saves the object to the database.
    /// </summary>
    /// <param name="userState">User state data.</param>
    /// <param name="isSync">True if the save operation should be synchronous.</param>
    protected virtual async Task<T> SaveAsync(object? userState, bool isSync)
    {
      T result;
      if (IsChild)
        throw new InvalidOperationException(Resources.NoSaveChildException);

      if (EditLevel > 0)
        throw new InvalidOperationException(Resources.NoSaveEditingException);

      if (!IsValid)
        throw new Rules.ValidationException(Resources.NoSaveInvalidException);

      if (IsBusy)
        throw new InvalidOperationException(Resources.BusyObjectsMayNotBeSaved);

      if (IsDirty)
      {
        var dp = ApplicationContext.CreateInstanceDI<DataPortal<T>>();
        if (isSync)
        {
          result = dp.Update((T)this);
        }
        else
        {
          result = await dp.UpdateAsync((T)this);
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
    /// Saves the object to the database, merging
    /// any resulting updates into the existing
    /// object graph.
    /// </summary>
    public async Task SaveAndMergeAsync()
    {
      await new GraphMerger(ApplicationContext).MergeBusinessListGraphAsync<T, C>((T)this, await SaveAsync());
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    #endregion

    #region ISavable Members

    object ISavable.Save()
    {
      return Save();
    }

    object ISavable.Save(bool forceUpdate)
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

    Task ISavable.SaveAndMergeAsync(bool forceUpdate)
    {
      return SaveAndMergeAsync();
    }

    void ISavable.SaveComplete(object newObject)
    {
      OnSaved((T)newObject, null, null);
    }

    T ISavable<T>.Save(bool forceUpdate)
    {
      return Save();
    }

    async Task<T> ISavable<T>.SaveAsync(bool forceUpdate)
    {
      return await SaveAsync();
    }

    Task ISavable<T>.SaveAndMergeAsync(bool forceUpdate)
    {
      return SaveAndMergeAsync();
    }

    void ISavable<T>.SaveComplete(T newObject)
    {
      OnSaved(newObject, null, null);
    }

    [NonSerialized]
    [NotUndoable]
    private EventHandler<SavedEventArgs>? _savedEvent;

    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event EventHandler<SavedEventArgs>? Saved
    {
      add
      {
        _savedEvent = (EventHandler<SavedEventArgs>?)Delegate.Combine(_savedEvent, value);
      }
      remove
      {
        _savedEvent = (EventHandler<SavedEventArgs>?)Delegate.Remove(_savedEvent, value);
      }
    }

    /// <summary>
    /// Raises the <see cref="Saved"/> event, indicating that the
    /// object has been saved, and providing a reference
    /// to the new object instance.
    /// </summary>
    /// <param name="newObject">The new object instance.</param>
    /// <param name="e">Exception that occurred during the operation.</param>
    /// <param name="userState">User state object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newObject"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSaved(T newObject, Exception? e, object? userState)
    {
      if (newObject is null)
        throw new ArgumentNullException(nameof(newObject));

      SavedEventArgs args = new SavedEventArgs(newObject, e, userState);
      _savedEvent?.Invoke(this, args);
    }
    #endregion

    #region  Parent/Child link

    [NotUndoable, NonSerialized]
    private IParent? _parent;

    /// <summary>
    /// Provide access to the parent reference for use
    /// in child object code.
    /// </summary>
    /// <remarks>
    /// This value will be Nothing for root objects.
    /// </remarks>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IParent? Parent
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
    protected virtual void SetParent(IParent? parent)
    {
      // ensure parent & _identity manager not null for case to check
      if (parent != null && _identityManager != null)
      {
        // changed GetNextIdentity to accept optional parameter to return next identity
        int current = _identityManager.GetNextIdentity();
        // attempt to find case where we decrement next identity so none are skipped - not necessary
        if (current > 0)
          --current;

        // ensure parent next identity accounts for this (child collection) next identity
        parent.GetNextIdentity(current);

        _parent = parent;
        _identityManager = null;
      }
      else
      {
        // case when current identity manager has next identity of > 1
        // parent next identity incremented by 1 not accounting for this (child collection) next identity
        _parent = parent;
        _identityManager = null;
        InitializeIdentity();
      }
    }

    /// <summary>
    /// Used by BusinessListBase as a child object is 
    /// created to tell the child object about its
    /// parent.
    /// </summary>
    /// <param name="parent">A reference to the parent collection object.</param>
    void IEditableCollection.SetParent(IParent? parent)
    {
      SetParent(parent);
    }

    #endregion

    #region IDataPortalTarget Members

    void IDataPortalTarget.CheckRules()
    { }

    Task IDataPortalTarget.CheckRulesAsync() => Task.CompletedTask;

    void IDataPortalTarget.MarkAsChild()
    {
      MarkAsChild();
    }

    void IDataPortalTarget.MarkNew()
    { }

    void IDataPortalTarget.MarkOld()
    { }

    void IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      DataPortal_OnDataPortalException(e, ex);
    }

    void IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      Child_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      Child_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      Child_OnDataPortalException(e, ex);
    }

    #endregion

  }
}