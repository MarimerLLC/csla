using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Validation;
using System.ComponentModel;
using Csla.DataPortalClient;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class Company : BusinessBase<Company>
  {

#if SILVERLIGHT
    public Company() { DisableIEditableObject = true; }
#else
    private Company() { DisableIEditableObject = true; }
#endif


    private static PropertyInfo<int> CompanyIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyId", "Company Id", 0));
    public int CompanyId
    {
      get
      {
        return GetProperty(CompanyIdProperty);
      }
    }

    private static PropertyInfo<string> CompanyNameProperty = RegisterProperty<string>(new PropertyInfo<string>("CompanyName", "Company Name", string.Empty));
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

    private static PropertyInfo<SmartDate> DateAddedProperty = RegisterProperty<SmartDate>(new PropertyInfo<SmartDate>("DateAdded", "Date Added"));
    public string DateAdded
    {
      get
      {
        return GetProperty(DateAddedProperty).Text;
      }
      set
      {
        SmartDate test = new SmartDate();
        if (SmartDate.TryParse(value, ref test) == true)
        {
          SetProperty(DateAddedProperty, test);
        }
      }
    }

    private static PropertyInfo<CompanyContactList> ContactsProperty = RegisterProperty<CompanyContactList>(new PropertyInfo<CompanyContactList>("Contacts", "Contacts"));
    public CompanyContactList Contacts
    {
      get
      {
        return GetProperty(ContactsProperty);
      }
    }

    protected override void AddAuthorizationRules()
    {
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      string[] admin = new string[] { "AdminUser" };
      AuthorizationRules.AllowCreate(typeof(Company), admin);
      AuthorizationRules.AllowDelete(typeof(Company), admin);
      AuthorizationRules.AllowEdit(typeof(Company), canWrite);
      AuthorizationRules.AllowGet(typeof(Company), canRead);
      AuthorizationRules.AllowWrite(CompanyNameProperty, canWrite);
      AuthorizationRules.AllowWrite(CompanyIdProperty, canWrite);
      AuthorizationRules.AllowWrite(DateAddedProperty, canWrite);
      AuthorizationRules.AllowRead(CompanyNameProperty, canRead);
      AuthorizationRules.AllowRead(CompanyIdProperty, canRead);
      AuthorizationRules.AllowRead(DateAddedProperty, canRead);
      AuthorizationRules.AllowRead(ContactsProperty, canRead);
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, new Csla.Validation.RuleArgs(CompanyNameProperty));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs(CompanyNameProperty, 50));
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
          context.OutArgs.Description = "Error checking for duplicate company name.  " + e.Error.ToString();
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
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("GetCompany", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          command.Parameters.Add(new SqlParameter("@companyID", criteria.Value));
          using (Csla.Data.SafeDataReader reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
          {
            if (reader.Read())
            {
              LoadProperty<int>(CompanyIdProperty, reader.GetInt32("CompanyID"));
              LoadProperty<string>(CompanyNameProperty, reader.GetString("CompanyName"));
              LoadProperty<SmartDate>(DateAddedProperty, reader.GetSmartDate("DateAdded"));
            }
            reader.NextResult();
            LoadProperty<CompanyContactList>(ContactsProperty, CompanyContactList.GetCompanyContactList(reader));
            reader.NextResult();
            int contactId;
            while (reader.Read())
            {
              contactId = reader.GetInt32("CompanyContactId");
              foreach (CompanyContact oneContact in this.Contacts)
              {
                if (oneContact.CompanyContactId == contactId)
                {
                  oneContact.ContactPhones.Add(CompanyContactPhone.GetCompanyContactPhone(reader));
                }
              }
            }
          }
        }
        connection.Close();
      }
    }

    protected void DataPortal_Create()
    {
      LoadProperty<CompanyContactList>(ContactsProperty, CompanyContactList.NewCompanyContactList());
      ValidationRules.CheckRules();
    }


    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_DeleteSelf()
    {
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("CompaniesDelete", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          command.Parameters.Add(new SqlParameter("@companyID", ReadProperty(CompanyIdProperty)));
          command.ExecuteNonQuery();
        }
        connection.Close();
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Insert()
    {
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("CompaniesInsert", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          command.Parameters.Add(new SqlParameter("@companyID", ReadProperty(CompanyIdProperty)));
          command.Parameters["@companyID"].Direction = System.Data.ParameterDirection.Output;
          command.Parameters.Add(new SqlParameter("@companyName", ReadProperty(CompanyNameProperty)));
          command.Parameters.Add(new SqlParameter("@dateAdded", ReadProperty(DateAddedProperty).DBValue));
          command.ExecuteNonQuery();
          LoadProperty(CompanyIdProperty, command.Parameters["@companyID"].Value);
        }
        DataPortal.UpdateChild(ReadProperty(ContactsProperty), this, connection);
        connection.Close();
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Update()
    {
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("CompaniesUpdate", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          command.Parameters.Add(new SqlParameter("@companyID", ReadProperty(CompanyIdProperty)));
          command.Parameters.Add(new SqlParameter("@companyName", ReadProperty(CompanyNameProperty)));
          command.Parameters.Add(new SqlParameter("@dateAdded", ReadProperty(DateAddedProperty).DBValue));
          command.ExecuteNonQuery();
        }
        DataPortal.UpdateChild(ReadProperty(ContactsProperty), this, connection);
        connection.Close();
      }
    }
#endif
  }
}
