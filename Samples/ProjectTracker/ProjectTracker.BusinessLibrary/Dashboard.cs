using Csla;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class Dashboard : ReadOnlyBase<Dashboard>
  {
    public partial int ProjectCount { get; private set; }

    public partial int OpenProjectCount { get; private set; }

    public partial int ResourceCount { get; private set; }

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
