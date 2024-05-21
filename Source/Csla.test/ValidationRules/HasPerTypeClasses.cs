//-----------------------------------------------------------------------
// <copyright file="HasPerTypeClasses.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ValidationRules
{
  public class HasPerTypeRules : BusinessBase<HasPerTypeRules>
  {
    public static PropertyInfo<string> TestProperty = RegisterProperty<string>(c => c.Test);
    public string Test
    {
      get { return GetProperty(TestProperty); }
      set { SetProperty(TestProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Rules.CommonRules.Required(TestProperty));
      BusinessRules.AddRule(new Rules.CommonRules.MaxLength(TestProperty, 5));

      int value = int.Parse(TestResults.GetResult("Shared"));
      TestResults.AddOrOverwrite("Shared", (++value).ToString());
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    [Create]
    private void Create()
    {
    }
  }

  public class HasPerTypeRules2 : BusinessBase<HasPerTypeRules2>
  {
    public static PropertyInfo<string> TestProperty = RegisterProperty<string>(c => c.Test);
    public string Test
    {
      get { return GetProperty(TestProperty); }
      set { SetProperty(TestProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Rules.CommonRules.Required(TestProperty));
      BusinessRules.AddRule(new Rules.CommonRules.MaxLength(TestProperty, 5));

      int value = int.Parse(TestResults.GetResult("Shared"));
      TestResults.AddOrOverwrite("Shared", (++value).ToString());
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    [Create]
    private void Create()
    {
    }
  }


  public class HasOnlyPerTypeRules : BusinessBase<HasOnlyPerTypeRules>
  {
    public static PropertyInfo<string> TestProperty = RegisterProperty<string>(c => c.Test);
    public string Test
    {
      get { return GetProperty(TestProperty); }
      set { SetProperty(TestProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Rules.CommonRules.Required(TestProperty));
      BusinessRules.AddRule(new Rules.CommonRules.MaxLength(TestProperty, 5));

      int value = int.Parse(TestResults.GetResult("Shared"));
      TestResults.AddOrOverwrite("Shared", (++value).ToString());
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    [Create]
    private void Create()
    {
    }
  }

  public class HasRuleSetRules : BusinessBase<HasRuleSetRules>
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
      BusinessRules.AddRule(new Rules.CommonRules.Required(NameProperty));
      BusinessRules.RuleSet = "test";
      BusinessRules.AddRule(new Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(new Rules.CommonRules.MaxLength(NameProperty, 5));
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

    [Create]
    private void Create()
    {
    }
  }
}