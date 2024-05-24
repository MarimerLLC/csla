//-----------------------------------------------------------------------
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
  public class BrokenRulesPriority : BusinessBase<BrokenRulesPriority>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new CheckAA(NameProperty) { Priority = 3 });
      BusinessRules.AddRule(new CheckCase(NameProperty) { Priority = 1 });
      BusinessRules.AddRule(new NoZAllowed(NameProperty) { Priority = 2 });
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

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

    public class CheckCase : BusinessRule
    {
      public CheckCase(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      { }

      protected override void Execute(IRuleContext context)
      {
        var text = (string)ReadProperty(context.Target, PrimaryProperty);
        if (string.IsNullOrWhiteSpace(text)) return;
        var ideal = text.Substring(0, 1).ToUpper();
        ideal += text.Substring(1).ToLower();
        if (text != ideal)
          context.AddErrorResult("Check capitalization");
      }
    }
    public class CheckAA : BusinessRule
    {
      public CheckAA(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      { }

      protected override void Execute(IRuleContext context)
      {
        var text = (string)ReadProperty(context.Target, PrimaryProperty);
        if (string.IsNullOrWhiteSpace(text)) return;

        if (!text.Contains("aa"))
          context.AddErrorResult("aa should be in Name");
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