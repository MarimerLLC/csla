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
    /// Gets the local context
    /// </summary>
    HybridDictionary LocalContext { get; }
    /// <summary>
    /// Gets the client context
    /// </summary>
    HybridDictionary ClientContext { get; }
    /// <summary>
    /// Gets a data portal factory instance
    /// </summary>
    IDataPortalFactory DataPortalFactory { get; }
    /// <summary>
    /// Gets the current rule set
    /// </summary>
    string RuleSet { get; }
    /// <summary>
    /// Gets the current user
    /// </summary>
    IPrincipal User { get; }
    /// <summary>
    /// Gets the current user as a ClaimsPrincipal
    /// </summary>
    ClaimsPrincipal Principal { get; }
  }
}