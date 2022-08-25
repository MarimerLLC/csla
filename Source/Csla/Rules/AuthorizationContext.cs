//-----------------------------------------------------------------------
// <copyright file="AuthorizationContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context information provided to an authorization</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Security.Principal;
using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Context information provided to an authorization
  /// rule when it is invoked.
  /// </summary>
  public class AuthorizationContext : IAuthorizationContext 
  {
    /// <summary>
    /// Gets the rule object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IAuthorizationRule Rule { get; internal set; }
    /// <summary>
    /// Gets a reference to the target business object.
    /// </summary>
    public object Target { get; internal set; }
    /// <summary>
    /// Gets the type of the target business class.
    /// </summary>
    public Type TargetType { get; internal set; }
    /// <summary>
    /// Gets or sets a value indicating whether the
    /// current user has permission to perform the requested
    /// action.
    /// </summary>
    public bool HasPermission { get; set; }

    /// <summary>
    /// Gets an object which is the criteria specified in the data portal call, if any.
    /// </summary>
    public object[] Criteria { get; internal set; }

    /// <summary>
    /// Creates a AuthorizationContext instance for unit testing.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="rule">The rule.</param>
    /// <param name="target">The target.</param>
    /// <param name="targetType">Type of the target.</param>
    public AuthorizationContext(ApplicationContext applicationContext, IAuthorizationRule rule, object target, Type targetType)
    {
      ApplicationContext = applicationContext;
      Rule = rule;
      Target = target;
      TargetType = targetType;
    }

    /// <summary>
    /// Gets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext ApplicationContext { get; private set; }

    /// <summary>
    /// Gets a data portal factory instance
    /// </summary>
    public IDataPortalFactory DataPortalFactory =>
      (IDataPortalFactory)ApplicationContext.CurrentServiceProvider.GetService(typeof(IDataPortalFactory));
  }
}