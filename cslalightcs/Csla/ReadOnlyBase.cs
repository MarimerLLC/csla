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

    #endregion
  }
}
