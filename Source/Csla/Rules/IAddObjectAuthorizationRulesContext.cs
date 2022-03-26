//-----------------------------------------------------------------------
// <copyright file="IAddObjectAuthorizationRulesContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context for the AddObjectAuthorizationRulesContext method.</summary>
//-----------------------------------------------------------------------
using System.Collections.Specialized;
using System.Security.Claims;
using System.Security.Principal;

namespace Csla.Rules
{
  /// <summary>
  /// Context for the AddObjectAuthorizationRulesContext method.
  /// </summary>
  public interface IAddObjectAuthorizationRulesContext
  {
    /// <summary>
    /// Gets a reference to the current ApplicationContext.
    /// </summary>
    ApplicationContext ApplicationContext { get; }
    /// <summary>
    /// Gets a data portal factory instance
    /// </summary>
    IDataPortalFactory DataPortalFactory { get; }
    /// <summary>
    /// Gets the current rule set
    /// </summary>
    string RuleSet { get; }
  }
}