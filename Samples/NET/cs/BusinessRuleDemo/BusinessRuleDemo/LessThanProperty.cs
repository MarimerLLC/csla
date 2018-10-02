using System.Collections.Generic;
using System.Threading;
using Csla.Core;
using Csla.Rules;

namespace BusinessRuleDemo
{
  /// <summary>
  /// Implements a rule to compare 2 property values and make sure property1 is less than property2
  /// </summary>
  public class LessThanProperty : BusinessRule
  {
    private IPropertyInfo CompareTo { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanProperty"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="compareToProperty">The compare to property.</param>
    public LessThanProperty(IPropertyInfo primaryProperty, IPropertyInfo compareToProperty)
      : base(primaryProperty)
    {
      CompareTo = compareToProperty;

      if (InputProperties == null)
      {
        InputProperties = new List<IPropertyInfo>();
      }
      InputProperties.Add(primaryProperty);
      InputProperties.Add(compareToProperty);
    }

    /// <summary>
    /// Does the check for primary propert less than compareTo property
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected override void Execute(IRuleContext context)
    {
      var value1 = (dynamic)context.InputPropertyValues[PrimaryProperty];
      var value2 = (dynamic)context.InputPropertyValues[CompareTo];

      if (value1 > value2)
      {
        context.AddErrorResult(string.Format("{0} must be less than or equal {1}", PrimaryProperty.FriendlyName, CompareTo.FriendlyName));
      }
    }
  }
}
