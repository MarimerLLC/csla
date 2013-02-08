using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IDashboardDal
  {
    DashboardDto Fetch();
  }
}
