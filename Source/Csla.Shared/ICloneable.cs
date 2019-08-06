#if NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="ICloneable.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a cloneable object.</summary>
//-----------------------------------------------------------------------
namespace System
{
  /// <summary>
  /// Defines a cloneable object.
  /// </summary>
  public interface ICloneable
  {
    /// <summary>
    /// Gets a clone of the object.
    /// </summary>
    object Clone();
  }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ICloneable))]
#endif