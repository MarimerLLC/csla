using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpUI.ViewModel
{
  public class DashboardViewModel : ViewModel<ProjectTracker.Library.Dashboard>
  {
    protected override async Task<ProjectTracker.Library.Dashboard> DoInitAsync()
    {
      var x = await ProjectTracker.Library.Dashboard.GetDashboardAsync();
      return x;
    }
  }
}
