using System;
using System.Linq;
using Csla;
using Csla.Data;
using Rolodex.Business.Data;
using RolodexEF;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class ReadOnlyCompanyList : BusinessListBase<ReadOnlyCompanyList, ReadOnlyCompany>
  {
    public static void GetCompanyList(EventHandler<DataPortalResult<ReadOnlyCompanyList>> handler)
    {
      DataPortal<ReadOnlyCompanyList> dp = new DataPortal<ReadOnlyCompanyList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }

    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      //IsReadOnly = false;
      using (
        ObjectContextManager<RolodexEntities> manager =
          ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        foreach (var item in (from oneCompany in manager.ObjectContext.Companies
          orderby oneCompany.CompanyName
          select new {oneCompany.CompanyId, oneCompany.CompanyName}))
        {
          Add(ReadOnlyCompany.GetReadOnlyCompany(item.CompanyId, item.CompanyName));
        }
      }
      //IsReadOnly = true;
      RaiseListChangedEvents = true;
    }
  }
}