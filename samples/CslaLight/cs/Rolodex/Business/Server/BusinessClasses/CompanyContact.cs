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


#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
using Csla.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class CompanyContact : BusinessBase<CompanyContact>
  {
#if SILVERLIGHT
    public CompanyContact() { MarkAsChild(); DisableIEditableObject = true; }
#else
    private CompanyContact() { MarkAsChild(); DisableIEditableObject = true; }
#endif

    private static PropertyInfo<int> CompanyContactIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyContactId", "Contact Id", 0));
    public int CompanyContactId
    {
      get
      {
        return GetProperty(CompanyContactIdProperty);
      }
    }

    private static PropertyInfo<int> CompanyIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyId", "Company Id", 0));
    public int CompanyId
    {
      get
      {
        return GetProperty(CompanyIdProperty);
      }
      set
      {
        SetProperty(CompanyIdProperty, value);
      }
    }

    private static PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(new PropertyInfo<string>("FirstName", "First Name", string.Empty));
    public string FirstName
    {
      get
      {
        return GetProperty(FirstNameProperty);
      }
      set
      {
        SetProperty(FirstNameProperty, value);
      }
    }

    private static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(new PropertyInfo<string>("LastName", "Last Name", string.Empty));
    public string LastName
    {
      get
      {
        return GetProperty(LastNameProperty);
      }
      set
      {
        SetProperty(LastNameProperty, value);
      }
    }

    private static PropertyInfo<int> RankIdProperty = RegisterProperty<int>(new PropertyInfo<int>("RankId", "Rank", 0));
    public int RankId
    {
      get
      {
        return GetProperty(RankIdProperty);
      }
      set
      {
        SetProperty(RankIdProperty, value);
      }
    }

    private static PropertyInfo<SmartDate> BirthdayProperty = RegisterProperty<SmartDate>(new PropertyInfo<SmartDate>("Birthday", "Birthday"));
    public string Birthday
    {
      get
      {
        return GetProperty(BirthdayProperty).Text;
      }
      set
      {
        SmartDate test = new SmartDate();
        if (SmartDate.TryParse(value, ref test) == true)
        {
          SetProperty(BirthdayProperty, test);
        }
      }
    }

    private static PropertyInfo<decimal> BaseSalaryProperty = RegisterProperty<decimal>(new PropertyInfo<decimal>("BaseSalary", "Base Salary", 0));
    public decimal BaseSalary
    {
      get
      {
        return GetProperty(BaseSalaryProperty);
      }
      set
      {
        SetProperty(BaseSalaryProperty, value);
      }
    }

    private static PropertyInfo<CompanyContactPhoneList> ContactsPhonesProperty = RegisterProperty<CompanyContactPhoneList>(new PropertyInfo<CompanyContactPhoneList>("ContactPhones", "Contact Phones"));
    public CompanyContactPhoneList ContactPhones
    {
      get
      {
        return GetProperty(ContactsPhonesProperty);
      }
    }

    protected override void AddAuthorizationRules()
    {
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      string[] admin = new string[] { "AdminUser" };

      AuthorizationRules.AllowCreate(typeof(CompanyContact), admin);
      AuthorizationRules.AllowDelete(typeof(CompanyContact), admin);
      AuthorizationRules.AllowEdit(typeof(CompanyContact), canWrite);
      AuthorizationRules.AllowGet(typeof(CompanyContact), canRead);

      AuthorizationRules.AllowWrite(FirstNameProperty, canWrite);
      AuthorizationRules.AllowWrite(LastNameProperty, canWrite);
      AuthorizationRules.AllowWrite(CompanyContactIdProperty, canWrite);
      AuthorizationRules.AllowWrite(CompanyIdProperty, canWrite);
      AuthorizationRules.AllowWrite(RankIdProperty, canWrite);
      AuthorizationRules.AllowWrite(BirthdayProperty, canWrite);
      AuthorizationRules.AllowWrite(BaseSalaryProperty, canWrite);

      AuthorizationRules.AllowRead(FirstNameProperty, canRead);
      AuthorizationRules.AllowRead(LastNameProperty, canRead);
      AuthorizationRules.AllowRead(CompanyContactIdProperty, canRead);
      AuthorizationRules.AllowRead(CompanyIdProperty, canRead);
      AuthorizationRules.AllowRead(RankIdProperty, canRead);
      AuthorizationRules.AllowRead(BirthdayProperty, canRead);
      AuthorizationRules.AllowRead(BaseSalaryProperty, canRead);
      AuthorizationRules.AllowRead(ContactsPhonesProperty, canRead);
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, new Csla.Validation.RuleArgs(FirstNameProperty));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, new Csla.Validation.RuleArgs(LastNameProperty));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs(FirstNameProperty, 30));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs(LastNameProperty, 50));
      ValidationRules.AddRule<CompanyContact>(IsDateValid, BirthdayProperty);
      ValidationRules.AddRule(Csla.Validation.CommonRules.IntegerMinValue, new Csla.Validation.CommonRules.IntegerMinValueRuleArgs(RankIdProperty, 1));
    }

    private static bool IsDateValid(CompanyContact target, RuleArgs e)
    {
      SmartDate dateAdded = target.GetProperty(BirthdayProperty);
      if (!dateAdded.IsEmpty)
      {
        if (dateAdded.Date < (new DateTime(1900, 1, 1)))
        {
          e.Description = "Date must be greater that 1/1/1900!";
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

    internal static CompanyContact NewCompanyContact()
    {
      CompanyContact newContact = new CompanyContact();
      newContact.LoadProperty(ContactsPhonesProperty, CompanyContactPhoneList.NewCompanyContactPhoneList());
      newContact.MarkAsChild();
      newContact.ValidationRules.CheckRules();
      return newContact;
    }


#if !SILVERLIGHT

    internal static CompanyContact GetCompanyContact(SafeDataReader reader)
    {
      return DataPortal.FetchChild<CompanyContact>(reader);
    }

    private void Child_Fetch(SafeDataReader reader)
    {
      LoadProperty<int>(CompanyIdProperty, reader.GetInt32("CompanyId"));
      LoadProperty<int>(CompanyContactIdProperty, reader.GetInt32("CompanyContactId"));
      LoadProperty<string>(FirstNameProperty, reader.GetString("FirstName"));
      LoadProperty<string>(LastNameProperty, reader.GetString("LastName"));
      LoadProperty<SmartDate>(BirthdayProperty, reader.GetSmartDate("Birthday"));
      LoadProperty<int>(RankIdProperty, reader.GetInt32("RankId"));
      LoadProperty<decimal>(BaseSalaryProperty, reader.GetDecimal("BaseSalary"));
      LoadProperty(ContactsPhonesProperty, CompanyContactPhoneList.NewCompanyContactPhoneList());
    }

    private void Child_Insert(Company company, SqlConnection connection)
    {
      InsertUpdate(true, connection, company);
    }

    private void InsertUpdate(bool insert, SqlConnection connection, Company company)
    {
      using (SqlCommand command = new SqlCommand("CompanyContactsUpdate", connection))
      {
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@companyContactId", ReadProperty(CompanyContactIdProperty)));
        if (insert)
        {
          command.Parameters["@companyContactId"].Direction = System.Data.ParameterDirection.Output;
          LoadProperty<int>(CompanyIdProperty, company.CompanyId);
          command.CommandText = "CompanyContactsInsert";
        }

        command.Parameters.Add(new SqlParameter("@companyId", ReadProperty(CompanyIdProperty)));
        command.Parameters.Add(new SqlParameter("@firstName", ReadProperty(FirstNameProperty)));
        command.Parameters.Add(new SqlParameter("@lastName", ReadProperty(LastNameProperty)));
        command.Parameters.Add(new SqlParameter("@baseSalary", ReadProperty(BaseSalaryProperty)));
        command.Parameters.Add(new SqlParameter("@rankId", ReadProperty(RankIdProperty)));
        command.Parameters.Add(new SqlParameter("@birthday", ReadProperty(BirthdayProperty).DBValue));

        command.ExecuteNonQuery();
        if (insert)
        {
          LoadProperty(CompanyContactIdProperty, command.Parameters["@companyContactId"].Value);
        }
        DataPortal.UpdateChild(ReadProperty(ContactsPhonesProperty), this, connection);
        MarkOld();
      }
    }

    private void Child_Update(Company company, SqlConnection connection)
    {
      InsertUpdate(false, connection, company);
    }

    private void Child_DeleteSelf(Company company, SqlConnection connection)
    {
      using (SqlCommand command = new SqlCommand("CompanyContactsDelete", connection))
      {
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@companyContactId", ReadProperty(CompanyContactIdProperty)));
        command.ExecuteNonQuery();
      }
    }

#endif
  }
}
