using System;
using Csla;
using Csla.Security;
using Csla.Serialization;

#if !SILVERLIGHT
using Rolodex.Business.Data;
using Csla.Data;
using RolodexEF;
#endif


namespace Rolodex.Business.BusinessClasses
{
    [Serializable]
    public class CompanyContactPhone : BusinessBase<CompanyContactPhone>
    {

#if SILVERLIGHT
        public CompanyContactPhone() { }
#else
        private CompanyContactPhone() { }
#endif

        private static PropertyInfo<int> CompanyContactPhoneIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyContactPhoneId", "Company Contact Phone Id", 0));
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
            AuthorizationRules.AllowCreate(typeof(CompanyContactPhone), admin);
            AuthorizationRules.AllowDelete(typeof(CompanyContactPhone), admin);
            AuthorizationRules.AllowEdit(typeof(CompanyContactPhone), canWrite);
            AuthorizationRules.AllowGet(typeof(CompanyContactPhone), canRead);
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
            return DataPortal.CreateChild<CompanyContactPhone>();
        }

        new public void Child_Create()
        {
            base.Child_Create();
        }


#if !SILVERLIGHT

        internal static CompanyContactPhone GetCompanyContactPhone(CompanyContactPhones phone)
        {
            return DataPortal.FetchChild<CompanyContactPhone>(phone);
        }

        private void Child_Fetch(CompanyContactPhones phone)
        {
            LoadProperty<int>(CompanyContactPhoneIdProperty, phone.CompanyContactPhoneId);
            LoadProperty<int>(CompanyContactIdProperty, phone.CompanyContacts.CompanyContactId);
            LoadProperty<string>(PhoneNumberProperty, phone.PhoneNumber);
            LoadProperty<string>(FaxNumberProperty, phone.FaxNumber);
        }

        private void Child_Insert(CompanyContact companyContact, CompanyContacts entityContact)
        {
            using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
            {
                LoadProperty(CompanyContactIdProperty, companyContact.CompanyContactId);
                CompanyContactPhones newContactPhone = new CompanyContactPhones();
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
            CompanyContacts entityContact = sender as CompanyContacts;
            if (e.PropertyName == CompanyContactIdProperty.Name)
            {
                LoadProperty(CompanyContactIdProperty, entityContact.CompanyContactId);
                entityContact.PropertyChanged -= entityContact_PropertyChanged;
            }
        }

        void newContactPhone_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CompanyContactPhones phone = sender as CompanyContactPhones;
            if (e.PropertyName == CompanyContactPhoneIdProperty.Name)
            {
                LoadProperty(CompanyContactPhoneIdProperty, phone.CompanyContactPhoneId);
                phone.PropertyChanged -= newContactPhone_PropertyChanged;
            }
        }



        private void Child_Update(CompanyContact companyContact, CompanyContacts entityContact)
        {
            using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
            {
                CompanyContactPhones newContactPhone = new CompanyContactPhones();
                newContactPhone.CompanyContactPhoneId = ReadProperty(CompanyContactPhoneIdProperty);
                newContactPhone.EntityKey = new System.Data.EntityKey("RolodexEntities.CompanyContactPhones", "CompanyContactPhoneId", ReadProperty(CompanyContactPhoneIdProperty));
                manager.ObjectContext.Attach(newContactPhone);
                entityContact.CompanyContactPhones.Attach(newContactPhone);

                newContactPhone.FaxNumber = ReadProperty(FaxNumberProperty);
                newContactPhone.PhoneNumber = ReadProperty(PhoneNumberProperty);

            }
        }

        private void Child_DeleteSelf(CompanyContact companyContact, CompanyContacts entityContact)
        {
            if (!this.IsNew)
            {

                using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
                {
                    CompanyContactPhones deleted = new CompanyContactPhones();
                    deleted.CompanyContactPhoneId = ReadProperty(CompanyContactPhoneIdProperty);
                    deleted.EntityKey = new System.Data.EntityKey("RolodexEntities.CompanyContactPhones", "CompanyContactPhoneId", deleted.CompanyContactPhoneId);
                    manager.ObjectContext.Attach(deleted);
                    entityContact.CompanyContactPhones.Attach(deleted);
                    manager.ObjectContext.DeleteObject(deleted);

                }
            }

        }


#endif

    }
}
