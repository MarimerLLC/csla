using System;
using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;

namespace TransformationRules.Rules
{
  public class ToUpper : Csla.Rules.BusinessRule
  {
    public ToUpper(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo>(){primaryProperty};
      AffectedProperties.Add(primaryProperty);
    }

    protected override void Execute(RuleContext context)
    {
      var value = (string) context.InputPropertyValues[PrimaryProperty];
      context.AddOutValue(PrimaryProperty, value.ToUpper());

     if (context.IsCheckRulesContext)
        Console.WriteLine(".... Rule {0} running from CheckRules", this.GetType().Name);
      else
        Console.WriteLine(".... Rule {0} running from {1} was changed", this.GetType().Name, this.PrimaryProperty.Name);
    }
  }
}
