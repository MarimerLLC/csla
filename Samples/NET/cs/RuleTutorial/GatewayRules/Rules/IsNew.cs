// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsNew.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Gateway rule that will only call inner rule when object.IsNew is true
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Csla.Rules;

namespace GatewayRules.Rules
{
  using Csla.Core;

  /// <summary>
  /// Gateway rule that will only call inner rule when object.IsNew is true
  /// </summary>
  public class IsNew : PropertyRule
  {
    /// <summary>
    /// Gets the inner rule.
    /// </summary>
    public IBusinessRule InnerRule { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsNew"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    /// <param name="innerRule">
    /// The inner rule.
    /// </param>
    public IsNew(IPropertyInfo primaryProperty, IBusinessRule innerRule)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo> { primaryProperty };
      InnerRule = innerRule;
      RuleUri.AddQueryParameter("rule", System.Uri.EscapeUriString(InnerRule.RuleName));

      // merge InnerRule input property list into this rule's list 
      if (InnerRule.InputProperties != null)
      {
        InputProperties.AddRange(InnerRule.InputProperties);
      }

      // remove any duplicates 
      InputProperties = new List<IPropertyInfo>(InputProperties.Distinct());
      AffectedProperties.AddRange(innerRule.AffectedProperties);
    }

    /// <summary>
    /// Executes the rule
    /// </summary>
    /// <param name="context">
    /// The rule context.
    /// </param>
    protected override void Execute(IRuleContext context)
    {
      var target = (Csla.Core.ITrackStatus)context.Target;
      if (target.IsNew)
      {
        var chainedContext = context.GetChainedContext(InnerRule);
        InnerRule.Execute(chainedContext);
      }
    }
  }
}
