using System;
using Csla;
using Csla.Serialization;

#if!SILVERLIGHT
using RolodexEF;
#endif

namespace Rolodex.Business.BusinessClasses
{
    [Serializable]
    public class CompanyContactPhoneList : BusinessListBase<CompanyContactPhoneList, CompanyContactPhone>
    {

#if SILVERLIGHT
        public CompanyContactPhoneList() { }
        protected override void AddNewCore()
        {
          CompanyContactPhone newContactPhone = CompanyContactPhone.NewCompanyContactPhone();
          Add(newContactPhone);
        }

#else
        private CompanyContactPhoneList() { }
        protected override object AddNewCore()
        {
            CompanyContactPhone newContactPhone = CompanyContactPhone.NewCompanyContactPhone();
            Add(newContactPhone);
            return newContactPhone;
        }

        internal static CompanyContactPhoneList GetCompanyContactPhoneList(CompanyContacts contact)
        {
            return DataPortal.FetchChild<CompanyContactPhoneList>(contact);
        }

        private void Child_Fetch(CompanyContacts contact)
        {
            RaiseListChangedEvents = false;
            foreach (var item in contact.CompanyContactPhones)
            {
                Add(CompanyContactPhone.GetCompanyContactPhone(item));
            }
            RaiseListChangedEvents = true;
        }
#endif

        internal static CompanyContactPhoneList NewCompanyContactPhoneList()
        {
            return DataPortal.CreateChild<CompanyContactPhoneList>();
        }

        new public void Child_Create()
        {
            base.Child_Create();
        }


    }
}
