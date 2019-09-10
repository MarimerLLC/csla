//-----------------------------------------------------------------------
// <copyright file="ObjectAuthorizationRulesAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Authorization method identifiation attribute</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// Attribute identifying static method invoked
  /// to add object authorization rules for type.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class ObjectAuthorizationRulesAttribute : Attribute 
  {
  }
}
