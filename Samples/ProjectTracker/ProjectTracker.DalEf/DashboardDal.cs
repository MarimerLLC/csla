using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using Csla.Data;
using System.Data;

namespace ProjectTracker.DalEf
{
  public class DashboardDal : IDashboardDal
  {
    public DashboardDto Fetch()
    {
      var result = new DashboardDto();
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        result.ProjectCount = ctx.ObjectContext.Projects.Count();
        result.OpenProjectCount = ctx.ObjectContext.Projects.Where(r => r.Ended == null).Count();
        result.ResourceCount = ctx.ObjectContext.Resources.Count();
      }
      return result;
    }
  }
}
