//-----------------------------------------------------------------------
// <copyright file="ObservableBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extends ObservableCollection with behaviors required</summary>
//-----------------------------------------------------------------------

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Csla.Properties;
using Csla.Serialization.Mobile;

namespace Csla.Core
{
  /// <summary>
  /// Extends ObservableCollection with behaviors required
  /// by CSLA .NET collections.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [Serializable]
  public class ObservableBindingList<T> : MobileObservableCollection<T>,
    IObservableBindingList,
    INotifyBusy,
    INotifyChildChanged,
    ISerializationNotification
  {
    #region SupportsChangeNotification

    private bool _supportsChangeNotificationCore = true;
    /// <summary>
    /// Gets a value indicating whether this object
    /// supports change notification.
    /// </summary>
    protected virtual bool SupportsChangeNotificationCore => _supportsChangeNotificationCore;

    #endregion

    #region AddNew

    private bool _raiseListChangedEvents = true;

    /// <summary>
    /// Gets or sets a value indicating whether data binding
    /// can automatically edit items in this collection.
    /// </summary>
    public bool AllowEdit
    {
      get => _allowEdit;
      protected set => _allowEdit = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether data binding
    /// can automatically add new items to this collection.
    /// </summary>
    public bool AllowNew
    {
      get => _allowNew;
      protected set => _allowNew = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether data binding
    /// can automatically remove items from this collection.
    /// </summary>
    public bool AllowRemove
    {
      get => _allowRemove;
      protected set => _allowRemove = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// collection should raise changed events.
    /// </summary>
    public bool RaiseListChangedEvents
    {
      get => _raiseListChangedEvents;
      set => _raiseListChangedEvents = value;
    }

    /// <summary>
    /// Adds a new item to this collection.
    /// </summary>
    public T AddNew()
    {
      var result = AddNewCore();
      OnAddedNew(result);
      return result;
    }

    object IObservableBindingList.AddNew()
    {
      return AddNew()!;
    }

    /// <summary>
    /// Adds a new item to this collection.
    /// </summary>
    public async Task<T> AddNewAsync()
    {
      var result = await AddNewCoreAsync();
      OnAddedNew(result);
      return result!;
    }

    async Task<object> IObservableBindingList.AddNewAsync()
    {
      return (await AddNewAsync())!;
    }

    #endregion

    #region RemovingItem event

    [NonSerialized]
    private EventHandler<RemovingItemEventArgs>? _removingItemHandler;

    /// <summary>
    /// Implements a serialization-safe RemovingItem event.
    /// </summary>
    public event EventHandler<RemovingItemEventArgs>? RemovingItem
    {
      add => _removingItemHandler = (EventHandler<RemovingItemEventArgs>?)Delegate.Combine(_removingItemHandler, value);
      remove => _removingItemHandler = (EventHandler<RemovingItemEventArgs>?)Delegate.Remove(_removingItemHandler, value);
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
      _removingItemHandler?.Invoke(this,
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
      OnRemoveEventHooks(this[index]);
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
        Add(element);
    }

    #endregion

    #region INotifyPropertyBusy Members

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
    protected void OnBusyChanged(string propertyName, bool busy)
    {
      OnBusyChanged(new BusyChangedEventArgs(propertyName, busy));
    }

    /// <summary>
    /// Gets the busy status for this object and its child objects.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public virtual bool IsBusy => false;

    /// <summary>
    /// Gets the busy status for this object.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
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
      return BusyHelper.WaitForIdleAsTimeout(WaitForIdle, GetType(), nameof(WaitForIdle), timeout);
    }

    /// <summary>
    /// Await this method to ensure the business object is not busy.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    public virtual Task WaitForIdle(CancellationToken ct)
    {
      return BusyHelper.WaitForIdle(this, ct);
    }

    #endregion

    #region INotifyUnhandledAsyncException Members

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

      //IBindingList list = item as IBindingList;
      //if (list != null)
      //  list.ListChanged += new ListChangedEventHandler(Child_ListChanged);

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

      //IBindingList list = item as IBindingList;
      //if(list!=null)
      //  list.ListChanged -= new ListChangedEventHandler(Child_ListChanged);

      if (item is INotifyChildChanged child)
        child.ChildChanged -= Child_Changed;
    }

    #endregion

    #region ISerializationNotification Members

    void ISerializationNotification.Deserialized()
    {
      // don't rehook events here, because the MobileFormatter has
      // created new objects and so the lists will auto-subscribe
      // the events
    }

    #endregion

    #region Child Change Notification

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

    /// <summary>
    /// Creates a ChildChangedEventArgs and raises the event.
    /// </summary>
    private void RaiseChildChanged(object childObject, PropertyChangedEventArgs? propertyArgs, NotifyCollectionChangedEventArgs? listArgs)
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
      // Issue 813
      // MetaPropertyHasChanged calls in OnChildChanged we're leading to exponential growth in OnChildChanged calls
      // Those notifications are for the UI. Ignore them in the parent
      if (!(e is MetaPropertyChangedEventArgs) && sender is not null)
      {
        RaiseChildChanged(sender, e, null);
      }
    }

    /// <summary>
    /// Handles any ChildChanged event from
    /// a child object and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    private void Child_Changed(object? sender, ChildChangedEventArgs e)
    {
      RaiseChildChanged(e.ChildObject, e.PropertyChangedArgs, e.CollectionChangedArgs);
    }

    #endregion

    #region AddNewCore

    [NotUndoable]
    [NonSerialized]
    private EventHandler<AddedNewEventArgs<T>>? _addedNewHandlers = null;

    private bool _allowEdit;
    private bool _allowNew;
    private bool _allowRemove;

    /// <summary>
    /// Event raised when a new object has been 
    /// added to the collection.
    /// </summary>
    public event EventHandler<AddedNewEventArgs<T>>? AddedNew
    {
      add => _addedNewHandlers = (EventHandler<AddedNewEventArgs<T>>?)Delegate.Combine(_addedNewHandlers, value);
      remove => _addedNewHandlers = (EventHandler<AddedNewEventArgs<T>>?)Delegate.Remove(_addedNewHandlers, value);
    }

    /// <summary>
    /// Raises the AddedNew event.
    /// </summary>
    /// <param name="item"></param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void OnAddedNew(T item)
    {
      if (_addedNewHandlers != null)
      {
        var args = new AddedNewEventArgs<T>(item);
        _addedNewHandlers(this, args);
      }
    }

#if ANDROID || IOS
    /// <summary>
    /// Override this method to create a new object that is added
    /// to the collection. 
    /// </summary>
    protected virtual void AddNewCore()
    {
      throw new NotImplementedException(Resources.AddNewCoreMustBeOverriden);
    }
#else
    /// <summary>
    /// Override this method to create a new object that is added
    /// to the collection. 
    /// </summary>
    protected virtual T AddNewCore()
    {
      throw new NotImplementedException(Resources.AddNewCoreMustBeOverriden);
    }

    /// <summary>
    /// Override this method to create a new object that is added
    /// to the collection. 
    /// </summary>
    protected virtual async Task<T> AddNewCoreAsync()
    {
      return await Task.FromException<T>(new NotImplementedException(Resources.AddNewCoreMustBeOverriden));
    }
#endif

    #endregion

    #region OnCollectionChanged

    /// <summary>
    /// Raises the CollectionChanged event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (SupportsChangeNotificationCore && RaiseListChangedEvents)
        base.OnCollectionChanged(e);
    }

    #endregion

    #region MobileObject

    /// <summary>
    /// Override this method to get custom field values
    /// from the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnGetState(SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("Csla.Core.MobileList.AllowEdit", AllowEdit);
      info.AddValue("Csla.Core.MobileList.AllowNew", AllowNew);
      info.AddValue("Csla.Core.MobileList.AllowRemove", AllowRemove);
      info.AddValue("Csla.Core.MobileList.RaiseListChangedEvents", RaiseListChangedEvents);
      info.AddValue("Csla.Core.MobileList._supportsChangeNotificationCore", _supportsChangeNotificationCore);
    }

    /// <summary>
    /// Override this method to set custom field values
    /// into the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnSetState(SerializationInfo info)
    {
      base.OnSetState(info);
      AllowEdit = info.GetValue<bool>("Csla.Core.MobileList.AllowEdit");
      AllowNew = info.GetValue<bool>("Csla.Core.MobileList.AllowNew");
      AllowRemove = info.GetValue<bool>("Csla.Core.MobileList.AllowRemove");
      RaiseListChangedEvents = info.GetValue<bool>("Csla.Core.MobileList.RaiseListChangedEvents");
      _supportsChangeNotificationCore = info.GetValue<bool>("Csla.Core.MobileList._supportsChangeNotificationCore");
    }

    #endregion

    #region SuppressLListChanged

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
    class SuppressListChangedEventsClass<TC> : IDisposable
    {
      private readonly ObservableBindingList<TC> _businessObject;
      private readonly bool _initialRaiseListChangedEvents;

      public SuppressListChangedEventsClass(ObservableBindingList<TC> businessObject)
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

    #endregion

    [NonSerialized]
    [NotUndoable]
    private Stack<bool>? _oldRLCE;

    /// <summary>
    /// Sets the load list mode for the list
    /// </summary>
    /// <param name="enabled">Enabled value</param>
    protected override void SetLoadListMode(bool enabled)
    {
      if (_oldRLCE == null)
        _oldRLCE = new Stack<bool>();
      if (enabled)
      {
        _oldRLCE.Push(_raiseListChangedEvents);
        _raiseListChangedEvents = false;
      }
      else
      {
        if (_oldRLCE.Count > 0)
          _raiseListChangedEvents = _oldRLCE.Pop();
      }
    }
  }
}