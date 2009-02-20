using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Csla.Properties;

namespace Csla.Core
{
  /// <summary>
  /// A readonly version of BindingList(Of T)
  /// </summary>
  /// <typeparam name="C">Type of item contained in the list.</typeparam>
  /// <remarks>
  /// This is a subclass of BindingList(Of T) that implements
  /// a readonly list, preventing adding and removing of items
  /// from the list. Use the IsReadOnly property
  /// to unlock the list for loading/unloading data.
  /// </remarks>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", 
    "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable()]
  public abstract class ReadOnlyBindingList<C> :
    Core.ExtendedBindingList<C>, Core.IBusinessObject, Core.IReadOnlyBindingList
  {
    private bool _isReadOnly = true;

    /// <summary>
    /// Gets or sets a value indicating whether the list is readonly.
    /// </summary>
    /// <remarks>
    /// Subclasses can set this value to unlock the collection
    /// in order to alter the collection's data.
    /// </remarks>
    /// <value>True indicates that the list is readonly.</value>
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      protected set { _isReadOnly = value; }
    }

    bool Core.IReadOnlyBindingList.IsReadOnly
    {
      get { return IsReadOnly; }
      set { IsReadOnly = value; }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected ReadOnlyBindingList()
    {
      this.RaiseListChangedEvents = false;
      AllowEdit = false;
      AllowRemove = false;
      AllowNew = false;
      this.RaiseListChangedEvents = true;
    }

    /// <summary>
    /// Prevents clearing the collection.
    /// </summary>
    protected override void ClearItems()
    {
      if (!IsReadOnly)
      {
        bool oldValue = AllowRemove;
        AllowRemove = true;
        base.ClearItems();
        DeferredLoadIndexIfNotLoaded();
        _indexSet.ClearIndexes();
        AllowRemove = oldValue;
      }
      else
        throw new NotSupportedException(Resources.ClearInvalidException);
    }

    /// <summary>
    /// Prevents insertion of items into the collection.
    /// </summary>
    protected override object AddNewCore()
    {
      if (!IsReadOnly)
        return base.AddNewCore();
      else
        throw new NotSupportedException(Resources.InsertInvalidException);
    }

    /// <summary>
    /// Prevents insertion of items into the collection.
    /// </summary>
    /// <param name="index">Index at which to insert the item.</param>
    /// <param name="item">Item to insert.</param>
    protected override void InsertItem(int index, C item)
    {
      if (!IsReadOnly)
      {
        InsertIndexItem(item);
        base.InsertItem(index, item);
      }
      else
        throw new NotSupportedException(Resources.InsertInvalidException);
    }

    /// <summary>
    /// Removes the item at the specified index if the collection is
    /// not in readonly mode.
    /// </summary>
    /// <param name="index">Index of the item to remove.</param>
    protected override void RemoveItem(int index)
    {
      if (!IsReadOnly)
      {
        bool oldValue = AllowRemove;
        AllowRemove = true;
        RemoveIndexItem(this[index]);
        base.RemoveItem(index);
        AllowRemove = oldValue;
      }
      else
        throw new NotSupportedException(Resources.RemoveInvalidException);
    }

    /// <summary>
    /// Replaces the item at the specified index with the 
    /// specified item if the collection is not in
    /// readonly mode.
    /// </summary>
    /// <param name="index">Index of the item to replace.</param>
    /// <param name="item">New item for the list.</param>
    protected override void SetItem(int index, C item)
    {
      if (!IsReadOnly)
      {
        RemoveIndexItem(this[index]);
        base.SetItem(index, item);
        InsertIndexItem(item);
      }
      else
        throw new NotSupportedException(Resources.ChangeInvalidException);
    }

    #region ITrackStatus

    /// <summary>
    /// Gets a value indicating whether this object or its
    /// child objects are busy.
    /// </summary>
    public override bool IsBusy
    {
      get
      {
        // run through all the child objects
        // and if any are dirty then then
        // collection is dirty
        foreach (C child in this)
        {
          INotifyBusy busy = child as INotifyBusy;
          if (busy != null && busy.IsBusy)
            return true;
        }

        return false;
      }
    }

    #endregion

    #region Indexing
    [NonSerialized]
    private Linq.IIndexSet<C> _indexSet;

    private void DeferredLoadIndexIfNotLoaded()
    {
      if (_indexSet == null) _indexSet = new Csla.Linq.IndexSet<C>();
    }

    /// <summary>
    /// Allows users of CSLA to override the indexing behavior of BLB
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Type IndexingProvider
    {
      get
      {
        DeferredLoadIndexIfNotLoaded();
        return _indexSet.GetType();
      }
      set
      {
        if (value.IsClass && !value.IsAbstract && value.IsAssignableFrom(typeof(Linq.IIndexSet<C>)))
        {
          _indexSet = Activator.CreateInstance(value) as Linq.IIndexSet<C>;
          ReIndexAll();
        }
      }
    }


    private IndexModeEnum IndexModeFor(string property)
    {
      DeferredLoadIndexIfNotLoaded();
      if (_indexSet.HasIndexFor(property))
        return _indexSet[property].IndexMode;
      else
        return IndexModeEnum.IndexModeNever;
    }

    private bool IndexLoadedFor(string property)
    {
      DeferredLoadIndexIfNotLoaded();
      if (_indexSet.HasIndexFor(property))
        return _indexSet[property].Loaded;
      else
        return false;
    }

    private void LoadIndexIfNotLoaded(string property)
    {
      if (IndexModeFor(property) != IndexModeEnum.IndexModeNever)
        if (!IndexLoadedFor(property))
        {
          _indexSet.LoadIndex(property);
          ReIndex(property);
        }
    }

    private void InsertIndexItem(C item)
    {
      DeferredLoadIndexIfNotLoaded();
      _indexSet.InsertItem(item);
    }

    private void InsertIndexItem(C item, string property)
    {
      DeferredLoadIndexIfNotLoaded();
      _indexSet.InsertItem(item, property);
    }

    private void RemoveIndexItem(C item)
    {
      DeferredLoadIndexIfNotLoaded();
      _indexSet.RemoveItem(item);
    }

    private void RemoveIndexItem(C item, string property)
    {
      DeferredLoadIndexIfNotLoaded();
      _indexSet.RemoveItem(item, property);
    }

    private void ReIndexItem(C item, string property)
    {
      DeferredLoadIndexIfNotLoaded();
      _indexSet.ReIndexItem(item, property);
    }

    private void ReIndexItem(C item)
    {
      DeferredLoadIndexIfNotLoaded();
      _indexSet.ReIndexItem(item);
    }

    private void ReIndexAll()
    {
      DeferredLoadIndexIfNotLoaded();
      _indexSet.ClearIndexes();
      foreach (C item in this)
        InsertIndexItem(item);
    }

    private void ReIndex(string property)
    {
      DeferredLoadIndexIfNotLoaded();
      _indexSet.ClearIndex(property);
      foreach (C item in this)
        InsertIndexItem(item, property);
      _indexSet[property].LoadComplete();
    }

    #endregion

    #region Where Implementation
    /// <summary>
    /// Iterates through a set of items according to the expression passed to it.
    /// </summary>
    public IEnumerable<C> SearchByExpression(Expression<Func<C, bool>> expr)
    {
      DeferredLoadIndexIfNotLoaded();
      string property = _indexSet.HasIndexFor(expr);
      if (
        property != null &&
        IndexModeFor(property) != IndexModeEnum.IndexModeNever
          )
      {
        LoadIndexIfNotLoaded(property);
        foreach (C item in _indexSet.Search(expr, property))
          yield return item;
      }
      else
      {
        IEnumerable<C> sourceEnum = this.AsEnumerable<C>();
        var result = sourceEnum.Where<C>(expr.Compile());
        foreach (C item in result)
          yield return item;
      }
    }

    #endregion

    #region MobileFormatter

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("Csla.Core.ReadOnlyBindingList._isReadOnly", _isReadOnly);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnSetState(info);
      _isReadOnly = info.GetValue<bool>("Csla.Core.ReadOnlyBindingList._isReadOnly");
    }

    /// <summary>
    /// Override this method to retrieve your child object
    /// references from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnSetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      var old = IsReadOnly;
      IsReadOnly = false;
      base.OnSetChildren(info, formatter);
      IsReadOnly = old;
    }

    #endregion
  }

  /// <summary>
  /// Extension method for implementation of LINQ methods on BusinessListBase
  /// </summary>
  public static class ReadOnlyBindingListExtension
  {
    /// <summary>
    /// Custom implementation of Where for BusinessListBase - used in LINQ
    /// </summary>
    public static IEnumerable<C> Where<C>(this ReadOnlyBindingList<C> source, Expression<Func<C, bool>> expr)
      where C : Core.IEditableBusinessObject
    {
      foreach (C item in source.SearchByExpression(expr))
        yield return item;
    }
  }
}