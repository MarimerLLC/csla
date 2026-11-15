using Csla.Core;
using Csla.Rules;

namespace AddonPropertyRules.Rules
{
  /// <summary>
  /// Sets PrimaryProperty to the values of two input properties, separated by a space.
  /// </summary>
  /// <remarks>
  /// Use it for a calculated property (see <see cref="Csla.CalculatedPropertyAttribute"/>).
  /// PrimaryProperty is the calculated property, so CSLA runs this rule again
  /// after the object is deserialized.
  /// </remarks>
  public class ConcatStrings : BusinessRule
  {
    /// <summary>
    /// Gets the first input property.
    /// </summary>
    public IPropertyInfo FirstProperty { get; }

    /// <summary>
    /// Gets the second input property.
    /// </summary>
    public IPropertyInfo SecondProperty { get; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property that gets the combined value.</param>
    /// <param name="firstProperty">First input property.</param>
    /// <param name="secondProperty">Second input property.</param>
    public ConcatStrings(IPropertyInfo primaryProperty, IPropertyInfo firstProperty, IPropertyInfo secondProperty)
      : base(primaryProperty)
    {
      FirstProperty = firstProperty;
      SecondProperty = secondProperty;
      InputProperties.Add(firstProperty);
      InputProperties.Add(secondProperty);
    }

    /// <summary>
    /// Business rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected override void Execute(IRuleContext context)
    {
      var first = context.InputPropertyValues[FirstProperty];
      var second = context.InputPropertyValues[SecondProperty];
      context.AddOutValue(PrimaryProperty!, string.Format("{0} {1}", first, second));
    }
  }
}
