using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public class DashboardDto
  {
    public int ProjectCount { get; set; }
    public int OpenProjectCount { get; set; }
    public int ResourceCount { get; set; }
  }
}
