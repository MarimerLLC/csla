using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core.FieldManager;
using Csla.Core;

namespace Csla.Core
{
  public class BindingList<T> : IList<T>, IBindingList
  {
    protected bool AllowEdit { get; set; }
    protected bool AllowNew { get; set; }
    protected bool AllowRemove { get; set; }
    protected bool RaiseListChangedEvents { get; set; }

    private List<T> _list = new List<T>();

    #region IList<T> Members

    public int IndexOf(T item)
    {
      return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      InsertItem(index, item);
    }

    protected virtual void InsertItem(int index, T item)
    {
      _list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      RemoveItem(index);
    }

    protected virtual void RemoveItem(int index)
    {
      _list.RemoveAt(index);
    }

    public T this[int index]
    {
      get
      {
        return _list[index];
      }
      set
      {
        SetItem(index, value);
      }
    }

    protected virtual void SetItem(int index, T item)
    {
      _list[index] = item;
    }


    #endregion

    #region ICollection<T> Members

    public void Add(T item)
    {
      _list.Add(item);
    }

    public object AddNew()
    {
      return AddNewCore();
    }

    protected virtual object AddNewCore()
    {
      T t = (T)Activator.CreateInstance(typeof(T));
      Add(t);
      return t;
    }

    public void Clear()
    {
      ClearItems();
    }

    protected virtual void ClearItems()
    {
      _list.Clear();
    }

    public bool Contains(T item)
    {
      return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _list.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _list.Count; }
    }

    public virtual bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(T item)
    {
      return _list.Remove(item);
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)_list).GetEnumerator();
    }

    #endregion

    #region IBindingList Members

    public event EventHandler<ListChangedEventArgs> ListChanged;

    protected virtual void OnListChanged(ListChangedEventArgs e)
    {
      if (ListChanged != null)
        ListChanged(this, e);
    }

    #endregion
  }
}
