using System;
using System.Collections;
using System.ComponentModel;
using CSLA.Resources;

namespace CSLA.Core 
{
  /// <summary>
  /// This is a base class that exposes an implementation
  /// of IBindableList that does nothing other than
  /// create a nonserialized version of the listchanged
  /// event.
  /// </summary>
  [Serializable]
  public abstract class BindableCollectionBase : CollectionBase, IBindingList 
  {

		#region Protected control variables

    /// <summary>
    /// Set this to True to allow data binding to add new
    /// child objects to the collection.
    /// </summary>
    /// <remarks>
    /// If you set this to True, you must also override the OnAddNew
    /// method. You must also set AllowEdit to True.
    /// </remarks>
    protected bool AllowNew = false;
    /// <summary>
    /// Set this to True to allow data binding to do in-place
    /// editing of child objects in a grid control.
    /// </summary>
    protected bool AllowEdit = false;
    /// <summary>
    /// Set this to True to allow data binding to automatically
    /// remove child objects from the collection.
    /// </summary>
    protected bool AllowRemove = false;
    /// <summary>
    /// Set this to True to allow this collection to be sorted.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There is an overhead cost to enabling sorting. Specifically,
    /// the collection must contain an internal collection containing
    /// the original order of the items in the collection, so the order
    /// can be reset if the sort is removed.
    /// </para><para>
    /// This overhead is only incurred if AllowSort is set to True, and is
    /// only a major concern if you are using a remote DataPortal. The concern
    /// there is that this extra collection must also be serialized, thus
    /// increasing the overall amount of data sent across the wire.
    /// </para>
    /// </remarks>
    protected bool AllowSort = false;
    /// <summary>
    /// Set this to True to allow this collection to be
    /// searched.
    /// </summary>
    protected bool AllowFind = false;

		#endregion

		#region ListChanged event

    [NonSerialized]
    ListChangedEventHandler _nonSerializableHandlers;
    ListChangedEventHandler _serializableHandlers;
    
    /// <summary>
    /// Declares a serialization-safe ListChanged event.
    /// </summary>
    public event ListChangedEventHandler ListChanged
    {
      add
      {
        if (value.Target != null)
        {
          if (value.Target.GetType().IsSerializable)
            _serializableHandlers = (ListChangedEventHandler)Delegate.Combine(_serializableHandlers, value);
          else
            _nonSerializableHandlers = (ListChangedEventHandler)Delegate.Combine(_nonSerializableHandlers, value);
        }
      }
      remove
      {
        if (value.Target != null)
        {
          if (value.Target.GetType().IsSerializable)
            _serializableHandlers = (ListChangedEventHandler)Delegate.Remove(_serializableHandlers, value);
          else
            _nonSerializableHandlers = (ListChangedEventHandler)Delegate.Remove(_nonSerializableHandlers, value);
        }
      }
    }
    
    /// <summary>
    /// Call this method to raise the ListChanged event.
    /// </summary>
    virtual protected void OnListChanged(System.ComponentModel.ListChangedEventArgs e)
    {
      if (_nonSerializableHandlers != null)
        _nonSerializableHandlers(this, e);
      if (_serializableHandlers != null)
        _serializableHandlers(this, e);
    }

		#endregion

		#region Collection events

    // *******************************************************************
    /// <summary>
    /// Ensures that the OnListChanged event is raised when a
    /// new child is inserted.
    /// </summary>
    override protected void OnInsertComplete(int index, object value)
    {
      OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
    }

    /// <summary>
    /// Ensures that the OnListChanged event is raised when the
    /// list is cleared.
    /// </summary>
    override protected void OnClearComplete()
    {
      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
    }

    /// <summary>
    /// Ensures that the OnListChanged event is raised when an
    /// item is removed.
    /// </summary>
    override protected void OnRemoveComplete(int index, object value)
    {
      OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
    }

    /// <summary>
    /// Ensures that the OnListChanged event is raised when an
    /// item is changed.
    /// </summary>
    override protected void OnSetComplete(int index, object oldValue, object newValue)
    {
      OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
    }

		#endregion

		#region IBindingList interface

    // *******************************************************************
    // This is most of the IBindingList interface.
    // Notice that each of these implementations merely
    // calls a virtual method, so subclasses can override those
    // methods and provide the actual implementation of the interface

    object IBindingList.AddNew() 
    { 
      if(AllowNew)
        return OnAddNew(); 
      else
        throw new InvalidOperationException(Strings.GetResourceString("AddItemException"));
    }

    bool IBindingList.AllowEdit { get { return AllowEdit; } }
    bool IBindingList.AllowNew { get { return AllowNew; } }
    bool IBindingList.AllowRemove { get { return AllowRemove; } }
    bool IBindingList.SupportsSearching { get { return AllowFind; } }
    bool IBindingList.SupportsSorting { get { return AllowSort; } }

    bool IBindingList.SupportsChangeNotification { get { return true; } }

    int IBindingList.Find(System.ComponentModel.PropertyDescriptor property, object key) 
    { 
      return IBindingList_Find(property, key);
    }

    void IBindingList.AddIndex(System.ComponentModel.PropertyDescriptor property) {}
    void IBindingList.RemoveIndex(System.ComponentModel.PropertyDescriptor property) {}

    void IBindingList.ApplySort(
      System.ComponentModel.PropertyDescriptor property, 
      System.ComponentModel.ListSortDirection direction) 
    {
      IBindingList_ApplySort(property, direction);
    }
    void IBindingList.RemoveSort() 
    {
      IBindingList_RemoveSort();
    }
    bool IBindingList.IsSorted { get { return IBindingList_IsSorted; } }
    System.ComponentModel.ListSortDirection IBindingList.SortDirection { 
      get { return IBindingList_SortDirection; } }
    System.ComponentModel.PropertyDescriptor IBindingList.SortProperty { 
      get { return IBindingList_SortProperty; } }

		#endregion

		#region OnAddNew

    // *******************************************************************
    // The following methods allow a subclass to actually provide
    // the implementation of adding a new child object

    /// <summary>
    /// Override this method to allow data binding to automatically
    /// add new child objects to a collection.
    /// </summary>
    /// <returns></returns>
    virtual protected object OnAddNew() { return null; }

		#endregion

		#region Search/Find

    // *******************************************************************
    // The following methods allow a subclass to actually provide
    // the implementation of IBindingList searching

    /// <summary>
    /// Override this method to implement search/find functionality
    /// for the collection.
    /// </summary>
    /// <param name="property">The property to search.</param>
    /// <param name="key">The value to searched for.</param>
    /// <returns></returns>
    protected virtual int IBindingList_Find(PropertyDescriptor property, object key) 
    {
      return -1;
    }

		#endregion

		#region Sorting

    // *******************************************************************
    // The following methods allow a subclass to actually provide
    // the implementation of IBindingList sorting

    /// <summary>
    /// Override this method to indicate whether your collection
    /// is currently sorted. This returns False by default.
    /// </summary>
    protected virtual bool IBindingList_IsSorted 
    {	get{		return false;}}

    /// <summary>
    /// Override this method to return the property by which
    /// the collection is sorted (if you implement sorting).
    /// </summary>
    protected virtual System.ComponentModel.PropertyDescriptor IBindingList_SortProperty 
    {	get{		return null;}}

    /// <summary>
    /// Override this method to return the current sort direction
    /// (if you implement sorting).
    /// </summary>
    protected virtual ListSortDirection IBindingList_SortDirection 
    {	get{		return ListSortDirection.Ascending;}}

    /// <summary>
    /// Override this method to provide sorting functionality
    /// (if you implement sorting).
    /// </summary>
    /// <param name="property">The property on which to sort.</param>
    /// <param name="direction">The sort direction.</param>
    protected virtual void 
      IBindingList_ApplySort(
        PropertyDescriptor property, ListSortDirection direction) {}

    /// <summary>
    /// Override this method to remove any existing sort
    /// (if you implement sorting).
    /// </summary>
    protected virtual void IBindingList_RemoveSort() {}

		#endregion

  }
}
