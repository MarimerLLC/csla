using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinFormsUi.ViewModels
{
  public class DashboardViewModel : ViewModel<ProjectTracker.Library.Dashboard>
  {
    protected override async Task<ProjectTracker.Library.Dashboard> DoInitAsync()
    {
      try
      { 
      Model = await ProjectTracker.Library.Dashboard.GetDashboardAsync();
      }
      catch (Exception ex)
      {
        var x = ex;
      }
      return Model;
    }
  }
}
