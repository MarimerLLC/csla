//-----------------------------------------------------------------------
// <copyright file="HasPermissionAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Authorizes user if they have permission</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.AspNetCore.Authorization;

namespace Csla.Blazor
{
  /// <summary>
  /// Authorizes user if they have permission.
  /// </summary>
  public class HasPermissionAttribute : AuthorizeAttribute
  {
    /// <summary>
    /// Creates instance of type
    /// </summary>
    /// <param name="action">Authorization action</param>
    /// <param name="objectType">Business domain type</param>
    public HasPermissionAttribute(Rules.AuthorizationActions action, Type objectType)
      : base(CslaPolicy.GetPolicy(action, objectType))
    { }
  }
}
