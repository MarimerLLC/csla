using System;
using Csla;
using Csla.Data;
using Csla.Rules;
using Csla.Rules.CommonRules;
using Rolodex.Business.Data;
using Rolodex.Business.Rules;
using RolodexEF;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class CompanyContact : BusinessBase<CompanyContact>
  {
    #region Properties

    public static readonly PropertyInfo<int> CompanyContactIdProperty =
      RegisterProperty(new PropertyInfo<int>("CompanyContactId", "Contact Id", 0));

    public int CompanyContactId
    {
      get { return GetProperty(CompanyContactIdProperty); }
    }

    public static readonly PropertyInfo<int> CompanyIdProperty =
      RegisterProperty(new PropertyInfo<int>("CompanyId", "Company Id", 0));

    public int CompanyId
    {
      get { return GetProperty(CompanyIdProperty); }
      set { SetProperty(CompanyIdProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty =
      RegisterProperty(new PropertyInfo<string>("FirstName", "First Name", string.Empty));

    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty =
      RegisterProperty(new PropertyInfo<string>("LastName", "Last Name", string.Empty));

    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    public static readonly PropertyInfo<int> RankIdProperty =
      RegisterProperty(new PropertyInfo<int>("RankId", "Rank", 0));

    public int RankId
    {
      get { return GetProperty(RankIdProperty); }
      set { SetProperty(RankIdProperty, value); }
    }

    public static readonly PropertyInfo<int> InitialRankIdProperty =
      RegisterProperty(new PropertyInfo<int>("InitialRankId", "Rank", 0));

    public int InitialRankId
    {
      get { return GetProperty(InitialRankIdProperty); }
    }

    public static readonly PropertyInfo<SmartDate> BirthdayProperty =
      RegisterProperty(new PropertyInfo<SmartDate>("Birthday", "Birthday"));

    public DateTime? Birthday
    {
      get { return GetProperty(BirthdayProperty).ToNullableDate(); }
      set { SetProperty(BirthdayProperty, new SmartDate(value)); }
    }

    public static readonly PropertyInfo<decimal> BaseSalaryProperty =
      RegisterProperty(new PropertyInfo<decimal>("BaseSalary", "Base Salary"));

    public decimal BaseSalary
    {
      get { return GetProperty(BaseSalaryProperty); }
      set { SetProperty(BaseSalaryProperty, value); }
    }

    public static readonly PropertyInfo<CompanyContactPhoneList> ContactsPhonesProperty =
      RegisterProperty(new PropertyInfo<CompanyContactPhoneList>("ContactPhones", "Contact Phones"));

    public CompanyContactPhoneList ContactPhones
    {
      get { return GetProperty(ContactsPhonesProperty); }
    }

    #endregion

    public static void AddObjectAuthorizationRules()
    {
      var canWrite = new[] {"AdminUser", "RegularUser"};
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};
      var admin = new[] {"AdminUser"};

      BusinessRules.AddRule(typeof(CompanyContact), new IsInRole(AuthorizationActions.CreateObject, admin));
      BusinessRules.AddRule(typeof(CompanyContact), new IsInRole(AuthorizationActions.DeleteObject, admin));
      BusinessRules.AddRule(typeof(CompanyContact), new IsInRole(AuthorizationActions.EditObject, canWrite));
      BusinessRules.AddRule(typeof(CompanyContact), new IsInRole(AuthorizationActions.GetObject, canRead));
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Required(FirstNameProperty));
      BusinessRules.AddRule(new Required(LastNameProperty));
      BusinessRules.AddRule(new MaxLength(FirstNameProperty, 30));
      BusinessRules.AddRule(new MaxLength(LastNameProperty, 50));
      BusinessRules.AddRule(new Required(BirthdayProperty));
      BusinessRules.AddRule(new IsDateValid(BirthdayProperty));
      BusinessRules.AddRule(new MinValue<int>(RankIdProperty, 1));

      var canWrite = new[] {"AdminUser", "RegularUser"};
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};

      FieldManager.GetRegisteredProperties().ForEach(item =>
      {
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, item, canWrite));
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, item, canRead));
      });
    }

    internal static CompanyContact NewCompanyContact()
    {
      var newContact = new CompanyContact();
      newContact.LoadProperty(ContactsPhonesProperty, CompanyContactPhoneList.NewCompanyContactPhoneList());
      newContact.MarkAsChild();
      newContact.BusinessRules.CheckRules();
      return newContact;
    }

    internal static CompanyContact GetCompanyContact(CompanyContacts contact)
    {
      return DataPortal.FetchChild<CompanyContact>(contact);
    }

    private void Child_Fetch(CompanyContacts contact)
    {
      LoadProperty(CompanyIdProperty, contact.Companies.CompanyId);
      LoadProperty(CompanyContactIdProperty, contact.CompanyContactId);
      LoadProperty(FirstNameProperty, contact.FirstName);
      LoadProperty(LastNameProperty, contact.LastName);
      LoadProperty(BirthdayProperty, new SmartDate(contact.Birthday));
      LoadProperty(RankIdProperty, contact.Ranks.RankId);
      LoadProperty(InitialRankIdProperty, contact.Ranks.RankId);
      LoadProperty(BaseSalaryProperty, contact.BaseSalary);
      LoadProperty(ContactsPhonesProperty, CompanyContactPhoneList.GetCompanyContactPhoneList(contact));
    }

    private RolodexEF.Ranks GetRank(int rankID)
    {
      using (var manager =
        ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        RolodexEF.Ranks rank;

        System.Data.EntityKey rankKey = new System.Data.EntityKey("RolodexEntities.Ranks", "RankId", rankID);

        System.Data.Objects.ObjectStateEntry entry;
        if (!manager.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(rankKey, out entry))
        {
          rank = new RolodexEF.Ranks();
          rank.RankId = rankID;
          rank.EntityKey = rankKey;
          manager.ObjectContext.Attach(rank);
        }
        else
        {
          rank = entry.Entity as RolodexEF.Ranks;
        }

        return rank;
      }
    }

    private void Child_Insert(Company company, Companies entityCompany)
    {
      using (var manager =
        ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        LoadProperty(CompanyIdProperty, company.CompanyId);
        RolodexEF.Ranks newRank = GetRank(ReadProperty(RankIdProperty));

        var newContact = new CompanyContacts();
        newContact.BaseSalary = ReadProperty(BaseSalaryProperty);
        var birthday = ReadProperty(BirthdayProperty);
        if (!birthday.IsEmpty)
          newContact.Birthday = birthday.Date;
        newContact.Companies = entityCompany;
        newContact.FirstName = ReadProperty(FirstNameProperty);
        newContact.LastName = ReadProperty(LastNameProperty);
        newContact.Ranks = newRank;

        LoadProperty(InitialRankIdProperty, ReadProperty(RankIdProperty));
        if (ReadProperty(CompanyContactIdProperty) == 0)
          newContact.PropertyChanged += entityContact_PropertyChanged;

        DataPortal.UpdateChild(ReadProperty(ContactsPhonesProperty), this, newContact);
      }
    }

    void entityContact_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      var entityContact = sender as CompanyContacts;
      if (e.PropertyName == CompanyContactIdProperty.Name)
      {
        LoadProperty(CompanyContactIdProperty, entityContact.CompanyContactId);
        entityContact.PropertyChanged -= entityContact_PropertyChanged;
      }
    }

    private void Child_Update(Company company, Companies entityCompany)
    {
      using (var manager =
        ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        RolodexEF.Ranks newRank = GetRank(ReadProperty(RankIdProperty));
        RolodexEF.Ranks oldRank = GetRank(ReadProperty(InitialRankIdProperty));

        var newContact = new CompanyContacts();
        newContact.CompanyContactId = ReadProperty(CompanyContactIdProperty);
        newContact.EntityKey = new System.Data.EntityKey("RolodexEntities.CompanyContacts", "CompanyContactId",
          ReadProperty(CompanyContactIdProperty));

        manager.ObjectContext.Attach(newContact);
        entityCompany.CompanyContacts.Attach(newContact);
        oldRank.CompanyContacts.Attach(newContact);
        newContact.Ranks = newRank;

        newContact.BaseSalary = ReadProperty(BaseSalaryProperty);
        var birthday = ReadProperty(BirthdayProperty);
        if (!birthday.IsEmpty)
          newContact.Birthday = birthday.Date;
        else
          newContact.Birthday = null;

        newContact.FirstName = ReadProperty(FirstNameProperty);
        newContact.LastName = ReadProperty(LastNameProperty);

        DataPortal.UpdateChild(ReadProperty(ContactsPhonesProperty), this, newContact);
      }
    }

    private void Child_DeleteSelf(Company company, Companies entityCompany)
    {
      if (!IsNew)
      {
        using (var manager =
          ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
        {
          var deleted = new CompanyContacts();
          deleted.CompanyContactId = ReadProperty(CompanyContactIdProperty);
          deleted.EntityKey = new System.Data.EntityKey("RolodexEntities.CompanyContacts", "CompanyContactId",
            deleted.CompanyContactId);
          manager.ObjectContext.Attach(deleted);
          entityCompany.CompanyContacts.Attach(deleted);
          manager.ObjectContext.DeleteObject(deleted);
        }
      }
    }
  }
}