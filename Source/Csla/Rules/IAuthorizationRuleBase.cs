//-----------------------------------------------------------------------
// <copyright file="IAuthorizationRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining an authorization</summary>
//-----------------------------------------------------------------------

namespace Csla.Rules
{
  /// <summary>
  /// Interface defining an authorization
  /// rule base implementation.
  /// </summary>
  public interface IAuthorizationRuleBase
  {
    /// <summary>
    /// Gets the element (property/method)
    /// to which this rule is associated.
    /// </summary>
    Csla.Core.IMemberInfo Element { get; }
    /// <summary>
    /// Gets the authorization action this rule
    /// will enforce.
    /// </summary>
    AuthorizationActions Action { get; }
    /// <summary>
    /// Gets a value indicating whether the results
    /// of this rule can be cached at the business
    /// object level.
    /// </summary>
    bool CacheResult { get; }
  }

  /// <summary>
  /// Interface defining an authorization
  /// rule implementation.
  /// </summary>
  public interface IAuthorizationRule : IAuthorizationRuleBase
  {
    /// <summary>
    /// Authorization rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    void Execute(IAuthorizationContext context);
  }

  /// <summary>
  /// Interface defining an authorization
  /// rule implementation.
  /// </summary>
  public interface IAuthorizationRuleAsync : IAuthorizationRuleBase
  {
    /// <summary>
    /// Authorization rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    /// <param name="ct">Cancellation token.</param>
    Task ExecuteAsync(IAuthorizationContext context, CancellationToken ct);
  }
}