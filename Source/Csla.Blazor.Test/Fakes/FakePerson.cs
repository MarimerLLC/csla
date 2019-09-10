using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Csla.Blazor.Test.Rules;

namespace Csla.Blazor.Test.Fakes
{
  [Serializable()]
  public class FakePerson : BusinessBase<FakePerson>
  {
    public static PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(c => c.FirstName);
    public static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(c => c.LastName);
    public static PropertyInfo<string> HomeTelephoneProperty = RegisterProperty<string>(c => c.HomeTelephone);
    public static PropertyInfo<string> MobileTelephoneProperty = RegisterProperty<string>(c => c.MobileTelephone);

    #region Properties 

    [MaxLength(25)]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    [Required]
    [MaxLength(25)]
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    public string HomeTelephone
    {
      get { return GetProperty(HomeTelephoneProperty); }
      set { SetProperty(HomeTelephoneProperty, value); }
    }

    public string MobileTelephone
    {
      get { return GetProperty(MobileTelephoneProperty); }
      set { SetProperty(MobileTelephoneProperty, value); }
    }

    #endregion

    #region Factory Methods

    public static FakePerson NewFakePerson()
    {
      return DataPortal.Create<FakePerson>();
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new OneOfSeveralStringsRequiredRule(HomeTelephoneProperty, MobileTelephoneProperty));
    }

    #endregion

    #region Data Access

    [RunLocal]
    protected override void DataPortal_Create()
    {
      // Trigger object checks
      CheckObjectRules();
    }

    #endregion

  }
}
