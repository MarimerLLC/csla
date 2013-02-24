﻿#if !__ANDROID__ && !IOS
//-----------------------------------------------------------------------
// <copyright file="NonSerializedAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Indicates that a field should not be</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Serialization
{
  /// <summary>
  /// Indicates that a field should not be
  /// serialized by the MobileFormatter.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public class NonSerializedAttribute : Attribute
  {
  }
}
#endif
