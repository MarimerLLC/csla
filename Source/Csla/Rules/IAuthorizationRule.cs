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
}