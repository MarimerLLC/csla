using System.Collections.Generic;
using System.Linq;
using Csla;
using Csla.Core;
using Csla.Rules;

namespace BusinessRuleDemo
{
  public class SetStateName : BusinessRule
  {
    public IPropertyInfo StateName { get; set; }

    public SetStateName(IPropertyInfo stateIdProperty, IPropertyInfo stateNameProperty)
      : base(stateIdProperty)
    {
      StateName = stateNameProperty;
      InputProperties = new List<IPropertyInfo> { stateIdProperty };
      AffectedProperties.Add(StateName);
    }

    /// <summary>
    /// Look up State and set the state name 
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected override void Execute(IRuleContext context)
    {
      var stateId = (string)context.InputPropertyValues[PrimaryProperty];
      var state = DataPortal.Fetch<StatesNVL>().Where(p => p.Key == stateId).FirstOrDefault();
      context.AddOutValue(StateName, state == null ? "Unknown state" : state.Value);
    }
  }
}
