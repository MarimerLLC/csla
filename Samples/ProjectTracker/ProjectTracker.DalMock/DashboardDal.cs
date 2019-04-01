using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using ProjectTracker.DalMock.MockDbTypes;

namespace ProjectTracker.DalMock
{
  public class DashboardDal : IDashboardDal
  {
    public DashboardDto Fetch()
    {
      var result = new DashboardDto();
      // open database context
      result.ProjectCount = MockDb.Projects.Count();
      result.OpenProjectCount = MockDb.Projects.Where(r => r.Ended == null).Count();
      result.ResourceCount = MockDb.Resources.Count();
      return result;
    }
  }
}
