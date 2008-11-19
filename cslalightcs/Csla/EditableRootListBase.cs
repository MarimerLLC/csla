using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using Csla.DataPortalClient;
using System.Collections.Specialized;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;

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
  [Serializable()]
  public abstract class EditableRootListBase<T> : Core.ExtendedBindingList<T>, Core.IParent, IDataPortalTarget, ISerializationNotification
    where T : Core.IEditableBusinessObject, Core.IUndoableObject, Core.ISavable, IMobileObject
  {

    #region  SaveItem Methods

    public event EventHandler<Csla.Core.SavedEventArgs> Saved;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void OnSaved(T newObject, Exception error)
    {
      if (Saved != null)
        Saved(this, new SavedEventArgs(newObject, error, null));
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
    public virtual void SaveItem(int index)
    {

      T item = this[index];
      if (item.IsDeleted || (item.IsValid && item.IsDirty))
      {
        T savable = item;

        // attempt to clone object
        ICloneable cloneable = savable as ICloneable;
        if (cloneable != null)
          savable = (T)cloneable.Clone();

        // commit all changes
        int editLevel = savable.EditLevel;
        for (int tmp = 1; tmp <= editLevel; tmp++)
          savable.AcceptChanges(editLevel - tmp, false);

        // save object
        DataPortal<T> dp = new DataPortal<T>();
        dp.UpdateCompleted += (o, e) =>
          {
            if (e.Error == null)
            {
              T result = e.Object;
              if (item.IsDeleted)
              {
                //SafeRemoveItem  will raise INotifyCollectionChanged event
                SafeRemoveItem(index);
              }
              else
              {
                for (int tmp = 1; tmp <= editLevel; tmp++)
                  result.CopyState(tmp, false);

                SafeSetItem(index, result);
                //Because SL Data Grid does not support replace action.
                // we have to artificially raise remove/insert events
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this[index], index));

              }
              item.SaveComplete(result, null, null);
              OnSaved(result, null);
            }
            else
            {
              item.SaveComplete(item, e.Error, null);
              OnSaved(item, e.Error);
            }
          };
        dp.BeginUpdate(savable);
      }
    }

    private void SafeSetItem(int index, T newObject)
    {
      //This is needed because we cannot call base.SetItem from lambda expression
      newObject.SetParent(this);
      base.SetItem(index, newObject);
    }

    private void SafeRemoveItem(int index)
    {
      this[index].SetParent(null);
      base.RemoveItem(index);
    }

    #endregion

    #region  Insert, Remove, Clear

    /// <summary>
    /// Gives the new object a parent reference to this
    /// list.
    /// </summary>
    /// <param name="index">Index at which to insert the item.</param>
    /// <param name="item">Item to insert.</param>
    protected override void InsertItem(int index, T item)
    {
      item.SetParent(this);
      base.InsertItem(index, item);
    }

    ///// <summary>
    ///// Removes an item from the list.
    ///// </summary>
    ///// <param name="index">Index of the item
    ///// to be removed.</param>
    protected override void RemoveItem(int index)
    {
      T item = this[index];
      if (item.IsDeleted == false)
      {
        // only delete/save the item if it is not new
        if (!item.IsNew)
        {
          item.Delete();
          SaveItem(index);
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

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      // SL Data Grid's DataGridDataConnection object does not support replace action.  It throws an excpetioon when this occurs.
      if (this.RaiseListChangedEvents && e.Action != NotifyCollectionChangedAction.Replace)
        base.OnCollectionChanged(e);
    }

    protected override bool SupportsChangeNotificationCore
    {
      get
      {
        return true;
      }
    }

    void Csla.Core.IParent.ApplyEditChild(Core.IEditableBusinessObject child)
    {
      if (child.EditLevel == 0)
        SaveItem((T)child);
    }

    void Csla.Core.IParent.RemoveChild(Core.IEditableBusinessObject child)
    {
      // do nothing, removal of a child is handled by
      // the RemoveItem override
    }

    #endregion

    #region IsBusy
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

    #region  Serialization Notification

    protected internal override void OnDeserializedInternal()
    {
      foreach (IEditableBusinessObject child in this)
        child.SetParent(this);
      
      base.OnDeserializedInternal();
    }

    #endregion

    #region  Data Access

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Properties.Resources.CreateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Properties.Resources.FetchNotSupportedException);
    }

    private void DataPortal_Update()
    {
      throw new NotSupportedException(Properties.Resources.UpdateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
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

    void IDataPortalTarget.MarkAsChild()
    { }

    void IDataPortalTarget.MarkNew()
    { }

    void IDataPortalTarget.MarkOld()
    { }

    void IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.DataPortal_OnDataPortalException(e, ex);
    }

    void IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    void IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    void IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    #endregion
  }
}
