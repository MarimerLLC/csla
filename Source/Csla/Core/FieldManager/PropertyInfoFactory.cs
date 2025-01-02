//-----------------------------------------------------------------------
// <copyright file="PropertyInfoFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates the factory object that</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Creates the factory object that
  /// creates PropertyInfo objects.
  /// </summary>
  public static class PropertyInfoFactory
  {
    /// <summary>
    /// Gets the PropertyInfoFactory type.
    /// </summary>
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
    public static Type FactoryType { get; internal set; } = typeof(DefaultPropertyInfoFactory);

    private static IPropertyInfoFactory? _factory;

    /// <summary>
    /// Gets the factory object that
    /// creates PropertyInfo objects.
    /// </summary>
    public static IPropertyInfoFactory Factory
    {
      get
      {
        _factory ??= (IPropertyInfoFactory)Activator.CreateInstance(FactoryType)!;
        return _factory;
      }
    }
  }
}