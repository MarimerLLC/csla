// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldExists.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Gateway rule that will only call inner rule when lazy loaded field (primary property) is initialized.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Csla.Core;
using Csla.Rules;

namespace GatewayRules.Rules
{
  /// <summary>
  /// Gateway rule that will only call inner rule when lazy loaded field (primary property) is initialized.
  /// </summary>
  public class FieldExists : BusinessRule
  {
    /// <summary>
    /// Gets or sets the inner rule.
    /// </summary>
    /// <value>The inner rule.</value>
    public IBusinessRule InnerRule { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldExists"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="innerRule">The inner rule.</param>
    public FieldExists(IPropertyInfo primaryProperty, IBusinessRule innerRule)
      : base(primaryProperty)
    {
      if (InputProperties == null)
        InputProperties = new List<IPropertyInfo>();
      InputProperties.Add(primaryProperty);
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
    /// Calls inner rule if PrimaryProperty has value.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected override void Execute(IRuleContext context)
    {
      if (context.InputPropertyValues.ContainsKey(PrimaryProperty))
      {
        context.ExecuteRule(InnerRule);
      }
    }
  }
}
