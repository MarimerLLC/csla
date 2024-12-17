//-----------------------------------------------------------------------
// <copyright file="NonSerializedPropertyAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicate that a field or property should be excluded from auto serialization</summary>
//-----------------------------------------------------------------------

namespace Csla
{
  /// <summary>
  /// Indicate that a public field or property should be excluded from auto serialization
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class NonSerializedPropertyAttribute : Attribute;
}