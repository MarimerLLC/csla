using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Rules;
using System.Collections;

namespace CslaMVC.Rules
{
    /// <summary>
    /// simple rule that tests value is in provided IList&lt;T&gt;
    /// </summary>
    /// <typeparam name="T">property data type that this rule is applied</typeparam>
    public class IsInListRule<T> : BusinessRule
    {
        private IList<T> ListOfValues { get; set; }

        public IsInListRule(IPropertyInfo primaryProperty, IList<T> listOfValues)
            : base(primaryProperty)
        {
            Priority = 10; //set priority to delay until after standard rules are executed (can be changed at rule addition if necessary)
            ListOfValues = listOfValues;
            InputProperties = new List<IPropertyInfo> { primaryProperty };
        }

        protected override void Execute(RuleContext context)
        {
            if (context.InputPropertyValues[PrimaryProperty] != null)
            {
                var val = (T)context.InputPropertyValues[PrimaryProperty];
                if (!ListOfValues.Contains(val))
                    context.AddErrorResult(string.Format("'{0}' is not in the list of expected values.", PrimaryProperty.FriendlyName));
            }
        }
    }
}
