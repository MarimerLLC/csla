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
    #region Business Methods

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required]   // Data Annotations rule for Required field
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1);
    public int Num1
    {
      get { return GetProperty(Num1Property); }
      set { SetProperty(Num1Property, value); }
    }


    public static readonly PropertyInfo<int> Num2Property = RegisterProperty<int>(c => c.Num2);
    [Range(1, 6000)]
    public int Num2
    {
      get { return GetProperty(Num2Property); }
      set { SetProperty(Num2Property, value); }
    }

    public static readonly PropertyInfo<int> SumProperty = RegisterProperty<int>(c => c.Sum);
    public int Sum
    {
      get { return GetProperty(SumProperty); }
      set { SetProperty(SumProperty, value); }
    }

    public static readonly PropertyInfo<string> CountryProperty = RegisterProperty<string>(c => c.Country);
    public string Country
    {
      get { return GetProperty(CountryProperty); }
      set { SetProperty(CountryProperty, value); }
    }

    public static readonly PropertyInfo<string> StateProperty = RegisterProperty<string>(c => c.State);
    public string State
    {
      get { return GetProperty(StateProperty); }
      set { SetProperty(StateProperty, value); }
    }

    public static readonly PropertyInfo<string> StateNameProperty = RegisterProperty<string>(c => c.StateName);
    public string StateName
    {
      get { return GetProperty(StateNameProperty); }
      set { SetProperty(StateNameProperty, value); }
    }

    public static readonly PropertyInfo<string> AdditionalInfoForUSProperty = RegisterProperty<string>(c => c.AdditionalInfoForUS);
    public string AdditionalInfoForUS
    {
      get { return GetProperty(AdditionalInfoForUSProperty); }
      set { SetProperty(AdditionalInfoForUSProperty, value); }
    }

    #endregion

    #region Validation Rules

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
      BusinessRules.AddRule(new CalcSum(SumProperty, Num1Property, Num2Property) { Priority = -1 });
      BusinessRules.AddRule(new MinValue<int>(SumProperty, 1));

      BusinessRules.AddRule(new StringRequiredIfUS(AdditionalInfoForUSProperty, CountryProperty));

      // Name Property - uses DataAnnotation Required combined with a Csla MaxLength rule
      //BusinessRules.AddRule(new Required(NameProperty));
      BusinessRules.AddRule(new MaxLength(NameProperty, 10));

      BusinessRules.AddRule(new SetStateName(StateProperty, StateNameProperty));

    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }

    public Root()
    { /* Require use of factory methods */}

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      base.OnDeserialized(context);

      this.ChildChanged += MyChildChanged;
    }

    private void MyChildChanged(object sender, ChildChangedEventArgs e)
    {
      Debug.Print(e.ChildObject.ToString(), e.ListChangedArgs);
    }

    #endregion

    protected override void DataPortal_Create()
    {
      using (BypassPropertyChecks)
      {
        Country = "US";
        State = "AZ";
      }
      BusinessRules.CheckRules();
    }


  }
}
