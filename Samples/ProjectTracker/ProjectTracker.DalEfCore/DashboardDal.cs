using System.Linq;
using ProjectTracker.Dal;

namespace ProjectTracker.DalEfCore
{
  public class DashboardDal : IDashboardDal
  {
    private readonly PTrackerContext db;

    public DashboardDal(PTrackerContext context)
    {
      db = context;
    }

    public DashboardDto Fetch()
    {
      var result = new DashboardDto();
      result.ProjectCount = db.Projects.Count();
      result.OpenProjectCount = db.Projects.Where(r => r.Ended == null).Count();
      result.ResourceCount = db.Resources.Count();
      return result;
    }
  }
}
