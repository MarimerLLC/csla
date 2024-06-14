//-----------------------------------------------------------------------
// <copyright file="ContextDictionary.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Dictionary type that is serializable</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;
using System.Collections;
using System.Collections.Concurrent;

namespace Csla.Core
{
  /// <summary>
  /// Dictionary type that is serializable
  /// with the SerializationFormatterFactory.GetFormatter().
  /// </summary>
  [Serializable]
  public class ContextDictionary : ConcurrentDictionary<object, object>, IContextDictionary
  {
    /// <inheritdoc cref="Csla.Core.IContextDictionary.GetValueOrNull(string)"/>
    public object GetValueOrNull(string key)
    {
      if (ContainsKey(key))
        return this[key];
      return null;
    }

    #region IMobileObject Members

    void IMobileObject.GetState(SerializationInfo info)
    {
      foreach (string key in Keys)
      {
        object value = this[key];
        if (value is not IMobileObject)
          info.AddValue(key, value);
      }
    }

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (string key in Keys)
      {
        object value = this[key];
        if (value is IMobileObject mobile)
        {
          SerializationInfo si = formatter.SerializeObject(mobile);
          info.AddChild(key, si.ReferenceId);
        }
      }
    }

    void IMobileObject.SetState(SerializationInfo info)
    {
      foreach (var item in info.Values)
      {
        Add(item.Key, item.Value.Value);
      }
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (var item in info.Children)
      {
        var referenceId = item.Value.ReferenceId;
        Add(item.Key, formatter.GetObject(referenceId));
      }
    }

    #endregion

    #region IDictionary Members

    /// <inheritdoc cref="System.Collections.IDictionary.IsReadOnly"/>
    public bool IsReadOnly
    {
      get => ((IDictionary)this).IsReadOnly;
    }

    /// <inheritdoc cref="System.Collections.IDictionary.IsFixedSize"/>
    public bool IsFixedSize
    {
      get => ((IDictionary)this).IsFixedSize;
    }

    /// <inheritdoc cref="System.Collections.IDictionary.Add(object, object?)"/>
    public void Add(object key, object value)
    {
      ((IDictionary)this).Add(key, value);
    }

    /// <inheritdoc cref="System.Collections.IDictionary.Remove(object)"/>
    public void Remove(object key)
    {
      ((IDictionary)this).Remove(key);
    }

    #endregion

    #region ICollection Members

    /// <inheritdoc cref="System.Collections.ICollection.SyncRoot"/>
    public object SyncRoot
    {
      get => ((IDictionary)this).SyncRoot;
    }

    /// <inheritdoc cref="System.Collections.ICollection.IsSynchronized"/>
    public bool IsSynchronized
    {
      get => ((ICollection)this).IsSynchronized;
    }

    /// <inheritdoc cref="System.Collections.ICollection.CopyTo(Array, int)"/>
    public void CopyTo(Array array, int index) 
    {
      ((ICollection)this).CopyTo(array, index);
    }

    #endregion
  }
}