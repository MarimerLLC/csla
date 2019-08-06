using Csla.Core;
using Csla.Rules;

namespace Templates
{
  public class BusinessRuleClass : Csla.Rules.BusinessRule
  {

    // TODO: Add additional parameters to your rule to the constructor
    public BusinessRuleClass(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      // TODO: If you are  going to add InputProperties make sure to uncomment line below as InputProperties is NULL by default
      //if (InputProperties == null) InputProperties = new List<IPropertyInfo>();

      // TODO: Add additional constructor code here 



      // TODO: Marke rule for IsAsync if Execute method implemets asyncronous calls
      // IsAsync = true; 
    }

    protected override void Execute(IRuleContext context)
    {
      // TODO: Asyncronous rules 
      // If rule is async make sure that ALL excution paths call context.Complete


      // TODO: Add actual rule code here. 
      //if (broken condition)
      //{
      //  context.AddErrorResult("Broken rule message");
      //}
    }
  }
}
