using System;
using System.Collections.Generic;

namespace Csla.Core
{
  internal interface IManageProperties
  {
    bool HasManagedProperties { get; }
    List<IPropertyInfo> GetManagedProperties();
    object GetProperty(IPropertyInfo propertyInfo);
    object ReadProperty(IPropertyInfo propertyInfo);
    void SetProperty(IPropertyInfo propertyInfo, object newValue);
    void LoadProperty(IPropertyInfo propertyInfo, object newValue);
    void LoadProperty<P>(PropertyInfo<P> propertyInfo, P newValue);
  }
}
