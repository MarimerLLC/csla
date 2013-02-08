using System;
using Csla;
using Csla.Security;
using Csla.Serialization;

#if!SILVERLIGHT
using RolodexEF;
#endif

namespace Rolodex.Business.BusinessClasses
{
    [Serializable]
    public class ReadOnlyCompany : BusinessBase<ReadOnlyCompany>
    {

#if SILVERLIGHT
    public ReadOnlyCompany() { }
#else
        private ReadOnlyCompany() { }
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
        }

        protected override void AddAuthorizationRules()
        {
            string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
            FieldManager.GetRegisteredProperties().ForEach(item => AuthorizationRules.AllowRead(item, canRead));
        }

        public static void AddObjectAuthorizationRules()
        {
            string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
            AuthorizationRules.AllowGet(typeof(Company), canRead);
        }

#if !SILVERLIGHT

        public static ReadOnlyCompany GetReadOnlyCompany(int companyId, string companyName)
        {
            return DataPortal.FetchChild<ReadOnlyCompany>(companyId, companyName);
        }

        private void Child_Fetch(int companyId, string companyName)
        {
            LoadProperty<int>(CompanyIdProperty, companyId);
            LoadProperty<string>(CompanyNameProperty, companyName);
        }


#endif
    }
}
