#if (!__ANDROID__ && !IOS) && NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Contains extension methods for Type.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Serialization
{
  /// <summary>
  /// Contains extension methods for Type.
  /// </summary>
  public static class TypeExtensions
  {
    /// <summary>
    /// Gets a value indicating whether this
    /// type is marked as Serializable.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if the type is Serializable.</returns>
    public static bool IsSerializable(this Type type)
    {
      var result = type.GetCustomAttributes(typeof(SerializableAttribute), false);
      return (result != null && result.Length > 0);
    }
  }
}
#endif