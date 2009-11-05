using System;
using System.Collections.Generic;
using System.Text;
using Csla.Validation;

namespace Csla.Test.ValidationRules
{
  public class BrokenRulesMergeRoot : BusinessBase<BrokenRulesMergeRoot>
  {
    public BrokenRulesMergeRoot()
    { }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(RuleBroken, "Test1");
      ValidationRules.AddRule(RuleBroken, "Test2");
    }

    public void Validate()
    {
      ValidationRules.CheckRules();
    }

    private static bool RuleBroken(object target, RuleArgs e)
    {
      e.Description = "Broken: " + RuleArgs.GetPropertyName(e);
      return false;
    }

    public override Csla.Validation.BrokenRulesCollection BrokenRulesCollection
    {
      get
      {
        BrokenRulesCollection result = BrokenRulesCollection.CreateCollection();
        result.Merge("root", base.BrokenRulesCollection);
        return result;
      }
    }
  }
}
