// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRuleTest.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Helper class for creating unit tests on authorization  rules.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Csla.Rules;

namespace RuleTutorial.Testing.Common
{
  /// <summary>
  /// Helper class for creating unit tests on authorization rules
  /// </summary>
  public class AuthorizationRuleTest
  {
    public AuthorizationContext  AuthorizationContext { get; private set; }
    protected static ObjectAccessor Accessor = new ObjectAccessor();

    /// <summary>
    /// Initializes the test.
    /// </summary>
    /// <param name="rule">The rule.</param>
    /// <param name="target">The target.</param>
    /// <param name="type">The type.</param>
    public void InitializeTest(IAuthorizationRule rule, object target, Type type)
    {
      AuthorizationContext = new AuthorizationContext(rule, target, type);
    }

    /// <summary>
    /// Executes the rule with the AuthorizationContext.
    /// </summary>
    public void ExecuteRule()
    {
      AuthorizationContext.Rule.Execute(AuthorizationContext);
    }
  }
}
