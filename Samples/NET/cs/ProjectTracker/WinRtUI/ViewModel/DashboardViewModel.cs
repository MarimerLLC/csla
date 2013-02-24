using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Xaml;

namespace WinRtUI.ViewModel
{
  public class DashboardViewModel : ViewModel<ProjectTracker.Library.Dashboard>
  {
    protected override async Task<ProjectTracker.Library.Dashboard> DoInitAsync()
    {
      return await ProjectTracker.Library.Dashboard.GetDashboardAsync();
    }
  }
}
