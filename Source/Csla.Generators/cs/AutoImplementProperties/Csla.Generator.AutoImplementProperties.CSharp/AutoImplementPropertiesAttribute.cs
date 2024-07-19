//-----------------------------------------------------------------------
// <copyright file="AutoImplementPropertiesAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicate that a type should auto implement properties</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization
{
  /// <summary>
  /// Indicate that a type should be auto serialized
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public class AutoImplementPropertiesAttribute : Attribute;
}