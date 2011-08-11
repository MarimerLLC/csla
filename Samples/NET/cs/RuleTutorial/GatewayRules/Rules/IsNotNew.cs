using System.Collections.Generic;
using System.Linq;
using Csla.Rules;

namespace GatewayRules.Rules
{
  /// Gateway rule that will only call inner rule when object.IsNew is false
  public class IsNotNew : PropertyRule
  {
    /// <summary>
    /// Gets the inner rule.
    /// </summary>
    public Csla.Rules.IBusinessRule InnerRule { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsNotNew"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="innerRule">The inner rule.</param>
    public IsNotNew(Csla.Core.IPropertyInfo primaryProperty, Csla.Rules.IBusinessRule innerRule)
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
      var target = (Csla.Core.ITrackStatus)context.Target;
      if (!target.IsNew)
      {
        var chainedContext = context.GetChainedContext(InnerRule);
        InnerRule.Execute(chainedContext);
      }
    }
  }
}
