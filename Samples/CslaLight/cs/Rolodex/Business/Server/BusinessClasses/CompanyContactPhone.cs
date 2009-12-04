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

#if !SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
using Csla.Data;
#endif


namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class CompanyContactPhone : BusinessBase<CompanyContactPhone>
  {

#if SILVERLIGHT
    public CompanyContactPhone() { MarkAsChild(); DisableIEditableObject = true; }
#else
    private CompanyContactPhone() { MarkAsChild(); DisableIEditableObject = true; }
#endif

    private static PropertyInfo<int> CompanyContactPhoneIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyId", "Company Contact Phone Id", 0));
    public int CompanyContactPhoneId
    {
      get
      {
        return GetProperty(CompanyContactPhoneIdProperty);
      }
    }

    private static PropertyInfo<int> CompanyContactIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyContactId", "Contact Id", 0));
    public int CompanyContactId
    {
      get
      {
        return GetProperty(CompanyContactIdProperty);
      }
      set
      {
        SetProperty(CompanyContactIdProperty, value);
      }
    }

    private static PropertyInfo<string> PhoneNumberProperty = RegisterProperty<string>(new PropertyInfo<string>("PhoneNumber", "Phone Number", string.Empty));
    public string PhoneNumber
    {
      get
      {
        return GetProperty(PhoneNumberProperty);
      }
      set
      {
        SetProperty(PhoneNumberProperty, value);
      }
    }

    private static PropertyInfo<string> FaxNumberProperty = RegisterProperty<string>(new PropertyInfo<string>("FaxNumber", "Fax Number", string.Empty));
    public string FaxNumber
    {
      get
      {
        return GetProperty(FaxNumberProperty);
      }
      set
      {
        SetProperty(FaxNumberProperty, value);
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

      AuthorizationRules.AllowWrite(PhoneNumberProperty, canWrite);
      AuthorizationRules.AllowWrite(FaxNumberProperty, canWrite);
      AuthorizationRules.AllowWrite(CompanyContactIdProperty, canWrite);

      AuthorizationRules.AllowRead(PhoneNumberProperty, canRead);
      AuthorizationRules.AllowRead(FaxNumberProperty, canRead);
      AuthorizationRules.AllowRead(CompanyContactIdProperty, canRead);
      AuthorizationRules.AllowRead(CompanyContactPhoneIdProperty, canRead);
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, new Csla.Validation.RuleArgs(PhoneNumberProperty));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, new Csla.Validation.RuleArgs(FaxNumberProperty));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs(PhoneNumberProperty, 30));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs(FaxNumberProperty, 50));
    }

    internal static CompanyContactPhone NewCompanyContactPhone()
    {
      CompanyContactPhone returnValue = new CompanyContactPhone();
      returnValue.MarkAsChild();
      returnValue.ValidationRules.CheckRules();
      return returnValue;
    }


#if !SILVERLIGHT

    internal static CompanyContactPhone GetCompanyContactPhone(SafeDataReader reader)
    {
      return DataPortal.FetchChild<CompanyContactPhone>(reader);
    }

    private void Child_Fetch(SafeDataReader reader)
    {
      LoadProperty<int>(CompanyContactPhoneIdProperty, reader.GetInt32("CompanyContactPhoneId"));
      LoadProperty<int>(CompanyContactIdProperty, reader.GetInt32("CompanyContactId"));
      LoadProperty<string>(PhoneNumberProperty, reader.GetString("PhoneNumber"));
      LoadProperty<string>(FaxNumberProperty, reader.GetString("FaxNumber"));
    }

    private void Child_Insert(CompanyContact companyContact, SqlConnection connection)
    {
      InsertUpdate(true, connection, companyContact);
    }

    private void InsertUpdate(bool insert, SqlConnection connection, CompanyContact companyContact)
    {
      using (SqlCommand command = new SqlCommand("CompanyContactPhonesUpdate", connection))
      {
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@companyContactPhoneId", ReadProperty(CompanyContactPhoneIdProperty)));
        if (insert)
        {
          command.Parameters["@companyContactPhoneId"].Direction = System.Data.ParameterDirection.Output;
          LoadProperty<int>(CompanyContactIdProperty, companyContact.CompanyContactId);
          command.CommandText = "CompanyContactPhonesInsert";
        }

        command.Parameters.Add(new SqlParameter("@companyContactId", ReadProperty(CompanyContactIdProperty)));
        command.Parameters.Add(new SqlParameter("@phoneNumber", ReadProperty(PhoneNumberProperty)));
        command.Parameters.Add(new SqlParameter("@faxNumber", ReadProperty(FaxNumberProperty)));

        command.ExecuteNonQuery();
        if (insert)
        {
          LoadProperty(CompanyContactPhoneIdProperty, command.Parameters["@companyContactPhoneId"].Value);
        }
        MarkOld();
      }
    }

    private void Child_Update(CompanyContact companyContact, SqlConnection connection)
    {
      InsertUpdate(false, connection, companyContact);
    }

    private void Child_DeleteSelf(CompanyContact companyContact, SqlConnection connection)
    {

      using (SqlCommand command = new SqlCommand("CompanyContactPhonesDelete", connection))
      {
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@companyContactPhoneId", ReadProperty(CompanyContactPhoneIdProperty)));
        command.ExecuteNonQuery();
      }
    }


#endif

  }
}
