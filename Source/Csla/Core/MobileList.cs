//-----------------------------------------------------------------------
// <copyright file="MobileList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a list that is serializable using</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization.Mobile;
using serialization = System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using Csla.Reflection;

namespace Csla.Core
{
  /// <summary>
  /// Implements a list that is serializable using
  /// the SerializationFormatterFactory.GetFormatter().
  /// </summary>
  /// <typeparam name="T">
  /// Type of object contained in the list.
  /// </typeparam>
  [Serializable]
  public class MobileList<T> : List<T>, IMobileObject
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public MobileList() : base() { }
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="capacity">Capacity of the list.</param>
    public MobileList(int capacity) : base(capacity) { }
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="collection">Data to add to list.</param>
    public MobileList(IEnumerable<T> collection) : base(collection) { }

    #region IMobileObject Members

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnGetChildren(info, formatter);
    }

    void IMobileObject.GetState(SerializationInfo info)
    {
      OnGetState(info);
    }

    /// <summary>
    /// Override this method to add extra field values to
    /// the serialization stream.
    /// </summary>
    /// <param name="info">Object containing field values.</param>
    protected virtual void OnGetState(SerializationInfo info)
    {
    }

    private const string _valuePrefix = "v";

    /// <summary>
    /// Override this method to manually serialize child objects
    /// contained within the current object.
    /// </summary>
    /// <param name="info">Object containing serialized values.</param>
    /// <param name="formatter">Reference to the current SerializationFormatterFactory.GetFormatter().</param>
    protected virtual void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      bool mobileChildren = typeof(IMobileObject).IsAssignableFrom(typeof(T));
      int count = 0;
      foreach (T child in this)
      {
        if (mobileChildren)
        {
          SerializationInfo si = formatter.SerializeObject(child);
          info.AddChild(_valuePrefix + count, si.ReferenceId);
        }
        else
        {
          info.AddValue(_valuePrefix + count, child);
        }
        count++;
      }
      info.AddValue("count", count);
    }

    void IMobileObject.SetState(SerializationInfo info)
    {
      OnSetState(info);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnSetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to retrieve extra field values to
    /// the serialization stream.
    /// </summary>
    /// <param name="info">Object containing field values.</param>
    protected virtual void OnSetState(SerializationInfo info) { }

    /// <summary>
    /// Override this method to manually deserialize child objects
    /// from data in the serialization stream.
    /// </summary>
    /// <param name="info">Object containing serialized values.</param>
    /// <param name="formatter">Reference to the current SerializationFormatterFactory.GetFormatter().</param>
    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      bool mobileChildren = typeof(IMobileObject).IsAssignableFrom(typeof(T));
      int count = info.GetValue<int>("count");

      for (int index = 0; index < count; index++)
      {
        T value;
        if (mobileChildren)
          value = (T)formatter.GetObject(info.Children[_valuePrefix + index].ReferenceId);
        else
          value = info.GetValue<T>(_valuePrefix + index);

        Add(value);
      }
    }

    #endregion
  }
}