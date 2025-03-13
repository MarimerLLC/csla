//-----------------------------------------------------------------------
// <copyright file="AuthorizationContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context information provided to an authorization</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

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
    public IAuthorizationRuleBase Rule { get; }
    /// <summary>
    /// Gets a reference to the target business object.
    /// </summary>
    public object? Target { get; }
    /// <summary>
    /// Gets the type of the target business class.
    /// </summary>
    public Type TargetType { get; }
    /// <summary>
    /// Gets or sets a value indicating whether the
    /// current user has permission to perform the requested
    /// action.
    /// </summary>
    public bool HasPermission { get; set; }

    /// <summary>
    /// Gets an object which is the criteria specified in the data portal call, if any.
    /// </summary>
    public object?[]? Criteria { get; }

    /// <summary>
    /// Gets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext ApplicationContext { get; }

    /// <summary>
    /// Gets a data portal factory instance
    /// </summary>
    public IDataPortalFactory DataPortalFactory => ApplicationContext.CurrentServiceProvider.GetRequiredService<IDataPortalFactory>();

    /// <summary>
    /// Creates a AuthorizationContext instance for unit testing.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="rule"/> or <paramref name="targetType"/> is <see langword="null"/>.</exception>
    public AuthorizationContext(ApplicationContext applicationContext, IAuthorizationRuleBase rule, object? target, Type targetType) : this(applicationContext, rule, target, targetType, null)
    {
    }

    /// <summary>
    /// Creates a AuthorizationContext instance for unit testing.
    /// </summary>
    /// <param name="targetType">Type of the target.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="rule"/> or <paramref name="targetType"/> is <see langword="null"/>.</exception>
    internal AuthorizationContext(ApplicationContext applicationContext, IAuthorizationRuleBase rule, object? target, Type targetType, object?[]? criteria)
    {
      ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      Rule = rule ?? throw new ArgumentNullException(nameof(rule));
      Target = target;
      TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
      Criteria = criteria;
    }
  }
}