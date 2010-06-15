//-----------------------------------------------------------------------
// <copyright file="AuthorizationRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Base class providing basic authorization rule</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Base class providing basic authorization rule
  /// implementation.
  /// </summary>
  public abstract class AuthorizationRule : IAuthorizationRule
  {
    private Csla.Core.IMemberInfo _element;
    private AuthorizationActions _action;

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    public AuthorizationRule(AuthorizationActions action)
    {
      _action = action;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="element">Method or property.</param>
    public AuthorizationRule(AuthorizationActions action, Csla.Core.IMemberInfo element)
      : this(action)
    {
      _element = element;
    }

    /// <summary>
    /// Authorization rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected abstract void Execute(AuthorizationContext context);

    /// <summary>
    /// Gets the name of the element (property/method)
    /// to which this rule is associated.
    /// </summary>
    protected Csla.Core.IMemberInfo Element
    {
      get { return _element; }
    }

    /// <summary>
    /// Gets the authorization action this rule
    /// will enforce.
    /// </summary>
    public AuthorizationActions Action
    {
      get { return _action; }
    }

    #region IAuthorizationRule

    void IAuthorizationRule.Execute(AuthorizationContext context)
    {
      Execute(context);
    }

    Csla.Core.IMemberInfo IAuthorizationRule.Element
    {
      get { return Element; }
    }

    /// <summary>
    /// Gets the authorization action this rule
    /// will enforce.
    /// </summary>
    AuthorizationActions IAuthorizationRule.Action
    {
      get { return Action; }
    }

    #endregion
  }
}