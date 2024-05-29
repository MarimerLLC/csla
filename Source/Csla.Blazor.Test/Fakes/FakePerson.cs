using System.ComponentModel.DataAnnotations;
using Csla.Blazor.Test.Rules;

namespace Csla.Blazor.Test.Fakes
{
  [Serializable]
  public class FakePerson : BusinessBase<FakePerson>
  {
    public static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(nameof(Id));
    public static PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(nameof(FirstName));
    public static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(nameof(LastName));
    public static PropertyInfo<string> HomeTelephoneProperty = RegisterProperty<string>(nameof(HomeTelephone));
    public static PropertyInfo<string> MobileTelephoneProperty = RegisterProperty<string>(nameof(MobileTelephone));
    public static PropertyInfo<FakePersonEmailAddresses> EmailAddressesProperty = RegisterProperty<FakePersonEmailAddresses>(nameof(EmailAddresses));

    public static string FirstNameFailOnInsertValue = "FailOnInsert";

    #region Properties 

    public Guid Id
    {
      get => GetProperty(IdProperty);
      private set => SetProperty(IdProperty, value);
    }

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

    public FakePersonEmailAddresses EmailAddresses
    {
      get { return GetProperty(EmailAddressesProperty); }
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      Csla.Rules.CommonRules.CommonBusinessRule rule;
      base.AddBusinessRules();
      BusinessRules.AddRule(new OneOfSeveralStringsRequiredRule(HomeTelephoneProperty, MobileTelephoneProperty));

      // Add additional rules for warning severity level
      rule = new Csla.Rules.CommonRules.MinLength(LastNameProperty, 2, "Last name is quite short!");
      rule.Severity = Csla.Rules.RuleSeverity.Warning;
      BusinessRules.AddRule(rule);

      // Add additional rules for information severity level
      rule = new Csla.Rules.CommonRules.MinLength(FirstNameProperty, 2, "First name is a bit short");
      rule.Severity = Csla.Rules.RuleSeverity.Information;
      BusinessRules.AddRule(rule);
    }

    protected override void OnChildChanged(Core.ChildChangedEventArgs e)
    {
      if (e.ChildObject is FakePersonEmailAddresses)
      {
        BusinessRules.CheckRules(EmailAddressesProperty);
        OnPropertyChanged(EmailAddressesProperty);
      }
      base.OnChildChanged(e);
    }

    #endregion

    #region Data Access

    [RunLocal]
    [Create]
    private void Create([Inject] IChildDataPortal<FakePersonEmailAddresses> dataPortal)
    {
      Id = new Guid();
      // Create an empty list for holding email addresses
      LoadProperty(EmailAddressesProperty, dataPortal.CreateChild());

      // Trigger object checks
      BusinessRules.CheckRules();
    }

    [RunLocal]
    [Fetch]
    private FakePerson Fetch(Guid id)
    {
      return FakeDataStorage.GetFakePerson(id);
    }

    [RunLocal]
    [Insert]
    private void Insert()
    {
      if (FirstName == FirstNameFailOnInsertValue) {
        throw new Exception("Insert failed");
      }

      FakeDataStorage.InsertFakePerson(this);
    }

    [RunLocal]
    [Update]
    private void Update()
    {
      FakeDataStorage.UpdateFakePerson(this);
    }
    #endregion

  }
}
