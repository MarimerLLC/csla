using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core.FieldManager;
using Csla.Core;

namespace Csla.Core
{
  /// <summary>
  /// Inherit from this base class to easily
  /// create a serializable class.
  /// </summary>
  [Serializable]
  public abstract class MobileObject : IMobileObject
  {
    #region Serialize

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
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
      OnSetState(info);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnSetChildren(info, formatter);
    }

    protected virtual void OnSetState(SerializationInfo info) { }

    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter) { }
    
    #endregion
  }
}
