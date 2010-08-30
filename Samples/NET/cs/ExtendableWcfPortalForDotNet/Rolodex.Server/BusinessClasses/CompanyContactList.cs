using System;
using Csla;
using Csla.Serialization;

#if!SILVERLIGHT
using RolodexEF;
#endif

namespace Rolodex.Business.BusinessClasses
{
    [Serializable]
    public class CompanyContactList : BusinessListBase<CompanyContactList, CompanyContact>
    {

        internal Company ParentCompany
        {
            get
            {
                return Parent as Company;
            }
        }

#if SILVERLIGHT
        public CompanyContactList() { }
        protected override void AddNewCore()
        {
          CompanyContact newContact = CompanyContact.NewCompanyContact();
          Add(newContact);
        }
#else
        private CompanyContactList() { }
        protected override CompanyContact AddNewCore()
        {
            CompanyContact newContact = CompanyContact.NewCompanyContact();
            Add(newContact);
            return newContact;
        }
#endif



        internal static CompanyContactList NewCompanyContactList()
        {
            return DataPortal.CreateChild<CompanyContactList>();
        }

        new public void Child_Create()
        {
            base.Child_Create();
        }

#if !SILVERLIGHT

        internal static CompanyContactList GetCompanyContactList(Companies company)
        {
            return DataPortal.FetchChild<CompanyContactList>(company);
        }

        private void Child_Fetch(Companies company)
        {
            RaiseListChangedEvents = false;
            foreach (var item in company.CompanyContacts)
            {
                Add(CompanyContact.GetCompanyContact(item));
            }
            RaiseListChangedEvents = true;
        }

#endif


    }
}
