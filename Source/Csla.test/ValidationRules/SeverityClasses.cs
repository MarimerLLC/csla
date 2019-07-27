//-----------------------------------------------------------------------
// <copyright file="SeverityClasses.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.ValidationRules
{
  public class SeverityRoot : Csla.BusinessBase<SeverityRoot>
  {
    public static PropertyInfo<string> TestProperty = RegisterProperty<string>(c => c.Test);
    public string Test
    {
      get { return GetProperty(TestProperty); }
      set { SetProperty(TestProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new AlwaysError { PrimaryProperty = TestProperty });
      BusinessRules.AddRule(new AlwaysWarns { PrimaryProperty = TestProperty });
      BusinessRules.AddRule(new AlwaysInfo { PrimaryProperty = TestProperty });
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    public class AlwaysInfo : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        context.AddInformationResult("Always info");
      }
    }

    public class AlwaysWarns : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        context.AddWarningResult("Always warns");
      }
    }

    public class AlwaysError : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        context.AddErrorResult("Always error");
      }
    }
  }

  public class NoErrorRoot : Csla.BusinessBase<NoErrorRoot>
  {
    public static PropertyInfo<string> TestProperty = RegisterProperty<string>(c => c.Test);
    public string Test
    {
      get { return GetProperty(TestProperty); }
      set { SetProperty(TestProperty, value); }
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new AlwaysInfo { PrimaryProperty = TestProperty });
      BusinessRules.AddRule(new AlwaysWarns { PrimaryProperty = TestProperty });
    }

    public class AlwaysInfo : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        context.AddInformationResult("Always info");
      }
    }

    public class AlwaysWarns : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        context.AddWarningResult("Always warns");
      }
    }
  }
}