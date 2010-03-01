using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules.CommonRules
{
  public class MaxLength : BusinessRule
  {
    public int Max { get; private set; }

    public MaxLength(int max)
    {
      Max = max;
    }

    protected override void Rule(RuleContext context)
    {
      var value = context.InputPropertyValues[context.PrimaryProperty].ToString();
      if (value.Length > Max)
        context.AddErrorResult(
          string.Format("{0} value too long", context.PrimaryProperty.FriendlyName));
    }
  }
}
