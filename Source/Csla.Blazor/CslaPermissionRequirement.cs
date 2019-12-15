//-----------------------------------------------------------------------
// <copyright file="CslaPermissionRequirement.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>CSLA permissions requirement</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.AspNetCore.Authorization;

namespace Csla.Blazor
{
  /// <summary>
  /// CSLA permissions requirement.
  /// </summary>
  public class CslaPermissionRequirement : IAuthorizationRequirement
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="action">Authorization action</param>
    /// <param name="objectType">Business object type</param>
    public CslaPermissionRequirement(Rules.AuthorizationActions action, Type objectType)
    {
      Action = action;
      ObjectType = objectType;
    }

    /// <summary>
    /// Gets or sets the authorization action
    /// </summary>
    public Rules.AuthorizationActions Action { get; set; }
    /// <summary>
    /// Gets or sets the business object type
    /// </summary>
    public Type ObjectType { get; set; }
  }
}
