//-----------------------------------------------------------------------
// <copyright file="DataPortalServerRoutingTagAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
    private string _routingTag;

    /// <summary>
    /// Gets or sets the routing tag (can not contain '-').
    /// </summary>
    public string RoutingTag
    {
      get { return _routingTag; }
      set
      {
        if (!string.IsNullOrWhiteSpace(value))
          if (value.Contains("-") || value.Contains("/"))
            throw new ArgumentException("valueRoutingToken");
        _routingTag = value;
      }
    }

    /// <summary>
    /// Creates an instance of this attribute.
    /// </summary>
    /// <param name="tag">Routing tag value (can not contain '-')</param>
    public DataPortalServerRoutingTagAttribute(string tag)
    {
      RoutingTag = tag;
    }
  }
}
