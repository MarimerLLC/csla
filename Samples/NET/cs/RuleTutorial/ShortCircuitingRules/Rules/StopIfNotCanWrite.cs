using Csla.Rules;

namespace ShortCircuitingRules.Rules
{
  /// <summary>
  ///  Short circuit rule processing for this property if user is not allowed to edit field.
  /// </summary>
  public class StopIfNotCanWrite : BusinessRule
  {
    public StopIfNotCanWrite(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    { }

    protected override void Execute(Csla.Rules.RuleContext context)
    {
      var target = (Csla.Core.BusinessBase)context.Target;
      
      // Short circuit rule processing for this property if user is not allowed to edit field.
      if (!target.CanWriteProperty(PrimaryProperty))
      {
        context.AddSuccessResult(true);
      }
    }
  }
}
