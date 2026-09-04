//-----------------------------------------------------------------------
// <copyright file="RuleTesterTestBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Shared setup for the rule tester tests</summary>
//-----------------------------------------------------------------------

using Csla.Testing;

namespace Csla.Testing.Tests.Rules
{
  /// <summary>
  /// Provides an application context for creating the business objects used as
  /// rule targets. The rule testers create their own services, so this exists
  /// only to construct the target objects the way an application would.
  /// </summary>
  public abstract class RuleTesterTestBase
  {
    /// <summary>
    /// An application context for creating target business objects.
    /// </summary>
    protected static ApplicationContext TestApplicationContext =>
      CslaTestHost.Create().ApplicationContext;
  }
}
