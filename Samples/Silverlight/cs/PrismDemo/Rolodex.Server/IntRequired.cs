using System.Collections.Generic;
using Csla.Properties;
using Csla.Rules;
using Csla.Rules.CommonRules;
using Csla.Core;

namespace Rolodex
{
    public class IntRequired : CommonBusinessRule
    {
        protected override void Execute(RuleContext context)
        {
            var value = (int)context.InputPropertyValues[PrimaryProperty];
            if (value <= 0)
            {
                var message = string.Format(Resources.StringRequiredRule, PrimaryProperty.FriendlyName);
                context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) { Severity = Severity });
            }
        }

        public IntRequired(IPropertyInfo primaryProperty)
            : base(primaryProperty)
        {
            InputProperties = new List<IPropertyInfo> { primaryProperty };
        }
    }
}
