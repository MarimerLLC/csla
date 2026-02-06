//-----------------------------------------------------------------------
// <copyright file="BusinessDocumentBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class combining BusinessBase and BusinessListBase capabilities.</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Csla.Core;
using Csla.Core.FieldManager;
using Csla.Properties;
using Csla.Reflection;
using Csla.Serialization.Mobile;
using Csla.Server;

namespace Csla
{
  /// <summary>
  /// Base class for an editable business object that has its own
  /// properties AND contains a collection of child items.
  /// Combines the capabilities of both <see cref="BusinessBase{T}"/>
  /// and <see cref="BusinessListBase{T,C}"/>.
  /// </summary>
  /// <typeparam name="T">Type of the business object being defined.</typeparam>
  /// <typeparam name="C">Type of the child objects contained in the collection.</typeparam>
  [Serializable]
  public abstract class BusinessDocumentBase<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] C> :
    BusinessBase<T>,
    IEditableCollection,
    IContainsDeletedList,
    IObservableBindingList,
    INotifyCollectionChanged,
    IList<C>,
    IBusinessDocumentBase<C>
    where T : BusinessDocumentBase<T, C>
    where C : IEditableBusinessObject
  {
    #region Collection Storage

    [NotUndoable]
    [NonSerialized]
    private bool _completelyRemoveChild;

    [NotUndoable]
    private MobileList<C> _items = new();

    [NotUndoable]
    private MobileList<C>? _deletedList;

    /// <summary>
    /// A collection containing all child objects marked
    /// for deletion.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected MobileList<C> DeletedList
    {
      get
      {
        _deletedList ??= new MobileList<C>();
        return _deletedList;
      }
    }

    #endregion

    #region IContainsDeletedList

    IEnumerable<IEditableBusinessObject> IContainsDeletedList.DeletedList
      => (IEnumerable<IEditableBusinessObject>)DeletedList;

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

    #region Delete and Undelete child

    private void DeleteChild(C child)
    {
      // reset the child edit level
      UndoableBase.ResetChildEditLevel(child, EditLevel, false);
      // mark the object as deleted
      child.DeleteChild();
      // add to deleted collection for storage
      DeletedList.Add(child);
    }

    private void UnDeleteChild(C child)
    {
      // remove from deleted collection
      DeletedList.Remove(child);

      // preserve EditLevelAdded value
      int saveLevel = child.EditLevelAdded;

      // insert into active list
      _items.Add(child);
      OnAddEventHooks((IBusinessObject)child);

      // restore EditLevelAdded
      child.EditLevelAdded = saveLevel;
    }

    #endregion

    #region LoadListMode

    [NotUndoable]
    [NonSerialized]
    private Stack<bool>? _oldRLCE;

    [NotUndoable]
    [NonSerialized]
    private bool _raiseListChangedEvents = true;

    /// <summary>
    /// Gets or sets a value indicating whether list changed
    /// events should be raised.
    /// </summary>
    protected bool RaiseListChangedEvents
    {
      get => _raiseListChangedEvents;
      set => _raiseListChangedEvents = value;
    }

    /// <summary>
    /// Use this object to suppress list changed events
    /// during bulk operations.
    /// </summary>
    protected IDisposable LoadListMode => new LoadListModeObject(this);

    private sealed class LoadListModeObject : IDisposable
    {
      private readonly BusinessDocumentBase<T, C> _parent;

      internal LoadListModeObject(BusinessDocumentBase<T, C> parent)
      {
        _parent = parent;
        _parent._oldRLCE ??= new Stack<bool>();
        _parent._oldRLCE.Push(_parent._raiseListChangedEvents);
        _parent._raiseListChangedEvents = false;
      }

      public void Dispose()
      {
        if (_parent._oldRLCE?.Count > 0)
          _parent._raiseListChangedEvents = _parent._oldRLCE.Pop();
        GC.SuppressFinalize(this);
      }
    }

    #endregion

    #region IList<C> Implementation

    /// <summary>
    /// Gets the number of child items in the collection.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    bool ICollection<C>.IsReadOnly => false;

    /// <summary>
    /// Gets or sets the child item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    public C this[int index]
    {
      get => _items[index];
      set => SetItem(index, value);
    }

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="item">The child object to add.</param>
    public void Add(C item)
    {
      InsertItem(_items.Count, item);
    }

    /// <summary>
    /// Inserts an item at the specified index.
    /// </summary>
    /// <param name="index">Zero-based index.</param>
    /// <param name="item">The child object to insert.</param>
    public void Insert(int index, C item)
    {
      InsertItem(index, item);
    }

    /// <summary>
    /// Removes the first occurrence of a specific item from the collection.
    /// </summary>
    /// <param name="item">The child object to remove.</param>
    /// <returns>True if the item was found and removed.</returns>
    public bool Remove(C item)
    {
      int index = _items.IndexOf(item);
      if (index < 0)
        return false;
      RemoveItem(index);
      return true;
    }

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
      RemoveItem(index);
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear()
    {
      ClearItems();
    }

    /// <summary>
    /// Determines whether the collection contains a specific item.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    public bool Contains(C item) => _items.Contains(item);

    /// <summary>
    /// Determines the index of a specific item in the collection.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    public int IndexOf(C item) => _items.IndexOf(item);

    /// <summary>
    /// Copies the elements to an array starting at the specified index.
    /// </summary>
    /// <param name="array">Destination array.</param>
    /// <param name="arrayIndex">Start index in array.</param>
    public void CopyTo(C[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    public IEnumerator<C> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    #endregion

    #region Insert, Remove, Set, Clear

    /// <summary>
    /// Sets the edit level of the child object as it is added.
    /// </summary>
    /// <param name="index">Index of the item to insert.</param>
    /// <param name="item">Item to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    protected virtual void InsertItem(int index, C item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      if (item.IsChild)
      {
        IdentityManager.EnsureNextIdentityValueIsUnique(this, item);

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
        _items.Insert(index, item);
        OnAddEventHooks((IBusinessObject)item);
        if (RaiseListChangedEvents)
          OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
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
    protected virtual void RemoveItem(int index)
    {
      C child = _items[index];
      OnRemoveEventHooks((IBusinessObject)child);
      using (LoadListMode)
      {
        _items.RemoveAt(index);
      }
      if (!_completelyRemoveChild)
      {
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
    /// <param name="item">The new value for the item at the specified index.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    protected virtual void SetItem(int index, C item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      C? child = default;
      if (!ReferenceEquals(_items[index], item))
        child = _items[index];

      // delete old item
      using (LoadListMode)
      {
        if (child != null)
        {
          OnRemoveEventHooks((IBusinessObject)child);
          DeleteChild(child);
        }
      }

      // set parent reference
      item.SetParent(this);
      // ensure child uses same context as parent
      if (item is IUseApplicationContext iuac)
        iuac.ApplicationContext = ApplicationContext;
      // set child edit level
      UndoableBase.ResetChildEditLevel(item, EditLevel, false);
      // reset EditLevelAdded
      item.EditLevelAdded = EditLevel;
      _items[index] = item;
      OnAddEventHooks((IBusinessObject)item);
      if (RaiseListChangedEvents)
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, (object?)child, index));
    }

    /// <summary>
    /// Clears the collection, moving all active
    /// items to the deleted list.
    /// </summary>
    protected virtual void ClearItems()
    {
      while (_items.Count > 0)
        RemoveItem(0);
    }

    /// <summary>
    /// Override this method to create a new object that is added
    /// to the collection.
    /// </summary>
    protected virtual C AddNewCore()
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
    protected virtual async Task<C> AddNewCoreAsync()
    {
      var dp = ApplicationContext.CreateInstanceDI<DataPortal<C>>();
      var item = await dp.CreateChildAsync();
      Add(item);
      return item;
    }

    #endregion

    #region IObservableBindingList

    /// <summary>
    /// Creates and adds a new item to the collection.
    /// </summary>
    object IObservableBindingList.AddNew()
    {
      return AddNewCore();
    }

    /// <summary>
    /// Creates and adds a new item to the collection.
    /// </summary>
    async Task<object> IObservableBindingList.AddNewAsync()
    {
      return await AddNewCoreAsync();
    }

    [NonSerialized]
    [NotUndoable]
    private EventHandler<RemovingItemEventArgs>? _removingItemHandler;

    /// <summary>
    /// Event indicating that an item is being removed from the list.
    /// </summary>
    event EventHandler<RemovingItemEventArgs>? IObservableBindingList.RemovingItem
    {
      add => _removingItemHandler = (EventHandler<RemovingItemEventArgs>?)Delegate.Combine(_removingItemHandler, value);
      remove => _removingItemHandler = (EventHandler<RemovingItemEventArgs>?)Delegate.Remove(_removingItemHandler, value);
    }

    /// <summary>
    /// Raises the RemovingItem event.
    /// </summary>
    /// <param name="removingItem">The item being removed.</param>
    protected void OnRemovingItem(C removingItem)
    {
      _removingItemHandler?.Invoke(this, new RemovingItemEventArgs(removingItem));
    }

    #endregion

    #region INotifyCollectionChanged

    [NonSerialized]
    [NotUndoable]
    private NotifyCollectionChangedEventHandler? _collectionChanged;

    /// <summary>
    /// Event raised when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
      add => _collectionChanged = (NotifyCollectionChangedEventHandler?)Delegate.Combine(_collectionChanged, value);
      remove => _collectionChanged = (NotifyCollectionChangedEventHandler?)Delegate.Remove(_collectionChanged, value);
    }

    /// <summary>
    /// Raises the CollectionChanged event.
    /// </summary>
    /// <param name="e">Event args.</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (RaiseListChangedEvents)
        _collectionChanged?.Invoke(this, e);
    }

    #endregion

    #region IEditableCollection

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

    void IEditableCollection.SetParent(IParent? parent)
    {
      SetParent(parent!);
    }

    #endregion

    #region IParent override

    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    Task IParent.RemoveChild(IEditableBusinessObject child)
    {
      if (child is null)
        throw new ArgumentNullException(nameof(child));

      // Try to remove from the collection
      if (child is C typedChild)
      {
        int index = _items.IndexOf(typedChild);
        if (index >= 0)
        {
          Remove(typedChild);
          return Task.CompletedTask;
        }
      }

      // Fall back to FieldManager for property-based children
      var info = FieldManager.FindProperty(child);
      if (info is not null)
      {
        FieldManager.RemoveField(info);
      }

      return Task.CompletedTask;
    }

    #endregion

    #region Status Property Overrides

    /// <summary>
    /// Gets a value indicating whether this object's data has been changed.
    /// Aggregates the object's own dirty state with the dirty state
    /// of all collection children.
    /// </summary>
    public override bool IsDirty
    {
      get
      {
        // Check object's own dirty state (properties + FieldManager children)
        if (base.IsDirty)
          return true;

        // Any non-new deletions make us dirty
        foreach (C item in DeletedList)
          if (!item.IsNew)
            return true;

        // Check all collection children
        foreach (C child in _items)
          if (child.IsDirty)
            return true;

        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this object is currently in
    /// a valid state. Aggregates the object's own validity with the
    /// validity of all collection children.
    /// </summary>
    public override bool IsValid
    {
      get
      {
        // Check object's own validity (rules + FieldManager children)
        if (!base.IsValid)
          return false;

        // Check all collection children
        foreach (C child in _items)
          if (!child.IsValid)
            return false;

        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating if this object or its child objects
    /// are busy.
    /// </summary>
    public override bool IsBusy
    {
      get
      {
        // Check object's own busy state
        if (base.IsBusy)
          return true;

        // Check deleted children
        foreach (C item in DeletedList)
          if (item.IsBusy)
            return true;

        // Check active collection children
        foreach (C child in _items)
          if (child.IsBusy)
            return true;

        return false;
      }
    }

    #endregion

    #region N-Level Undo

    /// <summary>
    /// Cascades CopyState to collection children after the
    /// object's own state has been copied.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void CopyStateComplete()
    {
      base.CopyStateComplete();

      // Cascade to all active children
      for (int x = 0; x < _items.Count; x++)
      {
        C child = _items[x];
        child.CopyState(EditLevel, false);
      }

      // Cascade to all deleted children
      foreach (C child in DeletedList)
        child.CopyState(EditLevel, false);
    }

    /// <summary>
    /// Cascades UndoChanges to collection children after the
    /// object's own state has been restored.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void UndoChangesComplete()
    {
      base.UndoChangesComplete();

      C child;

      using (LoadListMode)
      {
        try
        {
          // Cancel edit on all current items (reverse order)
          for (int index = _items.Count - 1; index >= 0; index--)
          {
            child = _items[index];
            child.UndoChanges(EditLevel, false);

            // If item was added after this edit level, remove it completely
            if (child.EditLevelAdded > EditLevel)
            {
              try
              {
                _completelyRemoveChild = true;
                OnRemoveEventHooks((IBusinessObject)child);
                _items.RemoveAt(index);
              }
              finally
              {
                _completelyRemoveChild = false;
              }
            }
          }

          // Cancel edit on all deleted items (reverse order)
          for (int index = DeletedList.Count - 1; index >= 0; index--)
          {
            child = DeletedList[index];
            child.UndoChanges(EditLevel, false);
            if (child.EditLevelAdded > EditLevel)
            {
              // If item is below its point of addition, remove
              DeletedList.RemoveAt(index);
            }
            else
            {
              // If item is no longer deleted, move back to main list
              if (!child.IsDeleted)
                UnDeleteChild(child);
            }
          }
        }
        finally
        {
          if (RaiseListChangedEvents)
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
      }
    }

    /// <summary>
    /// Cascades AcceptChanges to collection children after the
    /// object's own changes have been accepted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void AcceptChangesComplete()
    {
      base.AcceptChangesComplete();

      // Cascade to all active children
      foreach (C child in _items)
      {
        child.AcceptChanges(EditLevel, false);
        // If item is below its point of addition, lower point of addition
        if (child.EditLevelAdded > EditLevel)
          child.EditLevelAdded = EditLevel;
      }

      // Cascade to deleted children (reverse order)
      for (int index = DeletedList.Count - 1; index >= 0; index--)
      {
        C child = DeletedList[index];
        child.AcceptChanges(EditLevel, false);
        // If item is below its point of addition, remove
        if (child.EditLevelAdded > EditLevel)
          DeletedList.RemoveAt(index);
      }
    }

    #endregion

    #region Mobile Object Overrides

    /// <summary>
    /// Override to serialize collection children.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="formatter">Formatter reference.</param>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> or <paramref name="formatter"/> is <see langword="null"/>.</exception>
    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));
      if (formatter is null)
        throw new ArgumentNullException(nameof(formatter));

      base.OnGetChildren(info, formatter);

      var itemsInfo = formatter.SerializeObject(_items);
      info.AddChild("_bdb_items", itemsInfo.ReferenceId);

      if (_deletedList != null)
      {
        var deletedInfo = formatter.SerializeObject(_deletedList);
        info.AddChild("_bdb_deletedList", deletedInfo.ReferenceId);
      }
    }

    /// <summary>
    /// Override to deserialize collection children.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="formatter">Formatter reference.</param>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> or <paramref name="formatter"/> is <see langword="null"/>.</exception>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));
      if (formatter is null)
        throw new ArgumentNullException(nameof(formatter));

      if (info.Children.TryGetValue("_bdb_items", out var itemsChild))
      {
        _items = (MobileList<C>)formatter.GetObject(itemsChild.ReferenceId)!;
      }

      if (info.Children.TryGetValue("_bdb_deletedList", out var deletedChild))
      {
        _deletedList = (MobileList<C>?)formatter.GetObject(deletedChild.ReferenceId);
      }

      base.OnSetChildren(info, formatter);
    }

    /// <summary>
    /// Override to re-establish parent references and event hooks
    /// for collection items after deserialization.
    /// </summary>
    protected override void Deserialized()
    {
      base.Deserialized();

      // Re-establish parent references and event hooks for active items
      foreach (var item in _items)
      {
        item.SetParent(this);
        OnAddEventHooks((IBusinessObject)item);
      }

      // Re-establish parent references for deleted items
      foreach (var item in DeletedList)
      {
        item.SetParent(this);
      }
    }

    #endregion

    #region Child Data Access

    /// <summary>
    /// Saves all items in the list, automatically
    /// performing insert, update or delete operations
    /// as necessary.
    /// </summary>
    /// <param name="parameters">
    /// Optional parameters passed to child update methods.
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

        foreach (var child in _items)
          if (child.IsDirty)
            dp.UpdateChild(child, parameters);
      }
    }

    /// <summary>
    /// Asynchronously saves all items in the list, automatically
    /// performing insert, update or delete operations as necessary.
    /// </summary>
    /// <param name="parameters">
    /// Optional parameters passed to child update methods.
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

        foreach (var child in _items)
          if (child.IsDirty)
            await dp.UpdateChildAsync(child, parameters).ConfigureAwait(false);
      }
    }

    #endregion

    #region Register Properties

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property.</typeparam>
    /// <param name="info">PropertyInfo object for the property.</param>
    /// <returns>The provided IPropertyInfo object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
    protected static new PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> info)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      return Core.FieldManager.PropertyInfoManager.RegisterProperty<P>(typeof(T), info);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property.</typeparam>
    /// <param name="propertyName">Property name from nameof().</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static new PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property.</typeparam>
    /// <param name="propertyLambdaExpression">Property expression.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected new static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

      System.Reflection.PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property.</typeparam>
    /// <param name="propertyName">Property name from nameof().</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static new PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, RelationshipTypes relationship)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, string.Empty, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property.</typeparam>
    /// <param name="propertyName">Property name from nameof().</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static new PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property.</typeparam>
    /// <param name="propertyName">Property name from nameof().</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding.</param>
    /// <param name="defaultValue">Default value for the property.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static new PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName, P? defaultValue)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property.</typeparam>
    /// <param name="propertyName">Property name from nameof().</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding.</param>
    /// <param name="defaultValue">Default value for the property.</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static new PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName, P? defaultValue, RelationshipTypes relationship)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue, relationship));
    }

    /// <summary>
    /// Registers a method for use in Authorization.
    /// </summary>
    /// <param name="methodName">Method name from nameof().</param>
    /// <exception cref="ArgumentNullException"><paramref name="methodName"/> is <see langword="null"/>.</exception>
    protected static new MethodInfo RegisterMethod(string methodName)
    {
      if (methodName is null)
        throw new ArgumentNullException(nameof(methodName));

      return RegisterMethod(typeof(T), methodName);
    }

    /// <summary>
    /// Registers a method for use in Authorization.
    /// </summary>
    /// <param name="methodLambdaExpression">The method lambda expression.</param>
    /// <exception cref="ArgumentNullException"><paramref name="methodLambdaExpression"/> is <see langword="null"/>.</exception>
    protected new static MethodInfo RegisterMethod(Expression<Action<T>> methodLambdaExpression)
    {
      if (methodLambdaExpression is null)
        throw new ArgumentNullException(nameof(methodLambdaExpression));

      System.Reflection.MethodInfo reflectedMethodInfo = Reflect<T>.GetMethod(methodLambdaExpression);
      return RegisterMethod(reflectedMethodInfo.Name);
    }

    #endregion
  }
}
