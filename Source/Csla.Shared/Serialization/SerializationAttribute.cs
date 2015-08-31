#if (!ANDROID && !IOS) && NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="SerializationAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Indicates that an object may be</summary>
//-----------------------------------------------------------------------
using System;

namespace System
{
  /// <summary>
  /// Indicates that an object may be
  /// serialized by the MobileFormatter.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public class SerializableAttribute : Attribute
  {
  }
}
#endif