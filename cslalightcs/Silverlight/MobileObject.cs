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

namespace Csla.Silverlight
{
  /// <summary>
  /// Inherit from this base class to easily
  /// create a serializable class.
  /// </summary>
  [Serializable]
  public abstract class MobileObject : IMobileObject
  {
    #region Field Manager

    private FieldDataManager _fieldManager;

    protected FieldDataManager FieldManager
    {
      get
      {
        if (_fieldManager == null)
        {
          _fieldManager = new FieldDataManager(this.GetType());
        }
        return _fieldManager;
      }
    }

    protected T GetProperty<T>(IPropertyInfo propertyInfo)
    {
      IFieldData data = FieldManager.GetFieldData(propertyInfo);
      return (T)(data != null ? data.Value : null);
    }

    protected void SetProperty<T>(IPropertyInfo propertyInfo, T value)
    {
      FieldManager.SetFieldData<T>(propertyInfo, value);
    }

    #endregion

    #region Serialize

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      SerializationInfo fieldManagerInfo = formatter.SerializeObject(_fieldManager);
      info.AddChild("_fieldManager", fieldManagerInfo.ReferenceId);

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
      SerializationInfo.ChildData childData = info.Children["_fieldManager"];
      _fieldManager = (FieldDataManager)formatter.GetObject(childData.ReferenceId);

      OnSetChildren(info, formatter);
    }

    protected virtual void OnSetState(SerializationInfo info) { }

    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter) { }
    
    #endregion
  }
}
