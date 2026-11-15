using Csla.Rules;

namespace BusinessLibrary
{
  /// <summary>
  /// Sets the primary property to the length of a text property.
  /// </summary>
  /// <remarks>
  /// The primary property is the calculated one, so CSLA runs this rule
  /// again after the object is deserialized.
  /// </remarks>
  public class LetterCount : BusinessRule
  {
    public LetterCount(Csla.Core.IPropertyInfo primaryProperty, Csla.Core.IPropertyInfo textProperty)
      : base(primaryProperty)
    {
      InputProperties.Add(textProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var text = (string?)context.InputPropertyValues[InputProperties[0]];
      var count = text?.Length ?? 0;
      context.AddOutValue(PrimaryProperty!, count);
    }
  }
}
