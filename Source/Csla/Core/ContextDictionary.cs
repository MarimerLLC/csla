//-----------------------------------------------------------------------
// <copyright file="ContextDictionary.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Dictionary type that is serializable</summary>
//-----------------------------------------------------------------------

using Csla.Properties;
using Csla.Serialization.Mobile;
using System.Collections.Concurrent;

namespace Csla.Core
{
  /// <summary>
  /// Dictionary type that is serializable
  /// with the SerializationFormatterFactory.GetFormatter().
  /// </summary>
  [Serializable]
  public class ContextDictionary : ConcurrentDictionary<object, object?>, IContextDictionary
  {
    /// <inheritdoc cref="Csla.Core.IContextDictionary.GetValueOrNull(string)"/>
    public object? GetValueOrNull(string key)
    {
      TryGetValue(key, out var result);
      return result;
    }

    #region IMobileObject Members

    void IMobileObject.GetState(SerializationInfo info)
    {
      foreach (string key in Keys)
      {
        object? value = this[key];
        if (value is not IMobileObject)
          info.AddValue(key, value);
      }
    }

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (string key in Keys)
      {
        object? value = this[key];
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
    public bool IsReadOnly => false;

    /// <inheritdoc cref="System.Collections.IDictionary.IsFixedSize"/>
    public bool IsFixedSize => false;

    /// <inheritdoc cref="System.Collections.IDictionary.Add(object, object?)"/>
    public void Add(object key, object? value)
    {
      bool added = TryAdd(key, value);
      if (!added)
      {
        throw new ArgumentException(Resources.KeyAlreadyExistsException);
      }
    }

    /// <inheritdoc cref="System.Collections.IDictionary.Remove(object)"/>
    public void Remove(object key)
    {
      var removed = TryRemove(key, out var _);
      if (!removed)
      {
        throw new NotSupportedException(Resources.KeyDoesNotExistException);
      }
    }

    #endregion

    #region ICollection Members

    /// <inheritdoc cref="System.Collections.ICollection.SyncRoot"/>
    public object SyncRoot => throw new NotSupportedException(Resources.SyncrootNotSupportedException);

    /// <inheritdoc cref="System.Collections.ICollection.IsSynchronized"/>
    public bool IsSynchronized => false;

    #endregion
  }
}