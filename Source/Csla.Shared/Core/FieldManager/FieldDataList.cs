//-----------------------------------------------------------------------
// <copyright file="FieldDataList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Csla.Core.FieldManager
{
  [Serializable]
#if (ANDROID || IOS) || NETFX_CORE
  internal class FieldDataList : Csla.Core.MobileObject, Csla.Serialization.Mobile.ISerializationNotification
#else
  internal class FieldDataList : ISerializable
#endif
  {
    [NonSerialized()]
    private Dictionary<string, int> _fieldIndex = new Dictionary<string, int>();
#if (ANDROID || IOS) || NETFX_CORE
    private Csla.Core.MobileBindingList<IFieldData> _fields = new Csla.Core.MobileBindingList<IFieldData>();
#else
    private List<IFieldData> _fields = new List<IFieldData>();
#endif

    public FieldDataList()
    { /* required due to serialization ctor */ }

    public bool TryGetValue(string key, out IFieldData result)
    {
      int index;
      if (_fieldIndex.TryGetValue(key, out index))
      {
        result = _fields[index];
        return true;
      }
      else
      {
        result = null;
        return false;
      }
    }

    public bool ContainsKey(string key)
    {
      return _fieldIndex.ContainsKey(key);
    }

    public IFieldData GetValue(string key)
    {
      return _fields[_fieldIndex[key]];
    }

    public void Add(string key, IFieldData value)
    {
      _fields.Add(value);
      _fieldIndex.Add(key, _fields.Count - 1);
    }

    internal string FindPropertyName(object value)
    {
      foreach (var item in _fields)
        if (ReferenceEquals(item.Value, value))
          return item.Name;
      return null;
    }

#if (ANDROID || IOS) || NETFX_CORE
    public Csla.Core.MobileBindingList<IFieldData> GetFieldDataList()
    {
      return _fields;
    }
#else
    public List<IFieldData> GetFieldDataList()
    {
      return _fields;
    }
#endif

#if (ANDROID || IOS) || NETFX_CORE
    #region ISerializationNotification Members

    void Csla.Serialization.Mobile.ISerializationNotification.Deserialized()
    {
      RebuildIndex();
    }

        private void RebuildIndex()
    {
      var position = 0;
      foreach (IFieldData item in _fields)
      {
        _fieldIndex.Add(item.Name, position);
        position += 1;
      }
    }

    #endregion
#else
    #region  ISerializable

    protected FieldDataList(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      _fields = (List<IFieldData>)(info.GetValue("Fields", typeof(List<IFieldData>)));
      RebuildIndex();
    }

    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      info.AddValue("Fields", _fields);
    }

    private void RebuildIndex()
    {
      var position = 0;
      foreach (IFieldData item in _fields)
      {
        _fieldIndex.Add(item.Name, position);
        position += 1;
      }
    }

    #endregion
#endif
  }
}