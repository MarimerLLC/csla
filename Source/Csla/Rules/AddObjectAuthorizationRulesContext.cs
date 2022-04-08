//-----------------------------------------------------------------------
// <copyright file="IAddObjectAuthorizationRulesContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context for the AddObjectAuthorizationRulesContext method.</summary>
//-----------------------------------------------------------------------
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

    /// <summary>
    /// Gets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext ApplicationContext { get; private set; }

    /// <summary>
    /// Gets a data portal factory instance
    /// </summary>
    public IDataPortalFactory DataPortalFactory => ApplicationContext.CurrentServiceProvider.GetRequiredService<IDataPortalFactory>();

    /// <summary>
    /// Gets the current rule set
    /// </summary>
    public string RuleSet => ApplicationContext.RuleSet;
  }
}
