//-----------------------------------------------------------------------
// <copyright file="DynamicBindingListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which collections</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Serialization.Mobile;

namespace Csla
{
  /// <summary>
  /// This is the base class from which collections
  /// of editable root business objects should be
  /// derived.
  /// </summary>
  /// <typeparam name="T">
  /// Type of editable root object to contain within
  /// the collection.
  /// </typeparam>
  /// <remarks>
  /// <para>
  /// Your subclass should implement a factory method
  /// and should override or overload
  /// DataPortal_Fetch() to implement data retrieval.
  /// </para><para>
  /// Saving (inserts or updates) of items in the collection
  /// should be handled through the SaveItem() method on
  /// the collection. 
  /// </para><para>
  /// Removing an item from the collection
  /// through Remove() or RemoveAt() causes immediate deletion
  /// of the object, by calling the object's Delete() and
  /// Save() methods.
  /// </para>
  /// </remarks>
  [Serializable]
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public abstract class DynamicBindingListBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T> :
    ExtendedBindingList<T>,
    IParent,
    Server.IDataPortalTarget,
    IBusinessObject,
    IUseApplicationContext
    where T : IEditableBusinessObject, IUndoableObject, ISavable, IMobileObject, IBusinessObject
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
#pragma warning disable CS8618 // It must be available for derived classes to be de-/serialized
    protected DynamicBindingListBase()
#pragma warning restore CS8618 
    { }

    /// <summary>
    /// Gets the current ApplicationContext
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }
    
    /// <inheritdoc />
    ApplicationContext IUseApplicationContext.ApplicationContext
    {
      get => ApplicationContext;
      set
      {
        ApplicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext));
        InitializeIdentity();
        Initialize();
        AllowNew = true;
      }
    }

    #region Initialize

    /// <summary>
    /// Override this method to set up event handlers so user
    /// code in a partial class can respond to events raised by
    /// generated code.
    /// </summary>
    protected virtual void Initialize()
    { /* allows subclass to initialize events before any other activity occurs */ }

    #endregion

    #region Identity

    private int _identity = -1;

    int IBusinessObject.Identity => _identity;

    private void InitializeIdentity()
    {
      _identity = ((IParent)this).GetNextIdentity(_identity);
    }

    [NonSerialized]
    [NotUndoable]
    private IdentityManager? _identityManager;

    int IParent.GetNextIdentity(int current)
    {
      var me = (IParent)this;
      if (me.Parent != null)
      {
        return me.Parent.GetNextIdentity(current);
      }
      else
      {
        if (_identityManager == null)
          _identityManager = new IdentityManager();
        return _identityManager.GetNextIdentity(current);
      }
    }

    #endregion

    #region  SaveItem Methods

    private bool _activelySaving;

    /// <summary>
    /// Saves the specified item in the list.
    /// </summary>
    /// <param name="item">
    /// Reference to the item to be saved.
    /// </param>
    /// <remarks>
    /// This method properly saves the child item,
    /// by making sure the item in the collection
    /// is properly replaced by the result of the
    /// Save() method call.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public T SaveItem(T item)
    {
      if(item is null) 
        throw new ArgumentNullException(nameof(item));

      return SaveItem(IndexOf(item));
    }

    /// <summary>
    /// Saves the specified item in the list.
    /// </summary>
    /// <param name="index">
    /// Index of the item to be saved.
    /// </param>
    /// <remarks>
    /// This method properly saves the child item,
    /// by making sure the item in the collection
    /// is properly replaced by the result of the
    /// Save() method call.
    /// </remarks>
    public virtual T SaveItem(int index)
    {
      bool raisingEvents = RaiseListChangedEvents;
      RaiseListChangedEvents = false;
      _activelySaving = true;

      T result;
      try
      {
        T item = this[index];
        result = item;
        T savable = item;

        // clone the object if possible
        if (savable is ICloneable clonable)
          savable = (T)clonable.Clone();

        // commit all changes
        int editLevel = savable.EditLevel;
        for (int tmp = 1; tmp <= editLevel; tmp++)
          savable.AcceptChanges(editLevel - tmp, false);

        // do the save
        result = (T)savable.Save();

        if (!ReferenceEquals(result, item))
        {
          // restore edit level to previous level
          for (int tmp = 1; tmp <= editLevel; tmp++)
            result.CopyState(tmp, false);

          // put result into collection
          this[index] = result;
        }

        if (!ReferenceEquals(savable, item))
        {
          // raise Saved event from original object
          if (item is ISavable original)
            original.SaveComplete(result);
        }

        OnSaved(result, null);
      }
      finally
      {
        _activelySaving = false;
        RaiseListChangedEvents = raisingEvents;
      }
      OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
      return result;
    }

    #endregion

    #region Saved Event
    [NonSerialized]
    [NotUndoable]
    private EventHandler<SavedEventArgs>? _nonSerializableSavedHandlers;
    [NotUndoable]
    private EventHandler<SavedEventArgs>? _serializableSavedHandlers;

    /// <summary>
    /// Event raised when an object has been saved.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public event EventHandler<SavedEventArgs>? Saved
    {
      add
      {
        if (value is null)
          return;

        if (value.Method.IsPublic && value.Method.IsStatic)
          _serializableSavedHandlers = (EventHandler<SavedEventArgs>)Delegate.Combine(_serializableSavedHandlers, value);
        else
          _nonSerializableSavedHandlers = (EventHandler<SavedEventArgs>)Delegate.Combine(_nonSerializableSavedHandlers, value);
      }
      remove
      {
        if (value is null)
          return;

        if (value.Method.IsPublic && value.Method.IsStatic)
          _serializableSavedHandlers = (EventHandler<SavedEventArgs>?)Delegate.Remove(_serializableSavedHandlers, value);
        else
          _nonSerializableSavedHandlers = (EventHandler<SavedEventArgs>?)Delegate.Remove(_nonSerializableSavedHandlers, value);
      }
    }

    /// <summary>
    /// Raises the Saved event.
    /// </summary>
    /// <param name="newObject">
    /// Reference to object returned from the save.
    /// </param>
    /// <param name="e">
    /// Reference to any exception that occurred during
    /// the save.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="newObject"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSaved(T newObject, Exception? e)
    {
      if (newObject == null) 
        throw new ArgumentNullException(nameof(newObject));

      SavedEventArgs args = new SavedEventArgs(newObject, e, null);
      _nonSerializableSavedHandlers?.Invoke(this, args);
      _serializableSavedHandlers?.Invoke(this, args);
    }

    #endregion

    #region  Insert, Remove, Clear

    /// <summary>
    /// Adds a new item to the list.
    /// </summary>
    /// <returns>The added object</returns>
    protected override object AddNewCore()
    {
      var dp = ApplicationContext.CreateInstanceDI<DataPortal<T>>();
      T item = dp.Create();
      Add(item);
      OnAddingNew(new AddingNewEventArgs(item));
      return item;
    }

    /// <summary>
    /// Gives the new object a parent reference to this
    /// list.
    /// </summary>
    /// <param name="index">Index at which to insert the item.</param>
    /// <param name="item">Item to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    protected override void InsertItem(int index, T item)
    {
      if(item is null) 
        throw new ArgumentNullException(nameof(item));

      IdentityManager.EnsureNextIdentityValueIsUnique(this, this);
      item.SetParent(this);
      base.InsertItem(index, item);
    }

    /// <summary>
    /// Removes an item from the list.
    /// </summary>
    /// <param name="index">Index of the item
    /// to be removed.</param>
    protected override void RemoveItem(int index)
    {
      // delete item from database
      T item = this[index];

      // only delete/save the item if it is not new
      bool raiseEventForNewItem = false;
      if (!item.IsNew)
      {
        item.Delete();
        SaveItem(index);
      }
      else
      {
        raiseEventForNewItem = true;
      }

      base.RemoveItem(index);
      if (raiseEventForNewItem)
        OnSaved(item, null);
    }

    /// <summary>
    /// Replaces item in the list.
    /// </summary>
    /// <param name="index">Index of the item
    /// that was replaced.</param>
    /// <param name="item">New item.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    protected override void SetItem(int index, T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      item.SetParent(this);
      base.SetItem(index, item);
    }

    #endregion

    #region  IParent Members

    Task IParent.ApplyEditChild(IEditableBusinessObject child)
    {
      if (child is null)
        throw new ArgumentNullException(nameof(child));

      if (!_activelySaving && child.EditLevel == 0)
        SaveItem((T)child);
      return Task.CompletedTask;
    }

    Task IParent.RemoveChild(IEditableBusinessObject child)
    {
      if (child is null)
        throw new ArgumentNullException(nameof(child));

      if (child.IsNew)
        Remove((T)child);
      return Task.CompletedTask;
    }

    IParent? IParent.Parent => null;

    #endregion

    #region  Cascade Child events

    /// <summary>
    /// Handles any PropertyChanged event from 
    /// a child object and echoes it up as
    /// a ChildChanged event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="e">Property changed args.</param>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void Child_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e is null)
        throw new ArgumentNullException(nameof(e));

      for (int index = 0; index < Count; index++)
      {
        if (ReferenceEquals(this[index], sender))
        {
          PropertyDescriptor? descriptor = GetPropertyDescriptor(e.PropertyName ?? "");
          if (descriptor != null)
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index, descriptor));
          else
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
          return;
        }
      }
      OnChildPropertyChanged(sender, e);
      base.Child_PropertyChanged(sender, e);
    }

    void Child_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      OnBusyChanged(e);
    }

    /// <summary>
    /// Override this method to be notified when a child object
    /// has been changed.
    /// </summary>
    /// <param name="sender">
    /// Child object where the PropertyChanged event originated.
    /// </param>
    /// <param name="e">
    /// PropertyChangedEventArgs from the child object.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnChildPropertyChanged(object? sender, PropertyChangedEventArgs e)
    { }

    private static PropertyDescriptorCollection? _propertyDescriptors;

    private PropertyDescriptor? GetPropertyDescriptor(string propertyName)
    {
      if (_propertyDescriptors == null)
        _propertyDescriptors = TypeDescriptor.GetProperties(GetType());
      PropertyDescriptor? result = null;
      foreach (PropertyDescriptor desc in _propertyDescriptors)
      {
        if (desc.Name == propertyName)
        {
          result = desc;
          break;
        }
      }
      return result;
    }

    #endregion

    #region  Data Access

    private void DataPortal_Update()
    {
      throw new NotSupportedException(Properties.Resources.UpdateNotSupportedException);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [Delete]
    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Properties.Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member"), EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member"), EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member"), EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    #endregion

    #region ToArray

    /// <summary>
    /// Get an array containing all items in the list.
    /// </summary>
    public T[] ToArray()
    {
      List<T> result = new List<T>();
      foreach (T item in this)
        result.Add(item);
      return result.ToArray();
    }

    #endregion

    #region IDataPortalTarget Members

    void Server.IDataPortalTarget.CheckRules()
    { }

    Task Server.IDataPortalTarget.CheckRulesAsync() => Task.CompletedTask;

    void Server.IDataPortalTarget.MarkAsChild()
    { }

    void Server.IDataPortalTarget.MarkNew()
    { }

    void Server.IDataPortalTarget.MarkOld()
    { }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvoke(e);
    }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvokeComplete(e);
    }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      DataPortal_OnDataPortalException(e, ex);
    }

    void Server.IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    void Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    void Server.IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    #endregion

    #region IsBusy

    /// <summary>
    /// Await this method to ensure business object is not busy.
    /// </summary>
    public async Task WaitForIdle()
    {
      var cslaOptions = ApplicationContext.GetRequiredService<Configuration.CslaOptions>();
      await WaitForIdle(TimeSpan.FromSeconds(cslaOptions.DefaultWaitForIdleTimeoutInSeconds)).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a value indicating whether this object
    /// is currently running an async operation.
    /// </summary>
    public override bool IsBusy
    {
      get
      {
        // run through all the child objects
        // and if any are dirty then then
        // collection is dirty
        foreach (T child in this)
          if (child.IsBusy)
            return true;

        return false;
      }
    }

    #endregion

    #region Mobile object overrides

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnGetState(SerializationInfo info)
    {
      info.AddValue("Csla.Core.BusinessBase._identity", _identity);
      base.OnGetState(info);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnSetState(SerializationInfo info)
    {
      _identity = info.GetValue<int>("Csla.Core.BusinessBase._identity");
      base.OnSetState(info);
    }

    #endregion
  }
}
