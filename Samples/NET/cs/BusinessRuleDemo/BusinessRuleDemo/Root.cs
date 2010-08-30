using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Rules.CommonRules;

namespace BusinessRuleDemo
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Methods

    private static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required]   // Data Annotations rule for Required field
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1);
    public int Num1
    {
      get { return GetProperty(Num1Property); }
      set { SetProperty(Num1Property, value); }
    }

    [Range(1, 6000)]
    private static readonly PropertyInfo<int> Num2Property = RegisterProperty<int>(c => c.Num2);
    public int Num2
    {
      get { return GetProperty(Num2Property); }
      set { SetProperty(Num2Property, value); }
    }

    private static readonly PropertyInfo<int> SumProperty = RegisterProperty<int>(c => c.Sum);
    public int Sum
    {
      get { return GetProperty(SumProperty); }
      set { SetProperty(SumProperty, value); }
    }

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // set up dependencies to that Sum is automatially recaclulated when PrimaryProperty is changed 
      BusinessRules.AddRule(new Dependency(Num1Property, SumProperty));
      BusinessRules.AddRule(new Dependency(Num2Property, SumProperty));
      BusinessRules.AddRule(new Dependency(Num2Property, Num1Property));

      BusinessRules.AddRule(new MaxValue<int>(Num1Property, 5000));
      BusinessRules.AddRule(new LessThanProperty(Num1Property, Num2Property));

      // calculates sum rule - must alwas un before MinValue with lower priority
      BusinessRules.AddRule(new CalcSum(SumProperty, Num1Property, Num2Property) { Priority = -1 });
      BusinessRules.AddRule(new MinValue<int>(SumProperty, 1));

      // Name Property
      //BusinessRules.AddRule(new Required(NameProperty));
      BusinessRules.AddRule(new MaxLength(NameProperty, 10));
    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }

    private Root()
    { /* Require use of factory methods */ }

    #endregion
  }
}
