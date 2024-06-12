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
    /// <summary>
    /// Get a value from the dictionary, or return null
    /// if the key is not found in the dictionary.
    /// </summary>
    /// <param name="key">Key of value to get from dictionary.</param>
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

    /// <summary>
    /// Gets a value indicating whether the System.Collections.IDictionary object is
    /// read-only.
    /// </summary>
    ///<returns>true if the System.Collections.IDictionary object is read-only; otherwise, false.</returns>
    public bool IsReadOnly
    {
      get => ((IDictionary)this).IsReadOnly;
    }

    /// <summary>
    /// Gets a value indicating whether the System.Collections.IDictionary object has
    /// a fixed size.
    /// </summary>
    /// <returns>true if the System.Collections.IDictionary object has a fixed size; otherwise, false.</returns>
    public bool IsFixedSize
    {
      get => ((IDictionary)this).IsFixedSize;
    }

    /// <summary>
    /// Adds an element with the provided key and value to the System.Collections.IDictionary
    /// object.
    /// </summary>
    /// <param name="key">The System.Object to use as the key of the element to add.</param>
    /// <param name="value">The System.Object to use as the value of the element to add.</param>
    /// <exception cref="System.ArgumentNullException">key is null.</exception>
    /// <exception cref="System.ArgumentException">An element with the same key already exists in the System.Collections.IDictionary object.</exception>
    /// <exception cref="System.NotSupportedException">The System.Collections.IDictionary is read-only. -or- The System.Collections.IDictionary has a fixed size.</exception>
    public void Add(object key, object value)
    {
      ((IDictionary)this).Add(key, value);
    }

    /// <summary>
    /// Removes the element with the specified key from the System.Collections.IDictionary
    /// object.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <exception cref="System.ArgumentNullException">key is null.</exception>
    /// <exception cref="System.NotSupportedException">The System.Collections.IDictionary object is read-only. -or- The System.Collections.IDictionary has a fixed size.</exception>
    public void Remove(object key)
    {
      ((IDictionary)this).Remove(key);
    }

    #endregion

    #region ICollection Members

    /// <summary>
    /// Gets an object that can be used to synchronize access to the System.Collections.ICollection.
    /// </summary>
    ///<returns>An object that can be used to synchronize access to the System.Collections.ICollection.</returns>
    public object SyncRoot
    {
      get => ((IDictionary)this).SyncRoot;
    }

    /// <summary>
    /// Gets a value indicating whether access to the System.Collections.ICollection
    /// is synchronized (thread safe).
    /// </summary>
    ///<returns>true if access to the System.Collections.ICollection is synchronized (thread safe); otherwise, false.</returns>
    public bool IsSynchronized
    {
      get => ((ICollection)this).IsSynchronized;
    }

    /// <summary>
    /// Copies the elements of the System.Collections.ICollection to an System.Array,
    /// starting at a particular System.Array index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional System.Array that is the destination of the elements copied
    /// from System.Collections.ICollection. The System.Array must have zero-based indexing.
    /// </param>
    /// <param name="index">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">index is less than zero.</exception>
    /// <exception cref="System.ArgumentException">
    /// array is multidimensional. -or- The number of elements in the source System.Collections.ICollection
    /// is greater than the available space from index to the end of the destination
    /// array. -or- The type of the source System.Collections.ICollection cannot be cast
    /// automatically to the type of the destination array.
    /// </exception>
    public void CopyTo(Array array, int index) 
    {
      ((ICollection)this).CopyTo(array, index);
    }

    #endregion
  }
}