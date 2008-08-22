using System;
using System.ComponentModel;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Serialization.Mobile;

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
    INotifyBusy,
    ISerializationNotification
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
      OnRemoveEventHooksInternal(this[index]);
      base.RemoveItem(index);
    }

    #endregion

    #region AddRange

    /// <summary>
    /// Add a range of items to the list.
    /// </summary>
    /// <param name="range">List of items to add.</param>
    public void AddRange(IEnumerable<T> range)
    {
      foreach (var element in range)
        this.Add(element);
    }

    #endregion

    #region Bubbling event Hooks

    protected override void InsertItem(int index, T item)
    {
      base.InsertItem(index, item);
      OnAddEventHooksInternal(item);
    }

    protected internal virtual void OnAddEventHooksInternal(T item)
    {
      INotifyBusy busy = item as INotifyBusy;
      if(busy!=null)
        busy.BusyChanged += new BusyChangedEventHandler(busy_BusyChanged);

      INotifyUnhandledAsyncException unhandled = item as INotifyUnhandledAsyncException;
      if(unhandled!=null)
        unhandled.UnhandledAsyncException += new EventHandler<ErrorEventArgs>(unhandled_UnhandledAsyncException);

      OnAddEventHooks(item);
    }

    protected virtual void OnAddEventHooks(T item)
    {
    }

    protected internal virtual void OnRemoveEventHooksInternal(T item)
    {
      INotifyBusy busy = item as INotifyBusy;
      if (busy != null)
        busy.BusyChanged -= new BusyChangedEventHandler(busy_BusyChanged);

      INotifyUnhandledAsyncException unhandled = item as INotifyUnhandledAsyncException;
      if (unhandled != null)
        unhandled.UnhandledAsyncException -= new EventHandler<ErrorEventArgs>(unhandled_UnhandledAsyncException);

      OnRemoveEventHooks(item);
    }

    protected virtual void OnRemoveEventHooks(T item)
    {
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete, it is only implemented
    /// by internal classes to guarantee that they are executed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void OnDeserializedInternal()
    {
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized()
    {
      // do nothing - this is here so a subclass
      // could override if needed
    }

    #endregion

    #region INotifyPropertyBusy Members

    [NotUndoable]
    [NonSerialized]
    private BusyChangedEventHandler _busyChanged = null;

    public event BusyChangedEventHandler BusyChanged
    {
      add { _busyChanged = (BusyChangedEventHandler)Delegate.Combine(_busyChanged, value); }
      remove { _busyChanged = (BusyChangedEventHandler)Delegate.Remove(_busyChanged, value); }
    }

    protected void OnBusyChanged(string propertyName, bool busy)
    {
      OnBusyChanged(new BusyChangedEventArgs(propertyName, busy));
    }

    protected virtual void OnBusyChanged(BusyChangedEventArgs args)
    {
      if (_busyChanged != null)
        _busyChanged(this, args);
    }

    public virtual bool IsBusy
    {
      get { throw new NotImplementedException(); }
    }

    public virtual bool IsSelfBusy
    {
      get { return IsBusy; }
    }

    void busy_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

    #endregion

    #region INotifyUnhandledAsyncException Members

    [NotUndoable]
    [NonSerialized]
    private EventHandler<ErrorEventArgs> _unhandledAsyncException;

    public event EventHandler<ErrorEventArgs> UnhandledAsyncException
    {
      add { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
      remove { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
    }

    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
      if (_unhandledAsyncException != null)
        _unhandledAsyncException(this, error);
    }

    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncException(new ErrorEventArgs(originalSender, error));
    }

    void unhandled_UnhandledAsyncException(object sender, ErrorEventArgs e)
    {
      OnUnhandledAsyncException(e);
    }

    #endregion

    #region ISerializationNotification Members

    void ISerializationNotification.Deserialized()
    {
      OnDeserializedInternal();
      OnDeserialized();
    }

    #endregion
  }
}
