//-----------------------------------------------------------------------
// <copyright file="DataPortalServerResourceAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Specifies a server resource required by a business type</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// Specifies a server resource required by a business type
  /// so the data portal can route any calls to the correct
  /// server.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public class DataPortalServerResourceAttribute : Attribute
  {
    /// <summary>
    /// Gets the resource id.
    /// </summary>
    public int ResourceId { get; private set; }

    /// <summary>
    /// Creates an instance of this attribute.
    /// </summary>
    /// <param name="resourceId">Server resource id</param>
    public DataPortalServerResourceAttribute(int resourceId)
    {
      ResourceId = resourceId;
    }
  }
}
