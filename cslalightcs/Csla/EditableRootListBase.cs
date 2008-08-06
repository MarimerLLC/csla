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
      if (item.IsDirty || item.IsDeleted)
      {
        int editLevel = item.EditLevel;
        // commit all changes
        for (int tmp = 1; tmp <= editLevel; tmp++)
          item.AcceptChanges(editLevel - tmp, false);
        T savable = item;
        DataPortal<T> dp = new DataPortal<T>();
        dp.UpdateCompleted += (o, e) =>
          {
            if (e.Error == null)
            {
              if (item.IsDeleted)
              {
                // disconnect event handler if necessary
                System.ComponentModel.INotifyPropertyChanged c = item as System.ComponentModel.INotifyPropertyChanged;
                if (c != null)
                {
                  c.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Child_PropertyChanged);
                }
                NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
                OnCollectionChanged(args);
                
                //base.RemoveItem(index);
              }
              else
              {
                
                for (int tmp = 1; tmp <= editLevel; tmp++)
                  e.Object.CopyState(tmp, false);
                //base.SetItem(index, e.Object);
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this[index], item, index));
              }
              item.SaveComplete(e.Object, null);
            }
            else
            {
              item.SaveComplete(item, e.Error);
            }
          };
        dp.BeginUpdate(savable);
      }
    }

    public void SetItemAtIndex(T item, int index)
    {
      base.SetItem(index, item);
    }

    public void RemoveItemAtIndex(int index)
    {
      T item = this[index];

      // only delete/save the item if it is not new
      if (!item.IsNew)
      {
        if (!item.IsDeleted)
        {
          // delete item from database
          item.Delete();
          SaveItem(index);
        }
      }
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
    //protected override void RemoveItem(int index)
    //{
    //  base.RemoveAt(index);      
    //}

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
      if (this.RaiseListChangedEvents)
        base.OnCollectionChanged(e);
    }

    protected override bool SupportsChangeNotificationCore
    {
      get
      {
        return false;
      }
    }

    void Csla.Core.IParent.ApplyEditChild(Core.IEditableBusinessObject child)
    {
      if (child.EditLevel != 0)
        SaveItem((T)child);
    }

    void Csla.Core.IParent.RemoveChild(Core.IEditableBusinessObject child)
    {
      // do nothing, removal of a child is handled by
      // the RemoveItem override
    }

    #endregion

    #region  Cascade Child events

    private void Child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      for (int index = 0; index < this.Count; index++)
      {
        if (ReferenceEquals(this[index], sender))
        {
          NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this[index], this[index], index);
          OnCollectionChanged(args);
          return;
        }
      }
      OnChildPropertyChanged(sender, e);
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
    protected virtual void OnChildPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    { }

    #endregion

    #region  Serialization Notification

    void ISerializationNotification.Deserialized()
    {

      OnDeserialized();
      foreach (Core.IEditableBusinessObject child in this)
      {
        child.SetParent(this);
        System.ComponentModel.INotifyPropertyChanged c = child as System.ComponentModel.INotifyPropertyChanged;
        if (c != null)
        {
          c.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Child_PropertyChanged);
        }
      }

    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    /// <param name="context">Serialization context object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized()
    {

      // do nothing - this is here so a subclass
      // could override if needed

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
