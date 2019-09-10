using Csla.Rules;

namespace BlazorExample.Shared
{
  public class NoZAllowed : BusinessRule
  {
    public NoZAllowed(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    { }

    protected override void Execute(IRuleContext context)
    {
      var text = (string)ReadProperty(context.Target, PrimaryProperty);
      if (text.ToLower().Contains("z"))
        context.AddErrorResult("No letter Z allowed");
    }
  }
}
