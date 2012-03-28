using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable]
  public class Dashboard : ReadOnlyBase<Dashboard>
  {
    public static readonly PropertyInfo<int> ProjectCountProperty = RegisterProperty<int>(c => c.ProjectCount);
    public int ProjectCount
    {
      get { return GetProperty(ProjectCountProperty); }
      private set { LoadProperty(ProjectCountProperty, value); }
    }

    public static readonly PropertyInfo<int> OpenProjectCountProperty = RegisterProperty<int>(c => c.OpenProjectCount);
    public int OpenProjectCount
    {
      get { return GetProperty(OpenProjectCountProperty); }
      private set { LoadProperty(OpenProjectCountProperty, value); }
    }

    public static readonly PropertyInfo<int> ResourceCountProperty = RegisterProperty<int>(c => c.ResourceCount);
    public int ResourceCount
    {
      get { return GetProperty(ResourceCountProperty); }
      private set { LoadProperty(ResourceCountProperty, value); }
    }


    public static void GetDashboard(
      EventHandler<DataPortalResult<Dashboard>> callback)
    {
      DataPortal.BeginFetch<Dashboard>(callback);
    }
#if WINRT
    public async static Dashboard GetDashboardAsync()
    {
      return await DataPortal.FetchAsync<Dashboard>();
    }
#endif
#if !SILVERLIGHT
    public static Dashboard GetDashboard()
    {
      return DataPortal.Fetch<Dashboard>();
    }

    private void DataPortal_Fetch()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IDashboardDal>();
        var data = dal.Fetch();
        ProjectCount = data.ProjectCount;
        OpenProjectCount = data.OpenProjectCount;
        ResourceCount = data.ResourceCount;
      }
    }
#endif
  }
}
