using System;
using Csla;
using ProjectTracker.Dal;

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

    public async static System.Threading.Tasks.Task<Dashboard> GetDashboardAsync()
    {
      return await DataPortal.FetchAsync<Dashboard>();
    }

    [Fetch]
    private void Fetch([Inject] IDashboardDal dal)
    {
      var data = dal.Fetch();
      ProjectCount = data.ProjectCount;
      OpenProjectCount = data.OpenProjectCount;
      ResourceCount = data.ResourceCount;
    }
  }
}
