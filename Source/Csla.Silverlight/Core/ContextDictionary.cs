//-----------------------------------------------------------------------
// <copyright file="ContextDictionary.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines a counterpart to the .NET HybridDictionary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla.Serialization.Mobile;
using Csla.Serialization;

namespace Csla.Core
{
  /// <summary>
  /// Defines a counterpart to the .NET HybridDictionary
  /// that can be serialized by the MobileFormatter.
  /// </summary>
  [Serializable()]
  public class ContextDictionary : Dictionary<string, object>, IMobileObject
  {
    /// <summary>
    /// Gets a value indicating whether the
    /// dictionary contains a matching key value.
    /// </summary>
    /// <param name="key">Key value.</param>
    public bool Contains(string key)
    {
      return ContainsKey(key);
    }

    #region IMobileObject Members

    void IMobileObject.GetState(SerializationInfo info)
    {
      foreach (string key in this.Keys)
      {
        object value = this[key];
        if(!(value is IMobileObject))
          info.AddValue(key, value);
      }
    }

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (string key in this.Keys)
      {
        object value = this[key];
        IMobileObject mobile = value as IMobileObject;
        if (mobile != null)
        {
          SerializationInfo si = formatter.SerializeObject(mobile);
          info.AddChild(key, si.ReferenceId);
        }
      }
    }

    void IMobileObject.SetState(SerializationInfo info)
    {
      foreach (string key in info.Values.Keys)
      {
        Add(key, info.Values[key].Value);
      }
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      foreach (string key in info.Children.Keys)
      {
        int referenceId = info.Children[key].ReferenceId;
        this.Add(key, formatter.GetObject(referenceId));
      }
    }

    #endregion
  }
}