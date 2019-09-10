using System.Collections.Generic;
using System.Linq;
using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;
using CompareFieldsRules.Properties;

namespace CompareFieldsRules.Rules
{
  public class AnyRequired : CommonBusinessRule
  {
    public AnyRequired(IPropertyInfo primaryProperty, params IPropertyInfo[] additionalProperties)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo>() {primaryProperty};
      InputProperties.AddRange(additionalProperties);
      this.RuleUri.AddQueryParameter("any", string.Join(",", additionalProperties.Select(p => p.Name).ToArray()));
    }

    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.GetMessage() : Resources.AnyRequiredRule;
    }

    protected override void Execute(IRuleContext context)
    {
      // if all values are Null or Empty
      if (context.InputPropertyValues.Select(keyvalue => keyvalue.Value.ToString().Trim()).All(string.IsNullOrEmpty))
      {
        var fieldNames = string.Join(", ", context.InputPropertyValues.Select(p =>p.Key.FriendlyName));

        foreach (var field in context.InputPropertyValues)
        {
          context.Results.Add(new RuleResult(this.RuleName, field.Key, string.Format(GetMessage(), fieldNames)) { Severity = this.Severity });
        }
      }
    }
  }
}
