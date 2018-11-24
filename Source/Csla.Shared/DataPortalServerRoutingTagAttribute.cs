//-----------------------------------------------------------------------
// <copyright file="DataPortalServerRoutingTagAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Specifies a routing tag for use by the server-side data portal.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// Specifies a routing tag for use by the server-side data portal.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public class DataPortalServerRoutingTagAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets the routing tag
    /// </summary>
    public string RoutingTag { get; set; }

    /// <summary>
    /// Creates an instance of this attribute.
    /// </summary>
    /// <param name="tag">Routing tag value</param>
    public DataPortalServerRoutingTagAttribute(string tag)
    {
      RoutingTag = tag;
    }
  }
}
