using System;
using System.Collections;
using System.ComponentModel;

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
		/// Declares a serialization-safe ListChanged event.
		/// </summary>
    [field: NonSerialized]
    public event System.ComponentModel.ListChangedEventHandler ListChanged;
    
		/// <summary>
		/// Call this method to raise the ListChanged event.
		/// </summary>
    virtual protected void OnListChanged(System.ComponentModel.ListChangedEventArgs e)
    {
      if (ListChanged != null)
        ListChanged(this, e);
    }

    void IBindingList.AddIndex(System.ComponentModel.PropertyDescriptor property)     {    }
    object IBindingList.AddNew() { return OnAddNew(); }
    void IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor property, System.ComponentModel.ListSortDirection direction) {}
    int IBindingList.Find(System.ComponentModel.PropertyDescriptor property, object key) { return 0 ;}
    void IBindingList.RemoveIndex(System.ComponentModel.PropertyDescriptor property) {}
    void IBindingList.RemoveSort() {}
    bool IBindingList.AllowEdit { get { return AllowEdit; } }
    bool IBindingList.AllowNew { get { return AllowNew; } }
    bool IBindingList.AllowRemove { get { return AllowRemove; } }
    bool IBindingList.IsSorted { get { return false; } }
    System.ComponentModel.ListSortDirection IBindingList.SortDirection { get { return System.ComponentModel.ListSortDirection.Ascending; } }
    System.ComponentModel.PropertyDescriptor IBindingList.SortProperty { get { return null; } }
    bool IBindingList.SupportsChangeNotification { get { return true; } }
    bool IBindingList.SupportsSearching { get { return false; } }
    bool IBindingList.SupportsSorting { get { return false; } }

		/// <summary>
		/// Override this method to allow data binding to automatically
		/// add new child objects to a collection.
		/// </summary>
		/// <returns></returns>
    virtual protected object OnAddNew() { return null; }

		override protected void OnInsertComplete(int index, object value)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}

		override protected void OnClearComplete()
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
		}

		override protected void OnRemoveComplete(int index, object value)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		override protected void OnSetComplete(int index, object oldValue, object newValue)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}

  }
}
