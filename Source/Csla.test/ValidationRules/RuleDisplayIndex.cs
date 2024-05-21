﻿//-----------------------------------------------------------------------
// <copyright file="BrokenRulesMergeRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Rules;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class RuleDisplayIndex : BusinessBase<RuleDisplayIndex>
  {
    public static PropertyInfo<string> Test1Property = RegisterProperty<string>(c => c.Test1);
    public string Test1
    {
      get { return GetProperty(Test1Property); }
      set { SetProperty(Test1Property, value); }
    }

    public static PropertyInfo<string> Test2Property = RegisterProperty<string>(c => c.Test2);
    public string Test2
    {
      get { return GetProperty(Test2Property); }
      set { SetProperty(Test2Property, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new RuleBroken(Test1Property) { DisplayIndex = 2});
      BusinessRules.AddRule(new RuleBroken(Test2Property) { DisplayIndex = 1});
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    public class RuleBroken : BusinessRule
    {
      public RuleBroken(Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      { }

      protected override void Execute(IRuleContext context)
      {
        context.AddErrorResult("Broken: " + PrimaryProperty.FriendlyName);
      }
    }

    public override BrokenRulesCollection BrokenRulesCollection
    {
      get
      {
        var result = new BrokenRulesCollection();
        result.AddRange(base.BrokenRulesCollection);
        return result;
      }
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }
}