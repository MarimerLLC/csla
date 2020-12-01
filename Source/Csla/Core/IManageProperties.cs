//-----------------------------------------------------------------------
// <copyright file="IManageProperties.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Threading.Tasks;

namespace Csla.Core
{
  internal interface IManageProperties
  {
    bool HasManagedProperties { get; }
    bool FieldExists(Csla.Core.IPropertyInfo property);
    List<IPropertyInfo> GetManagedProperties();
    object GetProperty(IPropertyInfo propertyInfo);
    object LazyGetProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator);
    object LazyGetPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory);
    object ReadProperty(IPropertyInfo propertyInfo);
    P ReadProperty<P>(PropertyInfo<P> propertyInfo);
    P LazyReadProperty<P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator);
    P LazyReadPropertyAsync<P>(PropertyInfo<P> propertyInfo, Task<P> factory);
    void SetProperty(IPropertyInfo propertyInfo, object newValue);
    void LoadProperty(IPropertyInfo propertyInfo, object newValue);
    bool LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object newValue);
    void LoadProperty<P>(PropertyInfo<P> propertyInfo, P newValue);
    List<object> GetChildren();
  }
}