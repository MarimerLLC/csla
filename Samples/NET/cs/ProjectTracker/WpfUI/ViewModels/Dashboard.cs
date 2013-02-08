using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfUI.ViewModels
{
  public class Dashboard : ViewModel<ProjectTracker.Library.Dashboard>
  {
    public Dashboard()
    {
      BeginRefresh(ProjectTracker.Library.Dashboard.GetDashboard);
    }
  }
}
