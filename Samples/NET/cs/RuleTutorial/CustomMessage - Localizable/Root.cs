using System;
using Csla;
using Csla.Rules.CommonRules;
using CustomMessageLocalizable.Properties;

namespace CustomMessageLocalizable
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

      // add authorization rules 
      // MessageDelegate takes an anonymous function that returns the message. 
      // this allows you to use ResourceString and get the string associated with the current Threads CurrentCulture. 
      BusinessRules.AddRule(new Required(NameProperty){MessageDelegate = () => Resources.NameRequired});
      BusinessRules.AddRule(new MaxLength(NameProperty, 50){Priority = 1, MessageDelegate = () => Resources.NameMaxLength});
      BusinessRules.AddRule(new MinValue<int>(Num1Property, 5) { MessageDelegate = () => Resources.Num1MinValue });
    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }


    #endregion

    #region Data Access

    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
    }

    #endregion
  }
}
