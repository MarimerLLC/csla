using System;
using Csla.Serialization;
using Csla.Core;
using Csla.Core.FieldManager;

namespace Csla
{
  [Serializable]
  public class ReadOnlyBase<T> : Csla.Core.MobileObject
    where T : ReadOnlyBase<T>
  {
    #region FieldManager

    private FieldDataManager _fieldManager;

    public static PropertyInfo<P> RegisterProperty<P>(Type ownerType, PropertyInfo<P> property)
    {
      return PropertyInfoManager.RegisterProperty<P>(ownerType, property);
    }

    protected static PropertyInfo<P> RegisterProperty<P>(PropertyInfo<P> info)
    {
      return Core.FieldManager.PropertyInfoManager.RegisterProperty<P>(typeof(T), info);
    }

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

    protected P GetProperty<P>(IPropertyInfo propertyInfo)
    {
      IFieldData data = FieldManager.GetFieldData(propertyInfo);
      return (P)(data != null ? data.Value : null);
    }

    protected void SetProperty<P>(IPropertyInfo propertyInfo, P value)
    {
      FieldManager.SetFieldData<P>(propertyInfo, value);
    }

    protected void LoadProperty(IPropertyInfo propertyInfo, object newValue)
    {
      FieldManager.LoadFieldData(propertyInfo, newValue);
    }

    protected object ReadProperty(IPropertyInfo propertyInfo)
    {
      var info = FieldManager.GetFieldData(propertyInfo);
      if (info != null)
        return info.Value;
      else
        return null;
    }

    #endregion

    #region MobileFormatter

    protected override void OnGetChildren(
      Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      base.OnGetChildren(info, formatter);
      var fieldManagerInfo = formatter.SerializeObject(_fieldManager);
      info.AddChild("_fieldManager", fieldManagerInfo.ReferenceId);
    }

    protected override void OnSetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      var childData = info.Children["_fieldManager"];
      _fieldManager = (FieldDataManager)formatter.GetObject(childData.ReferenceId);
      base.OnSetChildren(info, formatter);
    }

    #endregion

  }
}
