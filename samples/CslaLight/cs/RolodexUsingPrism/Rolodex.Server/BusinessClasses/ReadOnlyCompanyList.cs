using System;
using Csla;
using Csla.Serialization;
using System.Linq;

#if!SILVERLIGHT
using Csla.Data;
using Rolodex.Business.Data;
using RolodexEF;
#endif

namespace Rolodex.Business.BusinessClasses
{
    [Serializable]
    public class ReadOnlyCompanyList : ReadOnlyListBase<ReadOnlyCompanyList, ReadOnlyCompany>
    {
#if SILVERLIGHT
    public ReadOnlyCompanyList() { }
#else
        private ReadOnlyCompanyList() { }
#endif

        public static void GetCompanyList(EventHandler<DataPortalResult<ReadOnlyCompanyList>> handler)
        {
            DataPortal<ReadOnlyCompanyList> dp = new DataPortal<ReadOnlyCompanyList>();
            dp.FetchCompleted += handler;
            dp.BeginFetch();
        }

#if !SILVERLIGHT

        private void DataPortal_Fetch()
        {
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
            {
                foreach (var item in (from oneCompany in manager.ObjectContext.Companies
                                      orderby oneCompany.CompanyName
                                      select new { oneCompany.CompanyId, oneCompany.CompanyName }))
                {
                    Add(ReadOnlyCompany.GetReadOnlyCompany(item.CompanyId, item.CompanyName));
                }
            }
            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

#endif
    }
}
