using System;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;
using GatewayRules.Rules;

namespace GatewayRules
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

    public static readonly PropertyInfo<string> NoteProperty = RegisterProperty<string>(c => c.Note);
    public string Note
    {
        get { return GetProperty(NoteProperty); }
        set { SetProperty(NoteProperty, value); }
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
      BusinessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, NoteProperty, "nobody"));

      // required only when user can write to field
      BusinessRules.AddRule(new CanWrite(NoteProperty, new Required(NoteProperty)));

      // different rules for Name whether object is New or Existing. 
      BusinessRules.AddRule(new IsNew(NameProperty, new Required(NameProperty)));
      BusinessRules.AddRule(new IsNotNew(NameProperty, new MaxLength(NameProperty, 50)));

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
      using (BypassPropertyChecks)
      {
        Name = "Rocky Lhotka";
        Num1 = 1001;
        Num2 = 666;
      }
      base.DataPortal_Create();
    }

    #endregion
  }
}
