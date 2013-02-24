﻿#if !__ANDROID__ && !IOS
//-----------------------------------------------------------------------
// <copyright file="SerializationAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Indicates that an object may be</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Serialization
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