using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Rules;

namespace CslaMVC.Rules
{
    /// <summary>
    /// rule that enforces if the primary property is set, the secondary property is required
    /// </summary>
    public class AlsoRequiredRule : BusinessRule
    {
        private IPropertyInfo OptionalProperty { get; set; }
        private IPropertyInfo RequiredProperty { get; set; }

        public AlsoRequiredRule(IPropertyInfo triggerProperty, IPropertyInfo optionalProperty, IPropertyInfo requiredProperty)
            : base(triggerProperty)
        {
            OptionalProperty = optionalProperty;
            RequiredProperty = requiredProperty;
            AffectedProperties.Add(OptionalProperty);
            AffectedProperties.Add(RequiredProperty);
            InputProperties = new List<IPropertyInfo> { OptionalProperty, RequiredProperty };
        }

        protected override void Execute(IRuleContext context)
        {
            var optionalVal = (string)context.InputPropertyValues[OptionalProperty];
            var requiredVal = (string)context.InputPropertyValues[RequiredProperty];
            if (!string.IsNullOrEmpty(optionalVal))
            {
                if (string.IsNullOrEmpty(requiredVal))
                {
                    context.AddErrorResult(string.Format("'{0}' is required since '{1}' has been set.", RequiredProperty.FriendlyName, OptionalProperty.FriendlyName));
                }
            }
        }
    }
}
