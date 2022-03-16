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
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Rules
{
  /// <summary>
  /// Context for the AddObjectAuthorizationRulesContext method.
  /// </summary>
  public class AddObjectAuthorizationRulesContext : IAddObjectAuthorizationRulesContext
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    public AddObjectAuthorizationRulesContext(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext;
    }

    private ApplicationContext ApplicationContext { get; set; }
    /// <summary>
    /// Gets the local context
    /// </summary>
    public HybridDictionary LocalContext => ApplicationContext.LocalContext;
    /// <summary>
    /// Gets the client context
    /// </summary>
    public HybridDictionary ClientContext => ApplicationContext.ClientContext;
    /// <summary>
    /// Gets a data portal factory instance
    /// </summary>
    public IDataPortalFactory DataPortalFactory => ApplicationContext.CurrentServiceProvider.GetRequiredService<IDataPortalFactory>();
    /// <summary>
    /// Gets the current rule set
    /// </summary>
    public string RuleSet => ApplicationContext.RuleSet;
    /// <summary>
    /// Gets the current user
    /// </summary>
    public IPrincipal User => ApplicationContext.User;
    /// <summary>
    /// Gets the current user as a ClaimsPrincipal
    /// </summary>
    public ClaimsPrincipal Principal => ApplicationContext.Principal;
  }
}
