//-----------------------------------------------------------------------
// <copyright file="HasPerTypeClasses.cs" company="Marimer LLC">
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
  public class HasPerTypeRules : Csla.BusinessBase<HasPerTypeRules>
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
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(TestProperty, 5));

      int value = (int)ApplicationContext.GlobalContext["Shared"];
      ApplicationContext.GlobalContext["Shared"] = ++value;
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }
  }

  public class HasPerTypeRules2 : Csla.BusinessBase<HasPerTypeRules2>
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
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(TestProperty, 5));

      int value = (int)ApplicationContext.GlobalContext["Shared"];
      ApplicationContext.GlobalContext["Shared"] = ++value;
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }
  }


  public class HasOnlyPerTypeRules : Csla.BusinessBase<HasOnlyPerTypeRules>
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
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(TestProperty, 5));

      int value = (int)ApplicationContext.GlobalContext["Shared"];
      ApplicationContext.GlobalContext["Shared"] = ++value;
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }
  }

  public class HasRuleSetRules : Csla.BusinessBase<HasRuleSetRules>
  {
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.RuleSet = "test";
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(NameProperty, 5));
    }

    public string[] GetDefaultRules()
    {
      BusinessRules.RuleSet = "default";
      return BusinessRules.GetRuleDescriptions();
    }

    public string[] GetTestRules()
    {
      BusinessRules.RuleSet = "test";
      return BusinessRules.GetRuleDescriptions();
    }
  }
}