using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;
using CustomAuthzRules.Rules;
using System.ComponentModel.DataAnnotations;

namespace AuthzReadWriteProperty
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

    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1, "Num1", 9);
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

    public static readonly PropertyInfo<string> CountryProperty = RegisterProperty<string>(c => c.Country);
    public string Country
    {
      get { return GetProperty(CountryProperty); }
      set { SetProperty(CountryProperty, value); }
    }

    public static readonly PropertyInfo<string> StateProperty = RegisterProperty<string>(c => c.State);
    [Required]
    public string State
    {
      get { return GetProperty(StateProperty); }
      set { SetProperty(StateProperty, value); }
    }
    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // State property onlyu writable when Country == "US"
      BusinessRules.AddRule(new OnlyForUS(AuthorizationActions.WriteProperty, StateProperty, CountryProperty));

      // Short circuit rule prosessing for State property when user is not allowed to edit field
      BusinessRules.AddRule(new StopIfNotCanWrite(StateProperty) {Priority = -1});

      BusinessRules.AddRule(new Dependency(CountryProperty, StateProperty));
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
