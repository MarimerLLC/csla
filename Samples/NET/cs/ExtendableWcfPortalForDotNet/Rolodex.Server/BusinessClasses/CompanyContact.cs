using System;
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
    public class CompanyContact : BusinessBase<CompanyContact>
    {
#if SILVERLIGHT
        public CompanyContact() { }
#else
        private CompanyContact() { }
#endif

        #region Properties
        private static PropertyInfo<int> CompanyContactIdProperty = RegisterProperty(new PropertyInfo<int>("CompanyContactId", "Contact Id", 0));
        public int CompanyContactId
        {
            get
            {
                return GetProperty(CompanyContactIdProperty);
            }
        }

        private static PropertyInfo<int> CompanyIdProperty = RegisterProperty(new PropertyInfo<int>("CompanyId", "Company Id", 0));
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

        private static PropertyInfo<string> FirstNameProperty = RegisterProperty(new PropertyInfo<string>("FirstName", "First Name", string.Empty));
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

        private static PropertyInfo<string> LastNameProperty = RegisterProperty(new PropertyInfo<string>("LastName", "Last Name", string.Empty));
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

        private static PropertyInfo<int> RankIdProperty = RegisterProperty(new PropertyInfo<int>("RankId", "Rank", 0));
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

        private static PropertyInfo<int> InitialRankIdProperty = RegisterProperty(new PropertyInfo<int>("InitialRankId", "Rank", 0));
        public int InitialRankId
        {
            get
            {
                return GetProperty(InitialRankIdProperty);
            }
        }

        private static PropertyInfo<SmartDate> BirthdayProperty = RegisterProperty(new PropertyInfo<SmartDate>("Birthday", "Birthday"));
        public DateTime? Birthday
        {
            get
            {
                return GetProperty(BirthdayProperty).ToNullableDate();
            }
            set
            {
                SetProperty(BirthdayProperty, new SmartDate(value));
            }
        }

        private static PropertyInfo<decimal> BaseSalaryProperty = RegisterProperty(new PropertyInfo<decimal>("BaseSalary", "Base Salary"));
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

        private static PropertyInfo<CompanyContactPhoneList> ContactsPhonesProperty = RegisterProperty(new PropertyInfo<CompanyContactPhoneList>("ContactPhones", "Contact Phones"));
        public CompanyContactPhoneList ContactPhones
        {
            get
            {
                return GetProperty(ContactsPhonesProperty);
            }
        }
        #endregion

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
            AuthorizationRules.AllowCreate(typeof(CompanyContact), admin);
            AuthorizationRules.AllowDelete(typeof(CompanyContact), admin);
            AuthorizationRules.AllowEdit(typeof(CompanyContact), canWrite);
            AuthorizationRules.AllowGet(typeof(CompanyContact), canRead);
        }

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, new RuleArgs(FirstNameProperty));
            ValidationRules.AddRule(CommonRules.StringRequired, new RuleArgs(LastNameProperty));
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(FirstNameProperty, 30));
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(LastNameProperty, 50));
            ValidationRules.AddRule<CompanyContact>(IsDateValid, BirthdayProperty);
            ValidationRules.AddRule(CommonRules.IntegerMinValue, new CommonRules.IntegerMinValueRuleArgs(RankIdProperty, 1));
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

        internal static CompanyContact GetCompanyContact(CompanyContacts contact)
        {
            return DataPortal.FetchChild<CompanyContact>(contact);
        }

        private void Child_Fetch(CompanyContacts contact)
        {
            LoadProperty<int>(CompanyIdProperty, contact.Companies.CompanyId);
            LoadProperty<int>(CompanyContactIdProperty, contact.CompanyContactId);
            LoadProperty<string>(FirstNameProperty, contact.FirstName);
            LoadProperty<string>(LastNameProperty, contact.LastName);
            LoadProperty<SmartDate>(BirthdayProperty, new SmartDate(contact.Birthday));
            LoadProperty<int>(RankIdProperty, contact.Ranks.RankId);
            LoadProperty(InitialRankIdProperty, contact.Ranks.RankId);
            LoadProperty<decimal>(BaseSalaryProperty, contact.BaseSalary);
            LoadProperty(ContactsPhonesProperty, CompanyContactPhoneList.GetCompanyContactPhoneList(contact));

        }

        private RolodexEF.Ranks GetRank(int rankID)
        {
            using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
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
            using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
            {
                LoadProperty(CompanyIdProperty, company.CompanyId);
                RolodexEF.Ranks newRank = GetRank(ReadProperty(RankIdProperty));

                CompanyContacts newContact = new CompanyContacts();
                newContact.BaseSalary = ReadProperty(BaseSalaryProperty);
                SmartDate birthday = ReadProperty(BirthdayProperty);
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
            CompanyContacts entityContact = sender as CompanyContacts;
            if (e.PropertyName == CompanyContactIdProperty.Name)
            {
                LoadProperty(CompanyContactIdProperty, entityContact.CompanyContactId);
                entityContact.PropertyChanged -= entityContact_PropertyChanged;
            }
        }


        private void Child_Update(Company company, Companies entityCompany)
        {

            using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
            {

                RolodexEF.Ranks newRank = GetRank(ReadProperty(RankIdProperty));
                RolodexEF.Ranks oldRank = GetRank(ReadProperty(InitialRankIdProperty));


                CompanyContacts newContact = new CompanyContacts();
                newContact.CompanyContactId = ReadProperty(CompanyContactIdProperty);
                newContact.EntityKey = new System.Data.EntityKey("RolodexEntities.CompanyContacts", "CompanyContactId", ReadProperty(CompanyContactIdProperty));

                manager.ObjectContext.Attach(newContact);
                entityCompany.CompanyContacts.Attach(newContact);
                oldRank.CompanyContacts.Attach(newContact);
                newContact.Ranks = newRank;

                newContact.BaseSalary = ReadProperty(BaseSalaryProperty);
                SmartDate birthday = ReadProperty(BirthdayProperty);
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
            if (!this.IsNew)
            {

                using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
                {
                    CompanyContacts deleted = new CompanyContacts();
                    deleted.CompanyContactId = ReadProperty(CompanyContactIdProperty);
                    deleted.EntityKey = new System.Data.EntityKey("RolodexEntities.CompanyContacts", "CompanyContactId", deleted.CompanyContactId);
                    manager.ObjectContext.Attach(deleted);
                    entityCompany.CompanyContacts.Attach(deleted);
                    manager.ObjectContext.DeleteObject(deleted);

                }
            }
        }

#endif
    }
}
