using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;

namespace GatewayRules.Rules
{
  /// <summary>
  /// Gateway rule that will only call inner rule when uuser can edit property
  /// </summary>
  public class CanWrite : PropertyRule
  {
    /// <summary>
    /// Gets the inner rule.
    /// </summary>
    public Csla.Rules.IBusinessRule InnerRule { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CanWrite"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="innerRule">The inner rule.</param>
    public CanWrite(Csla.Core.IPropertyInfo primaryProperty, Csla.Rules.IBusinessRule innerRule)
      : base(primaryProperty)
    {
      InputProperties = new List<Csla.Core.IPropertyInfo> { primaryProperty };
      InnerRule = innerRule;
      RuleUri.AddQueryParameter("rule", System.Uri.EscapeUriString(InnerRule.RuleName));

      // merge InnerRule input property list into this rule's list 
      if (InnerRule.InputProperties != null)
        InputProperties.AddRange(InnerRule.InputProperties);
      // remove any duplicates 
      InputProperties = new List<Csla.Core.IPropertyInfo>(InputProperties.Distinct());
      AffectedProperties.AddRange(innerRule.AffectedProperties);
    }

    /// <summary>
    /// Executes the rule
    /// </summary>
    /// <param name="context">The rule context.</param>
    protected override void Execute(Csla.Rules.RuleContext context)
    {
      var target = (Csla.Core.BusinessBase)context.Target;
      
      if (target.CanWriteProperty(PrimaryProperty))
      {
        var chainedContext = context.GetChainedContext(InnerRule);
        InnerRule.Execute(chainedContext);
      }
    }
  }
}
