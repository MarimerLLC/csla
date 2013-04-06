using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Csla.Serialization.Mobile;
using System.Collections.Specialized;

namespace Csla.Core
{
  /// <summary>
  /// Dictionary type that is serializable
  /// with the MobileFormatter.
  /// </summary>
  [Serializable()]
  public class ContextDictionary : HybridDictionary, IMobileObject
  {
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
