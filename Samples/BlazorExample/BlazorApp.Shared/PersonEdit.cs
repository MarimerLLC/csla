using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace BlazorApp.Shared
{
  [Serializable]
  public class PersonEdit : BusinessBase<PersonEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(c => c.FirstName);
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty = RegisterProperty<string>(c => c.LastName);
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    public static readonly PropertyInfo<int> UpdateCounterProperty = RegisterProperty<int>(c => c.UpdateCounter);
    public int UpdateCounter
    {
      get { return GetProperty(UpdateCounterProperty); }
      private set { LoadProperty(UpdateCounterProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(FirstNameProperty, "First name required"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(LastNameProperty, "Last name required"));
    }

    private void DataPortal_Fetch(int id)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        FirstName = "Luke";
        LastName = "Cage";
      }
    }

    protected override void DataPortal_Insert()
    {
      UpdateCounter = UpdateCounter + 1;
    }

    protected override void DataPortal_Update()
    {
      UpdateCounter = UpdateCounter + 1;
    }
  }
}
