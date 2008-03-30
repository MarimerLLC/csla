using System;
using System.ComponentModel;

namespace Csla.Core
{
  /// <summary>
  /// Extends BindingList of T by adding extra
  /// behaviors.
  /// </summary>
  /// <typeparam name="T">Type of item contained in list.</typeparam>
  [Serializable]
  public class ExtendedBindingList<T> : BindingList<T>, IExtendedBindingList
  {
    #region RemovingItem event

    [NonSerialized()]
    private EventHandler<RemovingItemEventArgs> _nonSerializableHandlers;
    private EventHandler<RemovingItemEventArgs> _serializableHandlers;

    /// <summary>
    /// Implements a serialization-safe RemovingItem event.
    /// </summary>
    public event EventHandler<RemovingItemEventArgs> RemovingItem
    {
      add
      {
        if (value.Method.IsPublic &&
           (value.Method.DeclaringType.IsSerializable ||
            value.Method.IsStatic))
          _serializableHandlers = (EventHandler<RemovingItemEventArgs>)
            System.Delegate.Combine(_serializableHandlers, value);
        else
          _nonSerializableHandlers = (EventHandler<RemovingItemEventArgs>)
            System.Delegate.Combine(_nonSerializableHandlers, value);
      }
      remove
      {
        if (value.Method.IsPublic &&
           (value.Method.DeclaringType.IsSerializable ||
            value.Method.IsStatic))
          _serializableHandlers = (EventHandler<RemovingItemEventArgs>)
            System.Delegate.Remove(_serializableHandlers, value);
        else
          _nonSerializableHandlers = (EventHandler<RemovingItemEventArgs>)
            System.Delegate.Remove(_nonSerializableHandlers, value);
      }
    }

    /// <summary>
    /// Raise the RemovingItem event.
    /// </summary>
    /// <param name="removedItem">
    /// A reference to the item that 
    /// is being removed.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnRemovingItem(T removedItem)
    {
      if (_nonSerializableHandlers != null)
        _nonSerializableHandlers.Invoke(this,
          new RemovingItemEventArgs(removedItem));
      if (_serializableHandlers != null)
        _serializableHandlers.Invoke(this,
          new RemovingItemEventArgs(removedItem));
    }

    #endregion

    #region RemoveItem

    /// <summary>
    /// Remove the item at the
    /// specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the item
    /// to remove.
    /// </param>
    protected override void RemoveItem(int index)
    {
      OnRemovingItem(this[index]);
      base.RemoveItem(index);
    }

    #endregion

    #region AddRange

    /// <summary>
    /// Add a range of items to the list.
    /// </summary>
    /// <param name="range">List of items to add.</param>
    public void AddRange(System.Collections.Generic.IEnumerable<T> range)
    {
      foreach (var element in range)
        this.Add(element);
    }

    #endregion
  }
}
