using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpUI.ViewModels
{
  public class DashboardViewModel : ViewModel<ProjectTracker.Library.Dashboard>
  {
    protected override async Task<ProjectTracker.Library.Dashboard> DoInitAsync()
    {
      Model = await ProjectTracker.Library.Dashboard.GetDashboardAsync();
      return Model;
    }
  }
}
