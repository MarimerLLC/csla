#if NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="ICloneable.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
#endif