﻿//-----------------------------------------------------------------------
// <copyright file="IAuthorizationContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining an authorization context</summary>
//-----------------------------------------------------------------------

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
    IAuthorizationRuleBase Rule { get; }

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
    /// Gets a reference to the current ApplicationContext.
    /// </summary>
    ApplicationContext ApplicationContext { get; }

    /// <summary>
    /// Gets a data portal factory instance
    /// </summary>
    IDataPortalFactory DataPortalFactory { get; }
  }
}