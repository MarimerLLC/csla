using System;
using System.Threading.Tasks;
using Csla;

namespace WpfUI.ViewModels
{
  public class Dashboard : ViewModel<ProjectTracker.Library.Dashboard>
  {
    public Dashboard()
    {
      var task = RefreshAsync<ProjectTracker.Library.Dashboard>(async () =>
        await ProjectTracker.Library.Dashboard.GetDashboardAsync());
    }
  }
}
