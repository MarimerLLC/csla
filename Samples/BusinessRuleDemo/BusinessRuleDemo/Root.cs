using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Csla;
using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace BusinessRuleDemo
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    [Required]
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(nameof(Num1));
    public int Num1
    {
      get => GetProperty(Num1Property);
      set => SetProperty(Num1Property, value);
    }

    public static readonly PropertyInfo<int> Num2Property = RegisterProperty<int>(nameof(Num2));
    [Range(1, 6000)]
    public int Num2
    {
      get => GetProperty(Num2Property);
      set => SetProperty(Num2Property, value);
    }

    public static readonly PropertyInfo<int> SumProperty = RegisterProperty<int>(nameof(Sum));
    public int Sum
    {
      get => GetProperty(SumProperty);
      set => SetProperty(SumProperty, value);
    }

    public static readonly PropertyInfo<string> CountryProperty = RegisterProperty<string>(nameof(Country));
    public string Country
    {
      get => GetProperty(CountryProperty);
      set => SetProperty(CountryProperty, value);
    }

    public static readonly PropertyInfo<string> StateProperty = RegisterProperty<string>(nameof(State));
    public string State
    {
      get => GetProperty(StateProperty);
      set => SetProperty(StateProperty, value);
    }

    public static readonly PropertyInfo<string> StateNameProperty = RegisterProperty<string>(nameof(StateName));
    public string StateName
    {
      get => GetProperty(StateNameProperty);
      set => SetProperty(StateNameProperty, value);
    }

    public static readonly PropertyInfo<string> AdditionalInfoForUSProperty = RegisterProperty<string>(nameof(AdditionalInfoForUS));
    public string AdditionalInfoForUS
    {
      get => GetProperty(AdditionalInfoForUSProperty);
      set => SetProperty(AdditionalInfoForUSProperty, value);
    }

    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // add authorization rules 
      BusinessRules.AddRule(new OnlyForUS(AuthorizationActions.WriteProperty, StateProperty, CountryProperty));

      // add validation rules 
      // set up dependencies to that Sum is automatially recaclulated when PrimaryProperty is changed 
      BusinessRules.AddRule(new Dependency(Num1Property, SumProperty));
      BusinessRules.AddRule(new Dependency(Num2Property, SumProperty));

      // add dependency for LessThanProperty rule on Num1
      BusinessRules.AddRule(new Dependency(Num2Property, Num1Property));

      // add dependency for StringRequiredIfUS on AddistionalInfoForUs
      BusinessRules.AddRule(new Dependency(CountryProperty, AdditionalInfoForUSProperty));

      BusinessRules.AddRule(new MaxValue<int>(Num1Property, 5000));
      BusinessRules.AddRule(new LessThanProperty(Num1Property, Num2Property));

      // calculates sum rule - must alwas run before MinValue with lower priority
      BusinessRules.AddRule(new CalcSum(SumProperty, Num1Property, Num2Property) {Priority = -1});
      BusinessRules.AddRule(new MinValue<int>(SumProperty, 1));

      BusinessRules.AddRule(new StringRequiredIfUS(AdditionalInfoForUSProperty, CountryProperty));

      // Name Property - uses DataAnnotation Required combined with a Csla MaxLength rule
      //BusinessRules.AddRule(new Required(NameProperty));
      BusinessRules.AddRule(new MaxLength(NameProperty, 10));

      BusinessRules.AddRule(new SetStateName(StateProperty, StateNameProperty));
    }
    
    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      base.OnDeserialized(context);

      this.ChildChanged += MyChildChanged;
    }

    private void MyChildChanged(object sender, ChildChangedEventArgs e)
    {
      Debug.Print(e.ChildObject.ToString(), e.ListChangedArgs);
    }

    [Create]
    private void Create()
    {
      using (BypassPropertyChecks)
      {
        Num1 = 5001;
        Num2 = 6001;
        Country = "UZ";
      }
      BusinessRules.CheckRules();
    }
  }
}