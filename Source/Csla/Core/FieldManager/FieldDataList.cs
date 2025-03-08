//-----------------------------------------------------------------------
// <copyright file="FieldDataList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Csla.Core.FieldManager
{
  [Serializable]
#if ANDROID || IOS
  internal class FieldDataList : Csla.Core.MobileObject, Csla.Serialization.Mobile.ISerializationNotification
#else
  internal class FieldDataList : ISerializable
#endif
  {
    [NonSerialized]
    private readonly Dictionary<string, int> _fieldIndex = [];
#if ANDROID || IOS
    private Csla.Core.MobileBindingList<IFieldData> _fields = new Csla.Core.MobileBindingList<IFieldData>();
#else
    private List<IFieldData> _fields = [];
#endif

    public FieldDataList()
    { /* required due to serialization ctor */ }

    public bool TryGetValue(string key, [NotNullWhen(true)] out IFieldData? result)
    {
      if (key is null)
        throw new ArgumentNullException(nameof(key));

      if (_fieldIndex.TryGetValue(key, out var index))
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
      if (key is null)
        throw new ArgumentNullException(nameof(key));

      return _fieldIndex.ContainsKey(key);
    }

    public IFieldData GetValue(string key)
    {
      if (key is null)
        throw new ArgumentNullException(nameof(key));

      return _fields[_fieldIndex[key]];
    }

    public void Add(string key, IFieldData value)
    {
      if (key is null)
        throw new ArgumentNullException(nameof(key));

      _fields.Add(value);
      _fieldIndex.Add(key, _fields.Count - 1);
    }

    internal string? FindPropertyName(object value)
    {
      foreach (var item in _fields)
        if (ReferenceEquals(item.Value, value))
          return item.Name;
      return null;
    }

#if ANDROID || IOS
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

#if ANDROID || IOS
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

    protected FieldDataList(SerializationInfo info, StreamingContext context)
    {
      _fields = (List<IFieldData>)info.GetValue("Fields", typeof(List<IFieldData>))!;
      RebuildIndex();
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
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