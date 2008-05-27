using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

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
  public abstract class EditableRootListBase<T> : Core.ExtendedBindingList<T>, Core.IParent, Server.IDataPortalTarget
    where T : Core.IEditableBusinessObject, Core.IUndoableObject, Core.ISavable
  {

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
    public T SaveItem(T item)
    {
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
      bool raisingEvents = this.RaiseListChangedEvents;
      this.RaiseListChangedEvents = false;

      _activelySaving = true;

      T item = this[index];
      int editLevel = item.EditLevel;
      // commit all changes
      for (int tmp = 1; tmp <= editLevel; tmp++)
        item.AcceptChanges(editLevel - tmp, false);
      try
      {
        T savable = item;
        if (!Csla.ApplicationContext.AutoCloneOnUpdate)
        {
          // clone the object if possible
          ICloneable clonable = savable as ICloneable;
          if (clonable != null)
            savable = (T)clonable.Clone();
        }

        // do the save
        this[index] = (T)savable.Save();

        if (!ReferenceEquals(savable, item) && !Csla.ApplicationContext.AutoCloneOnUpdate)
        {
          // raise Saved event from original object
          Core.ISavable original = item as Core.ISavable;
          if (original != null)
            original.SaveComplete(this[index]);
        }
      }
      finally
      {
        // restore edit level to previous level
        for (int tmp = 1; tmp <= editLevel; tmp++)
          item.CopyState(tmp, false);
        _activelySaving = false;
        this.RaiseListChangedEvents = raisingEvents;
      }
      this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
      return this[index];
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
      if (!item.IsNew)
      {
        item.Delete();
        SaveItem(index);
      }

      // disconnect event handler if necessary
      System.ComponentModel.INotifyPropertyChanged c = item as System.ComponentModel.INotifyPropertyChanged;
      if (c != null)
      {
        c.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Child_PropertyChanged);
      }

      base.RemoveItem(index);
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

    void Csla.Core.IParent.ApplyEditChild(Core.IEditableBusinessObject child)
    {
      if (!_activelySaving && child.EditLevel == 0)
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
          PropertyDescriptor descriptor = GetPropertyDescriptor(e.PropertyName);
          if (descriptor != null)
            OnListChanged(new ListChangedEventArgs(
              ListChangedType.ItemChanged, index, GetPropertyDescriptor(e.PropertyName)));
          else
            OnListChanged(new ListChangedEventArgs(
              ListChangedType.ItemChanged, index));
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

    private static PropertyDescriptorCollection _propertyDescriptors;

    private PropertyDescriptor GetPropertyDescriptor(string propertyName)
    {
      if (_propertyDescriptors == null)
        _propertyDescriptors = TypeDescriptor.GetProperties(this.GetType());
      PropertyDescriptor result = null;
      foreach (PropertyDescriptor desc in _propertyDescriptors)
        if (desc.Name == propertyName)
        {
          result = desc;
          break;
        }
      return result;
    }

    #endregion

    #region  Serialization Notification

    [OnDeserialized()]
    private void OnDeserializedHandler(StreamingContext context)
    {

      OnDeserialized(context);
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
    protected virtual void OnDeserialized(StreamingContext context)
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

    void Csla.Server.IDataPortalTarget.MarkAsChild()
    { }

    void Csla.Server.IDataPortalTarget.MarkNew()
    { }

    void Csla.Server.IDataPortalTarget.MarkOld()
    { }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvoke(e);
    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvokeComplete(e);
    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.DataPortal_OnDataPortalException(e, ex);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    #endregion
  }
}
