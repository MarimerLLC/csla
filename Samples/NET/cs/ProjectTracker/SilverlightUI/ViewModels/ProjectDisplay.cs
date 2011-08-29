using System;

namespace SilverlightUI.ViewModels
{
  public class ProjectDisplay : ViewModelLocal<ProjectTracker.Library.ProjectInfo>
  {
    public ProjectDisplay(ProjectTracker.Library.ProjectInfo info)
    {
      Model = info;
    }
  }
}
