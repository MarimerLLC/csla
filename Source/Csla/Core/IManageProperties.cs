//-----------------------------------------------------------------------
// <copyright file="IManageProperties.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  internal interface IManageProperties
  {
    bool HasManagedProperties { get; }
    bool FieldExists(IPropertyInfo property);
    List<IPropertyInfo> GetManagedProperties();
    object GetProperty(IPropertyInfo propertyInfo);
    object LazyGetProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator);
    object LazyGetPropertyAsync<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(PropertyInfo<P> propertyInfo, Task<P> factory);
    object ReadProperty(IPropertyInfo propertyInfo);
    P ReadProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(PropertyInfo<P> propertyInfo);
    P LazyReadProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator);
    P LazyReadPropertyAsync<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(PropertyInfo<P> propertyInfo, Task<P> factory);
    void SetProperty(IPropertyInfo propertyInfo, object newValue);
    void LoadProperty(IPropertyInfo propertyInfo, object newValue);
    bool LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object newValue);
    void LoadProperty<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      P>(PropertyInfo<P> propertyInfo, P newValue);
    List<object> GetChildren();
  }
}