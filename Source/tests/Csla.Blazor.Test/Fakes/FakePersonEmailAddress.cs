using System.ComponentModel.DataAnnotations;

namespace Csla.Blazor.Test.Fakes
{
  [Serializable]
  public class FakePersonEmailAddress : BusinessBase<FakePersonEmailAddress>
  {
    public static PropertyInfo<string> EmailAddressProperty = RegisterProperty<string>(nameof(EmailAddress));

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
