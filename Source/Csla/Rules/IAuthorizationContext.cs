//-----------------------------------------------------------------------
// <copyright file="IAuthorizationContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining an authorization context</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Claims;
using System.Security.Principal;
using Csla.Core;

namespace Csla.Rules 
{
  /// <summary>
  /// Implemented by objects which provide context information to an authorization
  /// rule when it is invoked.
  /// </summary>
  public interface IAuthorizationContext 
  {
    /// <summary>
    /// Gets the rule object.
    /// </summary>
    IAuthorizationRule Rule { get; }

    /// <summary>
    /// Gets a reference to the target business object.
    /// </summary>
    object Target { get; }

    /// <summary>
    /// Gets the type of the target business class.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// current user has permission to perform the requested
    /// action.
    /// </summary>
    bool HasPermission { get; set; }

    /// <summary>
    /// Gets an object which is the criteria specified in the data portal call, if any.
    /// </summary>
    object[] Criteria { get; }

    /// <summary>
    /// Gets the current user principal.
    /// </summary>
    IPrincipal User { get; }
    /// <summary>
    /// Gets the current user ClaimsPrincipal.
    /// </summary>
    ClaimsPrincipal Principal { get; }
    /// <summary>
    /// Gets the LocalContext.
    /// </summary>
    ContextDictionary LocalContext { get; }
    /// <summary>
    /// Gets the ClientContext.
    /// </summary>
    ContextDictionary ClientContext { get; }
  }
}