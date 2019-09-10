using System;
using System.Data;
using System.Linq;
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
  public class Company : BusinessBase<Company>
  {
    public static readonly PropertyInfo<int> CompanyIdProperty =
      RegisterProperty(new PropertyInfo<int>("CompanyId", "Company Id", 0));

    public int CompanyId
    {
      get { return GetProperty(CompanyIdProperty); }
    }

    public static readonly PropertyInfo<string> CompanyNameProperty =
      RegisterProperty(new PropertyInfo<string>("CompanyName", "Company Name", string.Empty));

    public string CompanyName
    {
      get { return GetProperty(CompanyNameProperty); }
      set { SetProperty(CompanyNameProperty, value); }
    }

    public static readonly PropertyInfo<SmartDate> DateAddedProperty =
      RegisterProperty(new PropertyInfo<SmartDate>("DateAdded", "Date Added"));

    public DateTime? DateAdded
    {
      get { return GetProperty(DateAddedProperty).ToNullableDate(); }
      set { SetProperty(DateAddedProperty, new SmartDate(value)); }
    }

    public static readonly PropertyInfo<CompanyContactList> ContactsProperty =
      RegisterProperty(new PropertyInfo<CompanyContactList>("Contacts", "Contacts"));

    public CompanyContactList Contacts
    {
      get { return GetProperty(ContactsProperty); }
    }

    public static readonly PropertyInfo<Ranks> RanksProperty =
      RegisterProperty(new PropertyInfo<Ranks>("Ranks", "Ranks"));

    public Ranks Ranks
    {
      get { return GetProperty(RanksProperty); }
    }

    protected static void AddObjectAuthorizationRules()
    {
      var canWrite = new[] {"AdminUser", "RegularUser"};
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};
      var admin = new[] {"AdminUser"};

      BusinessRules.AddRule(typeof(Company), new IsInRole(AuthorizationActions.CreateObject, admin));
      BusinessRules.AddRule(typeof(Company), new IsInRole(AuthorizationActions.DeleteObject, admin));
      BusinessRules.AddRule(typeof(Company), new IsInRole(AuthorizationActions.EditObject, canWrite));
      BusinessRules.AddRule(typeof(Company), new IsInRole(AuthorizationActions.GetObject, canRead));
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new Required(CompanyNameProperty));
      BusinessRules.AddRule(new MaxLength(CompanyNameProperty, 50));
      BusinessRules.AddRule(new Required(DateAddedProperty));
      BusinessRules.AddRule(new IsDateValid(DateAddedProperty));
      BusinessRules.AddRule(new IsDuplicateNameAsync(CompanyIdProperty, CompanyNameProperty));

      var canWrite = new[] {"AdminUser", "RegularUser"};
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};

      FieldManager.GetRegisteredProperties().ForEach(item =>
      {
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, item, canWrite));
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, item, canRead));
      });
    }

    public static void GetCompany(int companyId, EventHandler<DataPortalResult<Company>> handler)
    {
      var dp = new DataPortal<Company>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Company, int>(companyId));
    }

    public static void CreateCompany(EventHandler<DataPortalResult<Company>> handler)
    {
      var dp = new DataPortal<Company>();
      dp.CreateCompleted += handler;
      dp.BeginCreate();
    }

    protected void DataPortal_Fetch(SingleCriteria<Company, int> criteria)
    {
      using (var manager =
        ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        var company = (from oneCompany in manager.ObjectContext.Companies
            .Include("CompanyContacts")
            .Include("CompanyContacts.Ranks")
            .Include("CompanyContacts.CompanyContactPhones")
          where oneCompany.CompanyId == criteria.Value
          select oneCompany).FirstOrDefault();
        if (company != null)
        {
          LoadProperty<int>(CompanyIdProperty, company.CompanyId);
          LoadProperty<string>(CompanyNameProperty, company.CompanyName);
          LoadProperty<SmartDate>(DateAddedProperty, company.DateAdded);
          LoadProperty(ContactsProperty, CompanyContactList.GetCompanyContactList(company));
        }
      }
      LoadProperty(RanksProperty, Ranks.GetRanks());
    }

    protected override void DataPortal_Create()
    {
      LoadProperty<CompanyContactList>(ContactsProperty, CompanyContactList.NewCompanyContactList());
      LoadProperty(RanksProperty, Ranks.GetRanks());
      BusinessRules.CheckRules();
    }

    //used for testing only
    //[Transactional(TransactionalTypes.TransactionScope)]
    //protected void DataPortal_Delete(SingleCriteria<Company, int> criteria)
    //{
    //  using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
    //  {
    //    Companies deleted = new Companies();
    //    deleted.CompanyId = criteria.Value;
    //    deleted.EntityKey = new System.Data.EntityKey("RolodexEntities.Companies", "CompanyId", criteria.Value);
    //    manager.ObjectContext.Attach(deleted);
    //    manager.ObjectContext.DeleteObject(deleted);
    //    manager.ObjectContext.SaveChanges();
    //  }
    //}

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      if (!IsNew)
      {
        using (var manager =
          ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
        {
          var deleted = new Companies();
          deleted.CompanyId = CompanyId;
          deleted.EntityKey = new System.Data.EntityKey("RolodexEntities.Companies", "CompanyId", CompanyId);
          manager.ObjectContext.Attach(deleted);
          manager.ObjectContext.DeleteObject(deleted);
          manager.ObjectContext.SaveChanges();
        }
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (var manager =
        ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        var newCompany = new Companies();
        newCompany.CompanyName = ReadProperty(CompanyNameProperty);
        SmartDate added = ReadProperty(DateAddedProperty);
        if (!added.IsEmpty)
          newCompany.DateAdded = added.Date;
        DataPortal.UpdateChild(ReadProperty(ContactsProperty), this, newCompany);

        manager.ObjectContext.AddToCompanies(newCompany);
        if (ReadProperty(CompanyIdProperty) == 0)
          newCompany.PropertyChanged += newCompany_PropertyChanged;

        DataPortal.UpdateChild(ReadProperty(ContactsProperty), this, newCompany);
        manager.ObjectContext.SaveChanges();
        LoadProperty(CompanyIdProperty, newCompany.CompanyId);
      }
    }

    private void newCompany_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      var entityCompany = sender as Companies;
      if (e.PropertyName == CompanyIdProperty.Name)
      {
        LoadProperty(CompanyIdProperty, entityCompany.CompanyId);
        entityCompany.PropertyChanged -= newCompany_PropertyChanged;
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      using (var manager =
        ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        var newCompany = new Companies();
        newCompany.CompanyId = ReadProperty(CompanyIdProperty);
        newCompany.EntityKey = new System.Data.EntityKey("RolodexEntities.Companies", "CompanyId", newCompany.CompanyId);
        manager.ObjectContext.Attach(newCompany);

        newCompany.CompanyName = ReadProperty(CompanyNameProperty);
        var added = ReadProperty(DateAddedProperty);
        if (!added.IsEmpty)
          newCompany.DateAdded = added.Date;

        DataPortal.UpdateChild(ReadProperty(ContactsProperty), this, newCompany);
        manager.ObjectContext.SaveChanges();
      }
    }
  }
}