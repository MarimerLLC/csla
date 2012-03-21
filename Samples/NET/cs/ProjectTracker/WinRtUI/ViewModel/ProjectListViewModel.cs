using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Xaml;
using Windows.UI.Popups;

namespace WinRtUI.ViewModel
{
  public class ProjectListViewModel : ViewModel<ProjectTracker.Library.ProjectList>
  {
    public ProjectListViewModel()
    {
      BeginRefresh(ProjectTracker.Library.ProjectList.GetProjectList);
    }
  }
}
