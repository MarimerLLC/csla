﻿//-----------------------------------------------------------------------
// <copyright file="AuthorizationRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>IsInRole authorization rule.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules.CommonRules
{
  /// <summary>
  /// IsInRole authorization rule.
  /// </summary>
  public class IsInRole : AuthorizationRule
  {
    private List<string> _roles;

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="roles">List of allowed roles.</param>
    public IsInRole(AuthorizationActions action, List<string> roles)
      : base(action)
    {
      _roles = roles;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="roles">List of allowed roles.</param>
    public IsInRole(AuthorizationActions action, params string[] roles)
      : base(action)
    {
      _roles = new List<string>(roles);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="element">Member to be authorized.</param>
    /// <param name="roles">List of allowed roles.</param>
    public IsInRole(AuthorizationActions action, Csla.Core.IMemberInfo element, List<string> roles)
      : base(action, element)
    {
      _roles = roles;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="element">Member to be authorized.</param>
    /// <param name="roles">List of allowed roles.</param>
    public IsInRole(AuthorizationActions action, Csla.Core.IMemberInfo element, params string[] roles)
      : base(action, element)
    {
      _roles = new List<string>(roles);
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(AuthorizationContext context)
    {
      if (_roles.Count > 0)
      {
        foreach (var item in _roles)
          if (Csla.ApplicationContext.User.IsInRole(item))
          {
            context.HasPermission = true;
            break;
          }
      }
      else
      {
        context.HasPermission = true;
      }
    }
  }

  /// <summary>
  /// IsNotInRole authorization rule.
  /// </summary>
  public class IsNotInRole : AuthorizationRule
  {
    private List<string> _roles;

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="roles">List of disallowed roles.</param>
    public IsNotInRole(AuthorizationActions action, List<string> roles)
      : base(action)
    {
      _roles = roles;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="roles">List of disallowed roles.</param>
    public IsNotInRole(AuthorizationActions action, params string[] roles)
      : base(action)
    {
      _roles = new List<string>(roles);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="element">Member to be authorized.</param>
    /// <param name="roles">List of disallowed roles.</param>
    public IsNotInRole(AuthorizationActions action, Csla.Core.IMemberInfo element, List<string> roles)
      : base(action, element)
    {
      _roles = roles;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="element">Member to be authorized.</param>
    /// <param name="roles">List of disallowed roles.</param>
    public IsNotInRole(AuthorizationActions action, Csla.Core.IMemberInfo element, params string[] roles)
      : base(action, element)
    {
      _roles = new List<string>(roles);
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(AuthorizationContext context)
    {
      context.HasPermission = true;
      foreach (var item in _roles)
        if (Csla.ApplicationContext.User.IsInRole(item))
        {
          context.HasPermission = false;
          break;
        }
    }
  }
}