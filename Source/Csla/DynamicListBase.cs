﻿//-----------------------------------------------------------------------
// <copyright file="DynamicListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which collections</summary>
//-----------------------------------------------------------------------

using System.Collections.Specialized;
using System.ComponentModel;
using Csla.Core;
using Csla.Reflection;
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
  public abstract class DynamicListBase<T> :
#if ANDROID || IOS
    ExtendedBindingList<T>,
#else
    ObservableBindingList<T>,
#endif
    IParent,
    Server.IDataPortalTarget,
    IBusinessObject,
    IUseApplicationContext
    where T : IEditableBusinessObject, IUndoableObject, ISavable, IMobileObject, IBusinessObject
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public DynamicListBase()
    {
      InitializeIdentity();
      Initialize();
      AllowNew = true;
    }

    /// <summary>
    /// Gets the current ApplicationContext
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }
    ApplicationContext IUseApplicationContext.ApplicationContext { get => ApplicationContext; set => ApplicationContext = value; }

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

    int IBusinessObject.Identity
    {
      get { return _identity; }
    }

    private void InitializeIdentity()
    {
      _identity = ((IParent)this).GetNextIdentity(_identity);
    }

    [NonSerialized]
    [NotUndoable]
    private IdentityManager _identityManager;

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

    /// <summary>
    /// Event raised when an object in the list has been saved.
    /// </summary>
    public event EventHandler<SavedEventArgs> Saved;

    /// <summary>
    /// Raises the Saved event.
    /// </summary>
    /// <param name="newObject">Object returned as a result
    /// of the save operation.</param>
    /// <param name="error">Exception returned as a result
    /// of the save operation.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSaved(T newObject, Exception error)
    {
      Saved?.Invoke(this, new SavedEventArgs(newObject, error, null));
    }

    /// <summary>
    /// Saves the specified item in the list.
    /// </summary>
    /// <param name="item">Item to be saved.</param>
    public Task SaveItemAsync(T item)
    {
      return SaveItemAsync(IndexOf(item));
    }

    /// <summary>
    /// Saves the specified item in the list.
    /// </summary>
    /// <param name="index">Index of item to be saved.</param>
    public Task SaveItemAsync(int index)
    {
      return SaveItemAsync(index, false);
    }

    /// <summary>
    /// Saves the specified item in the list.
    /// </summary>
    /// <param name="index">Index of item to be saved.</param>
    /// <param name="delete">true if the item should be deleted.</param>
    protected virtual async Task SaveItemAsync(int index, bool delete)
    {
      T item = this[index];
      var handleBusy = false;
      if ((item.IsDeleted || delete) || (item.IsValid && item.IsDirty))
      {
        T savable = item;

        // attempt to clone object
        if (savable is ICloneable cloneable)
        {
          savable = (T)cloneable.Clone();
          MethodCaller.CallMethodIfImplemented(item, "MarkBusy");
          handleBusy = true;
        }

        // commit all changes
        int editLevel = savable.EditLevel;
        for (int tmp = 1; tmp <= editLevel; tmp++)
          savable.AcceptChanges(editLevel - tmp, false);

        if (delete)
          savable.Delete();

        Exception error = null;
        T result = default(T);
        var dp = ApplicationContext.CreateInstanceDI<DataPortal<T>>();
        try
        {
          result = await dp.UpdateAsync(savable);
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
            error = ex.InnerExceptions[0];
          else
            error = ex;
        }
        catch (Exception ex)
        {
          error = ex;
        }
        finally
        {
          if (handleBusy)
            MethodCaller.CallMethodIfImplemented(item, "MarkIdle");
        }
        // update index - this may have changed under the duration of async call 
        index = IndexOf(item);
        if (error == null && result != null)
        {
          if (savable.IsDeleted)
          {
            //SafeRemoveItem  will raise INotifyCollectionChanged event
            SafeRemoveItem(index);
          }
          else
          {
            for (int tmp = 1; tmp <= editLevel; tmp++)
              result.CopyState(tmp, false);

            SafeSetItem(index, result);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this[index], index));
          }
          item.SaveComplete(result);
          OnSaved(result, null);
        }
        else
        {
          item.SaveComplete(item);
          OnSaved(item, error);
        }
      }
    }

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
    public void SaveItem(T item)
    {
      SaveItem(IndexOf(item));
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
    public async void SaveItem(int index)
    {
      try
      {
        await SaveItemAsync(index);
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          throw ex.InnerExceptions[0];
        else
          throw;
      }
    }

    private void SafeSetItem(int index, T newObject)
    {
      //This is needed because we cannot call base.SetItem from lambda expression
      this[index].SetParent(null);
      base.OnRemoveEventHooks(this[index]);
      newObject.SetParent(this);
      base.SetItem(index, newObject);
      base.OnAddEventHooks(newObject);
    }

    private void SafeRemoveItem(int index)
    {
      this[index].SetParent(null);
      base.OnRemoveEventHooks(this[index]);
      base.RemoveItem(index);
    }

    #endregion

    #region  Insert, Remove, Clear

    /// <summary>
    /// Adds a new item to the list.
    /// </summary>
    /// <returns>The added object</returns>
    protected override T AddNewCore()
    {
      var dp = ApplicationContext.CreateInstanceDI<DataPortal<T>>();
      T item = dp.Create();
      Add(item);
      return item;
    }

    /// <summary>
    /// Adds a new item to the list.
    /// </summary>
    /// <returns>The added object</returns>
    protected override async Task<T> AddNewCoreAsync()
    {
      var dp = ApplicationContext.CreateInstanceDI<DataPortal<T>>();
      T item = await dp.CreateAsync();
      Add(item);
      return item;
    }

    /// <summary>
    /// Gives the new object a parent reference to this
    /// list.
    /// </summary>
    /// <param name="index">Index at which to insert the item.</param>
    /// <param name="item">Item to insert.</param>
    protected override void InsertItem(int index, T item)
    {
      item.SetParent(this);
      // ensure child uses same context as parent
      if (item is IUseApplicationContext iuac)
        iuac.ApplicationContext = ApplicationContext;
      base.InsertItem(index, item);
    }

    /// <summary>
    /// Removes an item from the list.
    /// </summary>
    /// <param name="index">Index of the item
    /// to be removed.</param>
    protected override async void RemoveItem(int index)
    {
      T item = this[index];
      if (item.IsDeleted == false)
      {
        // only delete/save the item if it is not new
        if (!item.IsNew)
        {
          try
          {
            await SaveItemAsync(index, true);
          }
          catch (AggregateException ex)
          {
            if (ex.InnerExceptions.Count > 0)
              throw ex.InnerExceptions[0];
            else
              throw;
          }
        }
        else
        {
          SafeRemoveItem(index);
          OnSaved(item, null);
        }
      }
    }

    /// <summary>
    /// Replaces item in the list.
    /// </summary>
    /// <param name="index">Index of the item
    /// that was replaced.</param>
    /// <param name="item">New item.</param>
    protected override void SetItem(int index, T item)
    {
      item.SetParent(this);
      base.SetItem(index, item);
    }

    #endregion

    #region  IParent Members

    /// <summary>
    /// Gets or sets a value indicating whether the Replace
    /// event should be raised when OnCollectionChanged() is
    /// called.
    /// </summary>
    /// <remarks>
    /// There's a bug in DataGridDataConnection that throws
    /// an exception on the replace action. By default we
    /// disable raising the replace event to avoid that bug, 
    /// but some other datagrid controls require the event.
    /// </remarks>
    protected bool RaiseReplaceEvents { get; set; }

    /// <summary>
    /// Raises the CollectionChanged event.
    /// </summary>
    /// <param name="e">Event args object</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      // SL Data Grid's DataGridDataConnection object does not support replace action.  
      // It throws an excpetioon when this occurs.
      if (RaiseListChangedEvents && (e.Action != NotifyCollectionChangedAction.Replace || RaiseReplaceEvents))
        base.OnCollectionChanged(e);
    }

    /// <summary>
    /// Gets a value indicating whether this collection
    /// supports change notification (always returns true).
    /// </summary>
    protected override bool SupportsChangeNotificationCore
    {
      get
      {
        return true;
      }
    }

    void IParent.ApplyEditChild(IEditableBusinessObject child)
    {
      if (child.EditLevel == 0)
        SaveItem((T)child);
    }

    void IParent.RemoveChild(IEditableBusinessObject child)
    {
      // do nothing, removal of a child is handled by
      // the RemoveItem override
    }


    IParent Csla.Core.IParent.Parent
    {
      get { return null; }
    }

    #endregion

    #region IsBusy

    /// <summary>
    /// Await this method to ensure business object
    /// is not busy running async rules.
    /// </summary>
    public async Task WaitForIdle()
    {
      var cslaOptions = ApplicationContext.GetRequiredService<Configuration.CslaOptions>();
      await WaitForIdle(TimeSpan.FromSeconds(cslaOptions.DefaultWaitForIdleTimeoutInSeconds)).ConfigureAwait(false);
    }

    /// <summary>
    /// Await this method to ensure business object
    /// is not busy running async rules.
    /// </summary>
    /// <param name="timeout">Timeout duration</param>
    public Task WaitForIdle(TimeSpan timeout)
    {
      return BusyHelper.WaitForIdle(this, timeout);
    }

    /// <summary>
    /// Await this method to ensure the business object
    /// is not busy running async rules.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    public Task WaitForIdle(CancellationToken ct)
    {
        return BusyHelper.WaitForIdle(this, ct);
    }

    /// <summary>
    /// Gets a value indicating whether this object
    /// or any child object is currently executing
    /// an async operation.
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

    #region  Data Access

    private void DataPortal_Update()
    {
      throw new NotSupportedException(Properties.Resources.UpdateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member"), EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member"), EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member"), EditorBrowsable(EditorBrowsableState.Advanced)]
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