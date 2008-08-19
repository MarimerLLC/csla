using System;
using System.ComponentModel;
using Csla.Serialization;

namespace Csla.Core
{
  /// <summary>
  /// Extends BindingList of T by adding extra
  /// behaviors.
  /// </summary>
  /// <typeparam name="T">Type of item contained in list.</typeparam>
  [Serializable]
  public class ExtendedBindingList<T> : MobileBindingList<T>,
    IExtendedBindingList,
    INotifyBusy
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
           (value.Method.DeclaringType.IsSerializable() ||
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
           (value.Method.DeclaringType.IsSerializable() ||
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

    #region INotifyPropertyBusy Members

    [NotUndoable]
    [NonSerialized]
    private PropertyChangedEventHandler _propertyBusy = null;
    [NotUndoable]
    [NonSerialized]
    private PropertyChangedEventHandler _propertyIdle = null;

    public event PropertyChangedEventHandler PropertyBusy
    {
      add { _propertyBusy = (PropertyChangedEventHandler)Delegate.Combine(_propertyBusy, value); }
      remove { _propertyBusy = (PropertyChangedEventHandler)Delegate.Remove(_propertyBusy, value); }
    }

    public event PropertyChangedEventHandler PropertyIdle
    {
      add { _propertyIdle = (PropertyChangedEventHandler)Delegate.Combine(_propertyIdle, value); }
      remove { _propertyIdle = (PropertyChangedEventHandler)Delegate.Remove(_propertyIdle, value); }
    }

    protected void OnPropertyBusy(PropertyChangedEventArgs args)
    {
      if (_propertyBusy != null)
        _propertyBusy(this, args);
    }

    protected void OnPropertyIdle(PropertyChangedEventArgs args)
    {
      if (_propertyIdle != null)
        _propertyIdle(this, args);
    }

    public virtual bool IsBusy
    {
      get { throw new NotImplementedException(); }
    }

    public virtual bool IsSelfBusy
    {
      get { return IsBusy; }
    }

    #endregion
  }
}
