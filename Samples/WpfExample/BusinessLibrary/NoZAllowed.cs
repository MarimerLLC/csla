using Csla.Rules;

namespace BusinessLibrary
{
  public class NoZAllowed : BusinessRule
  {
    public NoZAllowed(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    { }

    protected override void Execute(IRuleContext context)
    {
      if (context.Target is null || PrimaryProperty is null)
      {
        return;
      }

      var text = ReadProperty(context.Target, PrimaryProperty) as string;
      if (string.IsNullOrWhiteSpace(text)) return;
      if (text.ToLower().Contains("z"))
        context.AddErrorResult("No letter Z allowed");
    }
  }
}
