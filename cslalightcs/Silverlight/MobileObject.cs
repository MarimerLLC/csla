using System;
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

    public T GetProperty<T>(IPropertyInfo propertyInfo)
    {
      return (T)FieldManager.GetFieldData(propertyInfo).Value;
    }

    public void SetProperty<T>(IPropertyInfo propertyInfo, T value)
    {
      FieldManager.SetFieldData<T>(propertyInfo, value);
    }

    #endregion

    #region Serialize

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (_fieldManager != null)
      {
        FieldManager.GetRegisteredProperties().ForEach(p =>
        {
          if (typeof(IMobileObject).IsAssignableFrom(p.Type))
          {
            IFieldData data = FieldManager.GetFieldData(p);
            SerializationInfo childInfo = formatter.SerializeObject(data.Value);
            info.AddChild(p.Name, childInfo.ReferenceId);
          }
        });
      }
      OnGetChildren(info, formatter);
    }

    void IMobileObject.GetState(SerializationInfo info)
    {
      if (_fieldManager != null)
      {
        FieldManager.GetRegisteredProperties().ForEach(p =>
        {
          if (!typeof(IMobileObject).IsAssignableFrom(p.Type))
          {
            IFieldData data = FieldManager.GetFieldData(p);
            info.AddValue(data.Name, data.Value);
          }
        });
      }
      OnGetState(info);
    }

    protected virtual void OnGetState(SerializationInfo info) { }

    protected virtual void OnGetChildren(SerializationInfo info, MobileFormatter formatter) { }

    #endregion

    #region Deserialize

    void IMobileObject.SetState(SerializationInfo info)
    {
      if (info.Values != null && info.Values.Count > 0)
      {
        var properties = FieldManager.GetRegisteredProperties();
        foreach (var data in info.Values)
        {
          var property = properties.First(p => p.Name == data.Key);
          FieldManager.LoadFieldData(property, (object)data.Value);
        }
      }
      OnSetState(info);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
    }

    protected virtual void OnSetState(SerializationInfo info) { }

    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter) { }
    
    #endregion
  }
}
