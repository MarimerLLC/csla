#if !NETSTANDARD1_6 && !WINDOWS_UWP
#if NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="NonSerializedAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicates that a field should not be</summary>
//-----------------------------------------------------------------------
using System;

namespace System
{
  /// <summary>
  /// Indicates that a field should not be
  /// serialized by the SerializationFormatterFactory.GetFormatter().
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public class NonSerializedAttribute : Attribute
  {
  }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.NonSerializedAttribute))]
#endif
#endif