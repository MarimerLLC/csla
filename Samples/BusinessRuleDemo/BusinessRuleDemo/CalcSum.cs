using System.Collections.Generic;
using System.Linq;
using Csla.Core;
using Csla.Rules;

namespace BusinessRuleDemo
{
  /// <summary>
  /// CalcSum rule will set primary property to the sum of all.
  /// </summary>
  public class CalcSum : BusinessRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CalcSum"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="inputProperties">The input properties.</param>
    public CalcSum(IPropertyInfo primaryProperty, params IPropertyInfo[] inputProperties)
      : base(primaryProperty)
    {
      if (InputProperties == null)
      {
        InputProperties = new List<IPropertyInfo>();
      }
      InputProperties.AddRange(inputProperties);
    }

    protected override void Execute(IRuleContext context)
    {
      // Use linq Sum to calculate the sum value 
      var sum = context.InputPropertyValues.Sum(property => (dynamic)property.Value);

      // add calculated value to OutValues 
      // When rule is completed the RuleEngig will update businessobject
      context.AddOutValue(PrimaryProperty, sum);
    }
  }
}
