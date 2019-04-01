//-----------------------------------------------------------------------
// <copyright file="BrowsableAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Placeholder for missing Browsable attribute from full .NET.</summary>
//-----------------------------------------------------------------------
#if NETFX_CORE && !NETSTANDARD
using System;

namespace System.ComponentModel
{
  /// <summary>
  /// Placeholder for missing Browsable attribute from full .NET.
  /// </summary>
  public class BrowsableAttribute : Attribute
  {
    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="value"></param>
    public BrowsableAttribute(bool value)
    {
    }
  }

  /// <summary>
  /// Placeholder for missing Category attribute from full .NET.
  /// </summary>
  public class CategoryAttribute : Attribute
  {
    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="value"></param>
    public CategoryAttribute(string value)
    {
    }
  }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ComponentModel.BrowsableAttribute))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ComponentModel.CategoryAttribute))]
#endif
