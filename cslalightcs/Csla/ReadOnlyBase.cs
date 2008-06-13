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

    public static PropertyInfo<T> RegisterProperty<T>(Type ownerType, PropertyInfo<T> property)
    {
      return PropertyInfoManager.RegisterProperty<T>(ownerType, property);
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
  }
}
