//-----------------------------------------------------------------------
// <copyright file="ShortCircuitClasses.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class ShortCircuit : BusinessBase<ShortCircuit>
  {
    public static PropertyInfo<string> TestProperty = RegisterProperty<string>(c => c.Test);
    public string Test
    {
      get { return GetProperty(TestProperty); }
      set { SetProperty(TestProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(TestProperty));
      BusinessRules.AddRule(new AlwaysWarns { PrimaryProperty = TestProperty });
      BusinessRules.AddRule(new AlwaysFails { PrimaryProperty = TestProperty, Priority = 10 });
    }

    public int Threshold
    {
      get { return BusinessRules.ProcessThroughPriority; }
      set { BusinessRules.ProcessThroughPriority = value; }
    }

    public void CheckRules()
    {
      BusinessRules.CheckRules();
    }

    public class AlwaysWarns : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        context.AddWarningResult("Always warns");
      }
    }

    public class AlwaysFails : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        context.AddErrorResult("Always error");
      }
    }
  }
}