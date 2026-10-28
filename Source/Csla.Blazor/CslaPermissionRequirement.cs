//-----------------------------------------------------------------------
// <copyright file="CslaPermissionRequirement.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>CSLA permissions requirement</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

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
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is <see langword="null"/>.</exception>
    public CslaPermissionRequirement(Rules.AuthorizationActions action, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType)
    {
      Action = action;
      ObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
    }

    /// <summary>
    /// Gets or sets the authorization action
    /// </summary>
    public Rules.AuthorizationActions Action { get; set; }
    /// <summary>
    /// Gets or sets the business object type
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type ObjectType { get; set; }
  }
}
