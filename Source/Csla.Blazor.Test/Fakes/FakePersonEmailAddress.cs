using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Csla.Blazor.Test.Fakes
{
  public class FakePersonEmailAddress : BusinessBase<FakePersonEmailAddress>
  {
    public static Csla.PropertyInfo<string> EmailAddressProperty = RegisterProperty<string>(nameof(EmailAddress));

    #region Properties

    [Required]
    [MaxLength(250)]
    public string EmailAddress
    {
      get { return GetProperty(EmailAddressProperty); }
      set { SetProperty(EmailAddressProperty, value); }
    }

    #endregion

    #region Data Access

    [RunLocal]
    [Create]
    private void Create()
    {
      // Trigger object checks
      BusinessRules.CheckRules();
    }

    #endregion

  }
}
