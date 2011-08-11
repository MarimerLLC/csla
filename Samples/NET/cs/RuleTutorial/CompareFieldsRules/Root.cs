using System;
using CompareFieldsRules.Rules;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace CompareFieldsRules
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Methods

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1, null, 8);
    public int Num1
    {
      get { return GetProperty(Num1Property); }
      set { SetProperty(Num1Property, value); }
    }


    public static readonly PropertyInfo<int> Num2Property = RegisterProperty<int>(c => c.Num2);
    public int Num2
    {
      get { return GetProperty(Num2Property); }
      set { SetProperty(Num2Property, value); }
    }

    public static readonly PropertyInfo<SmartDate> StartDateProperty = RegisterProperty<SmartDate>(c => c.StartDate, null, new SmartDate() {Date = new Func<DateTime>(() => DateTime.Now).Invoke()});
    public string StartDate
    {
      get { return GetPropertyConvert<SmartDate, string>(StartDateProperty); }
      set { SetPropertyConvert<SmartDate, string>(StartDateProperty, value); }
    }

    public static readonly PropertyInfo<SmartDate> EndDateProperty = RegisterProperty<SmartDate>(c => c.EndDate, null, new SmartDate());
    public string EndDate
    {
      get { return GetPropertyConvert<SmartDate, string>(EndDateProperty); }
      set { SetPropertyConvert<SmartDate, string>(EndDateProperty, value); }
    }

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // add authorization rules 
      BusinessRules.AddRule(new GreaterThanOrEqual(Num2Property, Num1Property));
      BusinessRules.AddRule(new LessThan(StartDateProperty, EndDateProperty));
      BusinessRules.AddRule(new GreaterThan(EndDateProperty, StartDateProperty));
    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }


    #endregion

  }
}
