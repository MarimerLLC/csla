using System;
using Csla;
using Csla.Rules.CommonRules;

namespace CustomMessage
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

    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1);
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

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // add authorization rules - use MessageText to set a custom constant string message
      BusinessRules.AddRule(new Required(NameProperty){MessageText = "Customer must have name"});
      BusinessRules.AddRule(new MaxLength(NameProperty, 50){Priority = 1, MessageText = "{0} cannot be longer than {1} chars"});
      BusinessRules.AddRule(new MinValue<int>(Num1Property, 5) { MessageText = "Num1 must be larger than or equal to {1}" });
      
    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }


    #endregion

    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
    }


  }
}
