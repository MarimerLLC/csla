//-----------------------------------------------------------------------
// <copyright file="ContextDictionary.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Dictionary type that is serializable</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization.Mobile;
using System.Collections.Specialized;
#if NETFX_CORE
using System.Collections.Generic;
using Csla.Serialization;
#endif

namespace Csla.Core
{
  /// <summary>
  /// Dictionary type that is serializable
  /// with the MobileFormatter.
  /// </summary>
  [Serializable()]
#if NETFX_CORE
  public class ContextDictionary : Dictionary<string, object>, IMobileObject
#else
  public class ContextDictionary : HybridDictionary, IMobileObject
#endif
  {
#if NETFX_CORE
    internal object GetValueOrNull(string key)
    {
      object value;
      if (this.TryGetValue(key, out value))
        return value;
      else
        return null;
    }
#endif

    #region IMobileObject Members

    void IMobileObject.GetState(SerializationInfo info)
    {
      foreach (string key in this.Keys)
      {
        object value = this[key];
        if (!(value is IMobileObject))
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