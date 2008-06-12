using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core.FieldManager;
using Csla.Core;

namespace Csla.Silverlight
{
  /// <summary>
  /// Inherit from this base class to easily
  /// create a serializable list class.
  /// </summary>
  /// <typeparam name="T">
  /// Type of the items contained in the list.
  /// </typeparam>
  [Serializable]
  public class MobileList<T> : IMobileObject, IList<T>
  {
    #region Field Manager

    private FieldDataManager _fieldManager;

    protected FieldDataManager FieldManager
    {
      get
      {
        if (_fieldManager == null)
        {
          _fieldManager = new FieldDataManager(this.GetType());
        }
        return _fieldManager;
      }
    }

    protected T GetProperty<T>(IPropertyInfo propertyInfo)
    {
      IFieldData data = FieldManager.GetFieldData(propertyInfo);
      return (T)(data != null ? data.Value : null);
    }

    protected void SetProperty<T>(IPropertyInfo propertyInfo, T value)
    {
      FieldManager.SetFieldData<T>(propertyInfo, value);
    }

    #endregion

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

    #region IMobileObject Members

    public void GetState(SerializationInfo info)
    {
      throw new NotImplementedException();
    }

    public void GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      throw new NotImplementedException();
    }

    public void SetState(SerializationInfo info)
    {
      throw new NotImplementedException();
    }

    public void SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Serialize

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (_fieldManager != null)
      {
        SerializationInfo fieldManagerInfo = formatter.SerializeObject(_fieldManager);
        info.AddChild("_fieldManager", fieldManagerInfo.ReferenceId);
      }

      List<int> references = new List<int>();
      for(int x=0;x<this.Count;x++)
      {
        T child = this[x];
        if (child != null)
        {
          SerializationInfo childInfo = formatter.SerializeObject(child);
          references.Add(childInfo.ReferenceId);
        }
      }
      if (references.Count > 0)
        info.AddValue("$list", references);

      OnGetChildren(info, formatter);
    }

    void IMobileObject.GetState(SerializationInfo info)
    {
      OnGetState(info);
    }

    protected virtual void OnGetState(SerializationInfo info) { }

    protected virtual void OnGetChildren(SerializationInfo info, MobileFormatter formatter) { }

    #endregion

    #region Deserialize

    void IMobileObject.SetState(SerializationInfo info)
    {
      if(info.Values.ContainsKey("_fieldManager"))
        _fieldManager = (FieldDataManager)info.Values["_fieldManager"].Value;

      // TODO: set field values for MobileList

      OnSetState(info);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Values.ContainsKey("$list"))
      {
        List<int> references = (List<int>)info.Values["$list"].Value;
        foreach (int reference in references)
        {
          T child = (T)formatter.GetObject(reference);
          _list.Add(child);
        }
      }

      OnSetChildren(info, formatter);
    }

    protected virtual void OnSetState(SerializationInfo info) { }

    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter) { }

    #endregion
  }
}
