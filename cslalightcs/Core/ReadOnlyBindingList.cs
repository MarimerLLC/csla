using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Csla.Serialization;

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
  [Serializable]
  public abstract class ReadOnlyBindingList<C> :
    Core.ExtendedBindingList<C>, Core.IBusinessObject
  {
    #region Serialization

    protected override object GetValue(System.Reflection.FieldInfo field)
    {
      if (field.DeclaringType == typeof(ReadOnlyBindingList<C>))
        return field.GetValue(this);
      else
        return base.GetValue(field);
    }

    protected override void SetValue(System.Reflection.FieldInfo field, object value)
    {
      if (field.DeclaringType == typeof(ReadOnlyBindingList<C>))
        field.SetValue(this, value);
      else
        base.SetValue(field, value);
    }

    #endregion

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
  }
}