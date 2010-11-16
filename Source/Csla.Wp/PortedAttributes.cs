//-----------------------------------------------------------------------
// <copyright file="PortedAttributes.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Dummy implementations of .NET attributes missing in WP7.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// Mock attribute ported from .NET/SL.
  /// </summary>
  public class BrowsableAttribute : Attribute
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="flag">Flag value.</param>
    public BrowsableAttribute(bool flag)
    { }
  }
}
