//-----------------------------------------------------------------------
// <copyright file="ExtendedBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extends BindingList of T by adding extra</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Csla.Serialization.Mobile;
#if ANDROID || IOS
using System.Collections.Specialized;
#endif


namespace Csla.Core
{
  /// <summary>
  /// Extends BindingList of T by adding extra
  /// behaviors.
  /// </summary>
  /// <typeparam name="T">Type of item contained in list.</typeparam>
  [Serializable]
  public class ExtendedBindingList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T> : MobileBindingList<T>,
    IExtendedBindingList,
    INotifyBusy,
    INotifyChildChanged,
    ISerializationNotification
  {
    [NonSerialized]
    private EventHandler<RemovingItemEventArgs>? _nonSerializableHandlers;
    private EventHandler<RemovingItemEventArgs>? _serializableHandlers;

    /// <summary>
    /// Implements a serialization-safe RemovingItem event.
    /// </summary>
    public event EventHandler<RemovingItemEventArgs>? RemovingItem
    {
      add
      {
        if (value is null)
          return;

        if (value.Method.IsPublic)
          _serializableHandlers = (EventHandler<RemovingItemEventArgs>?)Delegate.Combine(_serializableHandlers, value);
        else
          _nonSerializableHandlers = (EventHandler<RemovingItemEventArgs>?)Delegate.Combine(_nonSerializableHandlers, value);
      }
      remove
      {
        if (value is null)
          return;

        if (value.Method.IsPublic)
          _serializableHandlers = (EventHandler<RemovingItemEventArgs>?)Delegate.Remove(_serializableHandlers, value);
        else
          _nonSerializableHandlers = (EventHandler<RemovingItemEventArgs>?)Delegate.Remove(_nonSerializableHandlers, value);
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
      _nonSerializableHandlers?.Invoke(this, new RemovingItemEventArgs(removedItem));
      _serializableHandlers?.Invoke(this, new RemovingItemEventArgs(removedItem));
    }

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
      var item = this[index];
      OnRemovingItem(item);
      OnRemoveEventHooks(this[index]);
      base.RemoveItem(index);
    }

    /// <summary>
    /// Add a range of items to the list.
    /// </summary>
    /// <param name="range">List of items to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="range"/> is <see langword="null"/>.</exception>
    public void AddRange(IEnumerable<T> range)
    {
      if (range is null)
        throw new ArgumentNullException(nameof(range));

      foreach (var element in range)
        Add(element);
    }

    [NotUndoable]
    [NonSerialized]
    private BusyChangedEventHandler? _busyChanged = null;

    /// <summary>
    /// Event indicating that the busy status of the
    /// object has changed.
    /// </summary>
    public event BusyChangedEventHandler? BusyChanged
    {
      add => _busyChanged = (BusyChangedEventHandler?)Delegate.Combine(_busyChanged, value);
      remove => _busyChanged = (BusyChangedEventHandler?)Delegate.Remove(_busyChanged, value);
    }

    /// <summary>
    /// Override this method to be notified when the
    /// IsBusy property has changed.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    protected virtual void OnBusyChanged(BusyChangedEventArgs args)
    {
      _busyChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the BusyChanged event for a specific property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="busy">New busy value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected void OnBusyChanged(string propertyName, bool busy)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      OnBusyChanged(new BusyChangedEventArgs(propertyName, busy));
    }

    /// <summary>
    /// Gets the busy status for this object and its child objects.
    /// </summary>
    [Browsable(false)]
    [System.ComponentModel.DataAnnotations.Display(AutoGenerateField = false)]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
    public virtual bool IsBusy => throw new NotImplementedException();

    /// <summary>
    /// Gets the busy status for this object.
    /// </summary>
    [Browsable(false)]
    [System.ComponentModel.DataAnnotations.Display(AutoGenerateField = false)]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
    public virtual bool IsSelfBusy => IsBusy;

    void busy_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

    /// <summary>
    /// Await this method to ensure business object is not busy.
    /// </summary>
    /// <param name="timeout">Timeout duration</param>
    public Task WaitForIdle(TimeSpan timeout)
    {
      return BusyHelper.WaitForIdleAsTimeout(() => WaitForIdle(timeout.ToCancellationToken()), GetType(), nameof(WaitForIdle), timeout);
    }

    /// <summary>
    /// Await this method to ensure the business object is not busy.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    public virtual Task WaitForIdle(CancellationToken ct)
    {
      return BusyHelper.WaitForIdle(this, ct);
    }

    [NotUndoable]
    [NonSerialized]
    private EventHandler<ErrorEventArgs>? _unhandledAsyncException;

    /// <summary>
    /// Event indicating that an exception occurred during
    /// an async operation.
    /// </summary>
    public event EventHandler<ErrorEventArgs>? UnhandledAsyncException
    {
      add => _unhandledAsyncException = (EventHandler<ErrorEventArgs>?)Delegate.Combine(_unhandledAsyncException, value);
      remove => _unhandledAsyncException = (EventHandler<ErrorEventArgs>?)Delegate.Remove(_unhandledAsyncException, value);
    }

    /// <summary>
    /// Method invoked when an unhandled async exception has
    /// occurred.
    /// </summary>
    /// <param name="error">Event arguments.</param>
    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
      _unhandledAsyncException?.Invoke(this, error);
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="originalSender">Original sender of event.</param>
    /// <param name="error">Exception that occurred.</param>
    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncException(new ErrorEventArgs(originalSender, error));
    }

    void unhandled_UnhandledAsyncException(object? sender, ErrorEventArgs e)
    {
      OnUnhandledAsyncException(e);
    }

    /// <summary>
    /// Invoked when an item is inserted into the list.
    /// </summary>
    /// <param name="index">Index of new item.</param>
    /// <param name="item">Reference to new item.</param>
    protected override void InsertItem(int index, T item)
    {
      base.InsertItem(index, item);
      OnAddEventHooks(item);
    }

    /// <summary>
    /// Method invoked when events are hooked for a child
    /// object.
    /// </summary>
    /// <param name="item">Reference to child object.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnAddEventHooks(T item)
    {
      if (item is INotifyBusy busy)
        busy.BusyChanged += busy_BusyChanged;

      if (item is INotifyUnhandledAsyncException unhandled)
        unhandled.UnhandledAsyncException += unhandled_UnhandledAsyncException;

      if (item is INotifyPropertyChanged c)
        c.PropertyChanged += Child_PropertyChanged;

      if (item is INotifyChildChanged child)
        child.ChildChanged += Child_Changed;
    }

    /// <summary>
    /// Method invoked when events are unhooked for a child
    /// object.
    /// </summary>
    /// <param name="item">Reference to child object.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnRemoveEventHooks(T item)
    {
      if (item is INotifyBusy busy)
        busy.BusyChanged -= busy_BusyChanged;

      if (item is INotifyUnhandledAsyncException unhandled)
        unhandled.UnhandledAsyncException -= unhandled_UnhandledAsyncException;

      if (item is INotifyPropertyChanged c)
        c.PropertyChanged -= Child_PropertyChanged;

      if (item is INotifyChildChanged child)
        child.ChildChanged -= Child_Changed;
    }

    void ISerializationNotification.Deserialized()
    {
      // don't rehook events here, because the MobileFormatter has
      // created new objects and so the lists will auto-subscribe
      // the events;
    }

    [NonSerialized]
    [NotUndoable]
    private EventHandler<ChildChangedEventArgs>? _childChangedHandlers;

    /// <summary>
    /// Event raised when a child object has been changed.
    /// </summary>
    public event EventHandler<ChildChangedEventArgs>? ChildChanged
    {
      add => _childChangedHandlers = (EventHandler<ChildChangedEventArgs>?)Delegate.Combine(_childChangedHandlers, value);
      remove => _childChangedHandlers = (EventHandler<ChildChangedEventArgs>?)Delegate.Remove(_childChangedHandlers, value);
    }

    /// <summary>
    /// Raises the ChildChanged event, indicating that a child
    /// object has been changed.
    /// </summary>
    /// <param name="e">
    /// ChildChangedEventArgs object.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnChildChanged(ChildChangedEventArgs e)
    {
      _childChangedHandlers?.Invoke(this, e);
    }

#if ANDROID || IOS

    /// <summary>
    /// Creates a ChildChangedEventArgs and raises the event.
    /// </summary>
    private void RaiseChildChanged(
      object childObject, PropertyChangedEventArgs propertyArgs, NotifyCollectionChangedEventArgs listArgs)
    {
      ChildChangedEventArgs args = new ChildChangedEventArgs(childObject, propertyArgs, listArgs);
      OnChildChanged(args);
    }

    /// <summary>
    /// Handles any PropertyChanged event from 
    /// a child object and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      RaiseChildChanged(sender, e, null);
    }

    /// <summary>
    /// Handles any ChildChanged event from
    /// a child object and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    private void Child_Changed(object sender, ChildChangedEventArgs e)
    {
      RaiseChildChanged(e.ChildObject, e.PropertyChangedArgs, e.CollectionChangedArgs);
    }
#else
    /// <summary>
    /// Creates a ChildChangedEventArgs and raises the event.
    /// </summary>
    private void RaiseChildChanged(object childObject, PropertyChangedEventArgs? propertyArgs, ListChangedEventArgs? listArgs)
    {
      ChildChangedEventArgs args = new ChildChangedEventArgs(childObject, propertyArgs, listArgs);
      OnChildChanged(args);
    }

    /// <summary>
    /// Handles any PropertyChanged event from 
    /// a child object and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="e">Property changed args.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void Child_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      RaiseChildChanged(sender!, e, null);
    }

    /// <summary>
    /// Handles any ChildChanged event from
    /// a child object and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    private void Child_Changed(object? sender, ChildChangedEventArgs e)
    {
      RaiseChildChanged(e.ChildObject, e.PropertyChangedArgs, e.ListChangedArgs);
    }
#endif

    /// <summary>
    /// Use this object to suppress ListChangedEvents for an entire code block.
    /// May be nested in multiple levels for the same object.
    /// </summary>
    public IDisposable SuppressListChangedEvents => new SuppressListChangedEventsClass<T>(this);

    /// <summary>
    /// Handles the suppressing of raising ChangedEvents when altering the content of an ObservableBindingList.
    /// Will be instanciated by a factory property on the ObservableBindingList implementation.
    /// </summary>
    /// <typeparam name="TC">The type of the C.</typeparam>
    class SuppressListChangedEventsClass<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TC> : IDisposable
    {
      private readonly BindingList<TC> _businessObject;
      private readonly bool _initialRaiseListChangedEvents;

      public SuppressListChangedEventsClass(BindingList<TC> businessObject)
      {
        _businessObject = businessObject;
        _initialRaiseListChangedEvents = businessObject.RaiseListChangedEvents;
        businessObject.RaiseListChangedEvents = false;
      }

      public void Dispose()
      {
        _businessObject.RaiseListChangedEvents = _initialRaiseListChangedEvents;
      }
    }
  }
}