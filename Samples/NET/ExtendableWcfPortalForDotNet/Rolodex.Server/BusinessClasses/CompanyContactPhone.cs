using System;
using Csla;
using Csla.Data;
using Csla.Rules;
using Csla.Rules.CommonRules;
using Rolodex.Business.Data;
using RolodexEF;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class CompanyContactPhone : BusinessBase<CompanyContactPhone>
  {
    public static readonly PropertyInfo<int> CompanyContactPhoneIdProperty =
      RegisterProperty<int>(new PropertyInfo<int>("CompanyContactPhoneId", "Company Contact Phone Id", 0));

    public int CompanyContactPhoneId
    {
      get { return GetProperty(CompanyContactPhoneIdProperty); }
    }

    public static readonly PropertyInfo<int> CompanyContactIdProperty =
      RegisterProperty<int>(new PropertyInfo<int>("CompanyContactId", "Contact Id", 0));

    public int CompanyContactId
    {
      get { return GetProperty(CompanyContactIdProperty); }
      set { SetProperty(CompanyContactIdProperty, value); }
    }

    public static readonly PropertyInfo<string> PhoneNumberProperty =
      RegisterProperty<string>(new PropertyInfo<string>("PhoneNumber", "Phone Number", string.Empty));

    public string PhoneNumber
    {
      get { return GetProperty(PhoneNumberProperty); }
      set { SetProperty(PhoneNumberProperty, value); }
    }

    public static readonly PropertyInfo<string> FaxNumberProperty =
      RegisterProperty<string>(new PropertyInfo<string>("FaxNumber", "Fax Number", string.Empty));

    public string FaxNumber
    {
      get { return GetProperty(FaxNumberProperty); }
      set { SetProperty(FaxNumberProperty, value); }
    }

    public static void AddObjectAuthorizationRules()
    {
      var canWrite = new[] {"AdminUser", "RegularUser"};
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};
      var admin = new[] {"AdminUser"};

      BusinessRules.AddRule(typeof(CompanyContactPhone), new IsInRole(AuthorizationActions.CreateObject, admin));
      BusinessRules.AddRule(typeof(CompanyContactPhone), new IsInRole(AuthorizationActions.DeleteObject, admin));
      BusinessRules.AddRule(typeof(CompanyContactPhone), new IsInRole(AuthorizationActions.EditObject, canWrite));
      BusinessRules.AddRule(typeof(CompanyContactPhone), new IsInRole(AuthorizationActions.GetObject, canRead));
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Required(PhoneNumberProperty));
      BusinessRules.AddRule(new Required(FaxNumberProperty));
      BusinessRules.AddRule(new MaxLength(PhoneNumberProperty, 30));
      BusinessRules.AddRule(new MaxLength(FaxNumberProperty, 50));

      var canWrite = new[] {"AdminUser", "RegularUser"};
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};

      FieldManager.GetRegisteredProperties().ForEach(item =>
      {
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, item, canWrite));
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, item, canRead));
      });
    }

    internal static CompanyContactPhone NewCompanyContactPhone()
    {
      return DataPortal.CreateChild<CompanyContactPhone>();
    }

    internal static CompanyContactPhone GetCompanyContactPhone(CompanyContactPhones phone)
    {
      return DataPortal.FetchChild<CompanyContactPhone>(phone);
    }

    private void Child_Fetch(CompanyContactPhones phone)
    {
      LoadProperty(CompanyContactPhoneIdProperty, phone.CompanyContactPhoneId);
      LoadProperty(CompanyContactIdProperty, phone.CompanyContacts.CompanyContactId);
      LoadProperty(PhoneNumberProperty, phone.PhoneNumber);
      LoadProperty(FaxNumberProperty, phone.FaxNumber);
    }

    private void Child_Insert(CompanyContact companyContact, CompanyContacts entityContact)
    {
      using (var manager =
        ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        LoadProperty(CompanyContactIdProperty, companyContact.CompanyContactId);
        var newContactPhone = new CompanyContactPhones();
        newContactPhone.FaxNumber = ReadProperty(FaxNumberProperty);
        newContactPhone.PhoneNumber = ReadProperty(PhoneNumberProperty);
        newContactPhone.CompanyContacts = entityContact;
        manager.ObjectContext.AddToCompanyContactPhones(newContactPhone);
        newContactPhone.PropertyChanged += newContactPhone_PropertyChanged;
        if (ReadProperty(CompanyContactIdProperty) == 0)
          entityContact.PropertyChanged += entityContact_PropertyChanged;
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

    void newContactPhone_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      var phone = sender as CompanyContactPhones;
      if (e.PropertyName == CompanyContactPhoneIdProperty.Name)
      {
        LoadProperty(CompanyContactPhoneIdProperty, phone.CompanyContactPhoneId);
        phone.PropertyChanged -= newContactPhone_PropertyChanged;
      }
    }

    private void Child_Update(CompanyContact companyContact, CompanyContacts entityContact)
    {
      using (var manager =
        ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        var newContactPhone = new CompanyContactPhones();
        newContactPhone.CompanyContactPhoneId = ReadProperty(CompanyContactPhoneIdProperty);
        newContactPhone.EntityKey = new System.Data.EntityKey("RolodexEntities.CompanyContactPhones",
          "CompanyContactPhoneId", ReadProperty(CompanyContactPhoneIdProperty));
        manager.ObjectContext.Attach(newContactPhone);
        entityContact.CompanyContactPhones.Attach(newContactPhone);

        newContactPhone.FaxNumber = ReadProperty(FaxNumberProperty);
        newContactPhone.PhoneNumber = ReadProperty(PhoneNumberProperty);
      }
    }

    private void Child_DeleteSelf(CompanyContact companyContact, CompanyContacts entityContact)
    {
      if (!IsNew)
      {
        using (var manager =
          ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
        {
          var deleted = new CompanyContactPhones();
          deleted.CompanyContactPhoneId = ReadProperty(CompanyContactPhoneIdProperty);
          deleted.EntityKey = new System.Data.EntityKey("RolodexEntities.CompanyContactPhones", "CompanyContactPhoneId",
            deleted.CompanyContactPhoneId);
          manager.ObjectContext.Attach(deleted);
          entityContact.CompanyContactPhones.Attach(deleted);
          manager.ObjectContext.DeleteObject(deleted);
        }
      }
    }
  }
}