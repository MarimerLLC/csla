using System;
using Csla.Properties;

namespace Csla.Core
{

  /// <summary>
  /// A readonly version of BindingList(Of T)
  /// </summary>
  /// <typeparam name="T">Type of item contained in the list.</typeparam>
  /// <remarks>
  /// This is a subclass of BindingList(Of T) that implements
  /// a readonly list, preventing adding and removing of items
  /// from the list. Use the Protected IsReadOnly property
  /// to unlock the list for loading/unloading data.
  /// </remarks>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable()]
  public abstract class ReadOnlyBindingList<T> : System.ComponentModel.BindingList<T>, Core.IBusinessObject
  {
    private bool _isReadOnly = true;

    /// <summary>
    /// Gets or sets whether the list is readonly.
    /// </summary>
    /// <value>True indicates that the list is readonly.</value>
    protected bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    protected ReadOnlyBindingList()
    {
      AllowEdit = false;
      AllowRemove = false;
      AllowNew = false;
    }

    /// <summary>
    /// Prevents clearing the collection.
    /// </summary>
    protected override void ClearItems()
    {
      if (!IsReadOnly)
      {
        AllowRemove = true;
        base.ClearItems();
        AllowRemove = false;
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
    protected override void InsertItem(int index, T item)
    {
      if (!IsReadOnly)
        base.InsertItem(index, item);
      else
        throw new NotSupportedException(Resources.InsertInvalidException);
    }

    /// <summary>
    /// Removes the item at the specified index if the collection is
    /// not in readonly mode.
    /// </summary>
    protected override void RemoveItem(int index)
    {
      if (!IsReadOnly)
      {
        AllowRemove = true;
        base.RemoveItem(index);
        AllowRemove = false;
      }
      else
        throw new NotSupportedException(Resources.RemoveInvalidException);
    }

    /// <summary>
    /// Replaces the item at the specified index with the 
    /// specified item if the collection is not in
    /// readonly mode.
    /// </summary>
    protected override void SetItem(int index, T item)
    {
      if (!IsReadOnly)
        base.SetItem(index, item);
      else
        throw new NotSupportedException(Resources.ChangeInvalidException);
    }
  }
}