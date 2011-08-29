using System;

namespace SilverlightUI.ViewModels
{
  public class ProjectEdit : ViewModelEdit<ProjectTracker.Library.ProjectEdit>
  {
    public ProjectEdit()
    {
      BeginRefresh(callback => ProjectTracker.Library.ProjectEdit.NewProject(callback));
    }

    public ProjectEdit(ProjectTracker.Library.ProjectInfo info)
    {
      BeginRefresh(callback => ProjectTracker.Library.ProjectEdit.GetProject(info.Id, callback));
    }
  }
}
