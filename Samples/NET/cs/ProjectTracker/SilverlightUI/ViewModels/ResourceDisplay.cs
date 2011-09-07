using System;

namespace SilverlightUI.ViewModels
{
  public class ResourceDisplay : ViewModelLocal<ProjectTracker.Library.ResourceInfo>
  {
    public ResourceDisplay(ProjectTracker.Library.ResourceInfo info)
    {
      Model = info;
    }
  }
}
