using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfUI.ViewModels
{
  public class ProjectList : ViewModel<ProjectTracker.Library.ProjectList>
  {
    public ProjectList()
    {
      BeginRefresh(ProjectTracker.Library.ProjectList.GetProjectList);
    }
  }
}
