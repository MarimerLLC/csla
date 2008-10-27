using System;
using System.ComponentModel;
using Csla.Serialization.Mobile;
using System.Runtime.Serialization;

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
    IMobileObject,
    INotifyBusy,
    INotifyChildChanged,
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
      OnRemoveEventHooksInternal(this[index]);
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
    private BusyChangedEventHandler _busyChanged = null;

    /// <summary>
    /// Event indicating that the busy status of the
    /// object has changed.
    /// </summary>
    public event BusyChangedEventHandler BusyChanged
    {
      add { _busyChanged = (BusyChangedEventHandler)Delegate.Combine(_busyChanged, value); }
      remove { _busyChanged = (BusyChangedEventHandler)Delegate.Remove(_busyChanged, value); }
    }

    /// <summary>
    /// Raises the BusyChanged event.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    protected internal virtual void OnBusyChangedInternal(BusyChangedEventArgs args)
    {
      OnBusyChanged(args);

      if (_busyChanged != null)
        _busyChanged(this, args);
    }

    /// <summary>
    /// Override this method to be notified when the
    /// IsBusy property has changed.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    protected virtual void OnBusyChanged(BusyChangedEventArgs args)
    {
    }

    /// <summary>
    /// Raises the BusyChanged event for a specific property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="busy">New busy value.</param>
    protected void OnBusyChanged(string propertyName, bool busy)
    {
      OnBusyChangedInternal(new BusyChangedEventArgs(propertyName, busy));
    }

    /// <summary>
    /// Gets the busy status for this object and its child objects.
    /// </summary>
    [Browsable(false)]
    public virtual bool IsBusy
    {
      get { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Gets the busy status for this object.
    /// </summary>
    [Browsable(false)]
    public virtual bool IsSelfBusy
    {
      get { return IsBusy; }
    }

    void busy_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChangedInternal(e);
    }

    #endregion

    #region INotifyUnhandledAsyncException Members

    [NotUndoable]
    [NonSerialized]
    private EventHandler<ErrorEventArgs> _unhandledAsyncException;

    /// <summary>
    /// Event indicating that an exception occurred during
    /// an async operation.
    /// </summary>
    public event EventHandler<ErrorEventArgs> UnhandledAsyncException
    {
      add { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
      remove { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="error">Event arguments.</param>
    protected internal virtual void OnUnhandledAsyncExceptionInternal(ErrorEventArgs error)
    {
      OnUnhandledAsyncException(error);

      if (_unhandledAsyncException != null)
        _unhandledAsyncException(this, error);
    }

    /// <summary>
    /// Method invoked when an unhandled async exception has
    /// occurred.
    /// </summary>
    /// <param name="error">Event arguments.</param>
    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="originalSender">Original sender of event.</param>
    /// <param name="error">Exception that occurred.</param>
    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncExceptionInternal(new ErrorEventArgs(originalSender, error));
    }

    void unhandled_UnhandledAsyncException(object sender, ErrorEventArgs e)
    {
      OnUnhandledAsyncExceptionInternal(e);
    }

    #endregion

    #region AddChildHooks

    /// <summary>
    /// Invoked when an item is inserted into the list.
    /// </summary>
    /// <param name="index">Index of new item.</param>
    /// <param name="item">Reference to new item.</param>
    protected override void InsertItem(int index, T item)
    {
      base.InsertItem(index, item);
      OnAddEventHooksInternal(item);
    }

    /// <summary>
    /// Adds event hooks to child object in list.
    /// </summary>
    /// <param name="item">Reference to child object.</param>
    protected virtual void OnAddEventHooksInternal(T item)
    {
      INotifyBusy busy = item as INotifyBusy;
      if (busy != null)
        busy.BusyChanged += new BusyChangedEventHandler(busy_BusyChanged);

      INotifyUnhandledAsyncException unhandled = item as INotifyUnhandledAsyncException;
      if (unhandled != null)
        unhandled.UnhandledAsyncException += new EventHandler<ErrorEventArgs>(unhandled_UnhandledAsyncException);

      INotifyPropertyChanged c = item as INotifyPropertyChanged;
      if (c != null)
        c.PropertyChanged += Child_PropertyChanged;

      IBindingList list = item as IBindingList;
      if (list != null)
        list.ListChanged += new ListChangedEventHandler(Child_ListChanged);

      INotifyChildChanged child = item as INotifyChildChanged;
      if(child!=null)
        child.ChildChanged += new EventHandler<ChildChangedEventArgs>(Child_Changed);

      OnAddEventHooks(item);
    }

    /// <summary>
    /// Method invoked when events are hooked for a child
    /// object.
    /// </summary>
    /// <param name="item">Reference to child object.</param>
    protected virtual void OnAddEventHooks(T item)
    {
    }

    /// <summary>
    /// Removes event hooks to child object in list.
    /// </summary>
    /// <param name="item">Reference to child object.</param>
    protected internal virtual void OnRemoveEventHooksInternal(T item)
    {
      INotifyBusy busy = item as INotifyBusy;
      if (busy != null)
        busy.BusyChanged -= new BusyChangedEventHandler(busy_BusyChanged);

      INotifyUnhandledAsyncException unhandled = item as INotifyUnhandledAsyncException;
      if (unhandled != null)
        unhandled.UnhandledAsyncException -= new EventHandler<ErrorEventArgs>(unhandled_UnhandledAsyncException);

      INotifyPropertyChanged c = item as INotifyPropertyChanged;
      if (c != null)
        c.PropertyChanged -= new PropertyChangedEventHandler(Child_PropertyChanged);

      IBindingList list = item as IBindingList;
      if(list!=null)
        list.ListChanged -= new ListChangedEventHandler(Child_ListChanged);

      INotifyChildChanged child = item as INotifyChildChanged;
      if (child != null)
        child.ChildChanged -= new EventHandler<ChildChangedEventArgs>(Child_Changed);

      OnRemoveEventHooks(item);
    }

    /// <summary>
    /// Method invoked when events are unhooked for a child
    /// object.
    /// </summary>
    /// <param name="item">Reference to child object.</param>
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

    #region ISerializationNotification Members

    [OnDeserialized]
    private void OnDeserializedHandler(StreamingContext context)
    {
      foreach (T item in this)
        OnAddEventHooksInternal(item);

      ((ISerializationNotification)this).Deserialized();
    }

    #region ISerializationNotification Members

    void ISerializationNotification.Deserialized()
    {
      OnDeserializedInternal();
      OnDeserialized();
    }

    #endregion

    #endregion

    #region Child Change Notification

    [NonSerialized]
    [NotUndoable]
    private EventHandler<Csla.Core.ChildChangedEventArgs> _childChangedHandlers;

    /// <summary>
    /// Event raised when a child object has been changed.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event EventHandler<Csla.Core.ChildChangedEventArgs> ChildChanged
    {
      add
      {
        _childChangedHandlers = (EventHandler<Csla.Core.ChildChangedEventArgs>)
          System.Delegate.Combine(_childChangedHandlers, value);
      }
      remove
      {
        _childChangedHandlers = (EventHandler<Csla.Core.ChildChangedEventArgs>)
          System.Delegate.Remove(_childChangedHandlers, value);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void OnChildChangedInternal(object source, ChildChangedEventArgs e)
    {
      OnChildChanged(source, e);

      if (_childChangedHandlers != null)
        _childChangedHandlers.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ChildChanged event, indicating that a child
    /// object has been changed.
    /// </summary>
    /// <param name="source">
    /// Reference to the object that was changed.
    /// </param>
    /// <param name="listArgs">
    /// ListChangedEventArgs object or null.
    /// </param>
    /// <param name="propertyArgs">
    /// PropertyChangedEventArgs object or null.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnChildChanged(object source, PropertyChangedEventArgs propertyArgs, ListChangedEventArgs listArgs)
    {
      OnChildChangedInternal(this, new ChildChangedEventArgs(source, propertyArgs, listArgs));
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnChildChanged(object sender, ChildChangedEventArgs e)
    {
    }

    private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnChildChangedInternal(this, new ChildChangedEventArgs(sender, e, null));
    }

    private void Child_ListChanged(object sender, ListChangedEventArgs e)
    {
      if (e.ListChangedType != ListChangedType.ItemChanged)
        OnChildChangedInternal(this, new ChildChangedEventArgs(sender, null, e));
    }

    private void Child_Changed(object sender, ChildChangedEventArgs e)
    {
      OnChildChangedInternal(this, e);
    }

    #endregion
  }
}
