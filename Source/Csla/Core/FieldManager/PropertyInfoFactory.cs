//-----------------------------------------------------------------------
// <copyright file="PropertyInfoFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates the factory object that</summary>
//-----------------------------------------------------------------------
using System;

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
    public static Type FactoryType { get; internal set; } = typeof(DefaultPropertyInfoFactory);

    private static IPropertyInfoFactory _factory;

    /// <summary>
    /// Gets the factory object that
    /// creates PropertyInfo objects.
    /// </summary>
    public static IPropertyInfoFactory Factory
    {
      get
      {
        if (_factory == null)
            _factory = (IPropertyInfoFactory)Activator.CreateInstance(FactoryType);
        return _factory;
      }
    }
  }
}