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
    Core.IMemberInfo Element { get; }
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
}