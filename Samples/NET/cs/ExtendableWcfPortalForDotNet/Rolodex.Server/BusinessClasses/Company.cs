using System;
using System.Linq;
using Csla;
using Csla.Security;
using Csla.Validation;
using Csla.Serialization;

#if!SILVERLIGHT
using Rolodex.Business.Data;
using Csla.Data;
using RolodexEF;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class Company : BusinessBase<Company>
  {

#if SILVERLIGHT
        public Company() { }
#else
    private Company() { }
#endif


    private static PropertyInfo<int> CompanyIdProperty = RegisterProperty(new PropertyInfo<int>("CompanyId", "Company Id", 0));
    public int CompanyId
    {
      get
      {
        return GetProperty(CompanyIdProperty);
      }
    }

    private static PropertyInfo<string> CompanyNameProperty = RegisterProperty(new PropertyInfo<string>("CompanyName", "Company Name", string.Empty));
    public string CompanyName
    {
      get
      {
        return GetProperty(CompanyNameProperty);
      }
      set
      {
        SetProperty(CompanyNameProperty, value);
      }
    }

    private static PropertyInfo<SmartDate> DateAddedProperty = RegisterProperty(new PropertyInfo<SmartDate>("DateAdded", "Date Added"));
    public DateTime? DateAdded
    {
      get
      {
        return GetProperty(DateAddedProperty).ToNullableDate();
      }
      set
      {
        SetProperty(DateAddedProperty, new SmartDate(value));
      }
    }

    private static PropertyInfo<CompanyContactList> ContactsProperty = RegisterProperty(new PropertyInfo<CompanyContactList>("Contacts", "Contacts"));
    public CompanyContactList Contacts
    {
      get
      {
        return GetProperty(ContactsProperty);
      }
    }

    private static PropertyInfo<Rolodex.Business.BusinessClasses.Ranks> RanksProperty = RegisterProperty(new PropertyInfo<Rolodex.Business.BusinessClasses.Ranks>("Ranks", "Ranks"));
    public Rolodex.Business.BusinessClasses.Ranks Ranks
    {
      get
      {
        return GetProperty(RanksProperty);
      }
    }


    protected override void AddAuthorizationRules()
    {
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };


      FieldManager.GetRegisteredProperties().ForEach(item =>
      {
        AuthorizationRules.AllowWrite(item, canWrite);
        AuthorizationRules.AllowRead(item, canRead);
      });
    }

    public static void AddObjectAuthorizationRules()
    {
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      string[] admin = new string[] { "AdminUser" };
      AuthorizationRules.AllowCreate(typeof(Company), admin);
      AuthorizationRules.AllowDelete(typeof(Company), admin);
      AuthorizationRules.AllowEdit(typeof(Company), canWrite);
      AuthorizationRules.AllowGet(typeof(Company), canRead);
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(CommonRules.StringRequired, new RuleArgs(CompanyNameProperty));
      ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(CompanyNameProperty, 50));
      ValidationRules.AddRule<Company>(IsDateValid, DateAddedProperty);
      ValidationRules.AddRule(IsDuplicateName, new AsyncRuleArgs(CompanyNameProperty, CompanyIdProperty));
    }

    private static void IsDuplicateName(AsyncValidationRuleContext context)
    {
      DuplicateCompanyCommand command = new DuplicateCompanyCommand(context.PropertyValues["CompanyName"].ToString(), (int)context.PropertyValues["CompanyId"]);
      DataPortal<DuplicateCompanyCommand> dp = new DataPortal<DuplicateCompanyCommand>();
      dp.ExecuteCompleted += (o, e) =>
      {
        if (e.Error != null)
        {
          context.OutArgs.Description = String.Format("Error checking for duplicate company name.  {0}", e.Error);
          context.OutArgs.Severity = RuleSeverity.Error;
          context.OutArgs.Result = false;
        }
        else
        {
          if (e.Object.IsDuplicate)
          {
            context.OutArgs.Description = "Duplicate company name.";
            context.OutArgs.Severity = RuleSeverity.Error;
            context.OutArgs.Result = false;
          }
          else
          {
            context.OutArgs.Result = true;
          }
        }
        context.Complete();
      };
      dp.BeginExecute(command);
    }

    private static bool IsDateValid(Company target, RuleArgs e)
    {
      SmartDate dateAdded = target.GetProperty(DateAddedProperty);
      if (!dateAdded.IsEmpty)
      {
        if (dateAdded.Date < (new DateTime(2000, 1, 1)))
        {
          e.Description = "Date must be greater that 1/1/2000!";
          return false;
        }
        else if (dateAdded.Date > DateTime.Today)
        {
          e.Description = "Date cannot be greater than today!";
          return false;
        }
      }
      else
      {
        e.Description = "Date added is required!";
        return false;
      }
      return true;
    }

    public static void GetCompany(int companyId, EventHandler<DataPortalResult<Company>> handler)
    {
      DataPortal<Company> dp = new DataPortal<Company>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Company, int>(companyId));
    }

    public static void CreateCompany(EventHandler<DataPortalResult<Company>> handler)
    {
      DataPortal<Company> dp = new DataPortal<Company>();
      dp.CreateCompleted += handler;
      dp.BeginCreate();
    }


#if !SILVERLIGHT

    protected void DataPortal_Fetch(SingleCriteria<Company, int> criteria)
    {
      using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        Companies company = (from oneCompany in manager.ObjectContext.Companies
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
      LoadProperty(RanksProperty, Rolodex.Business.BusinessClasses.Ranks.GetRanks());
    }

    protected override void DataPortal_Create()
    {
      LoadProperty<CompanyContactList>(ContactsProperty, CompanyContactList.NewCompanyContactList());
      LoadProperty(RanksProperty, Rolodex.Business.BusinessClasses.Ranks.GetRanks());
      ValidationRules.CheckRules();
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
        using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
        {
          Companies deleted = new Companies();
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
      using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        Companies newCompany = new Companies();
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

    void newCompany_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      Companies entityCompany = sender as Companies;
      if (e.PropertyName == CompanyIdProperty.Name)
      {
        LoadProperty(CompanyIdProperty, entityCompany.CompanyId);
        entityCompany.PropertyChanged -= newCompany_PropertyChanged;
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {

      using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        Companies newCompany = new Companies();
        newCompany.CompanyId = ReadProperty(CompanyIdProperty);
        newCompany.EntityKey = new System.Data.EntityKey("RolodexEntities.Companies", "CompanyId", newCompany.CompanyId);
        manager.ObjectContext.Attach(newCompany);


        newCompany.CompanyName = ReadProperty(CompanyNameProperty);
        SmartDate added = ReadProperty(DateAddedProperty);
        if (!added.IsEmpty)
          newCompany.DateAdded = added.Date;

        DataPortal.UpdateChild(ReadProperty(ContactsProperty), this, newCompany);
        manager.ObjectContext.SaveChanges();
      }

    }
#endif
  }
}
