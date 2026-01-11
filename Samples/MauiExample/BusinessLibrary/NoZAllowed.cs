using Csla.Core;
using Csla.Rules;

namespace BusinessLibrary;

public class NoZAllowed : BusinessRule
{
  public NoZAllowed(IPropertyInfo primaryProperty)
    : base(primaryProperty) 
  {
    InputProperties.Add(primaryProperty);
  }

  protected override void Execute(IRuleContext context)
  {
    var text = (string?)context.InputPropertyValues[PrimaryProperty!];
    if (string.IsNullOrWhiteSpace(text))
      return;

    if (text.IndexOf('z', StringComparison.OrdinalIgnoreCase) >= 0)
      context.AddErrorResult("No letter Z allowed");
  }
}
