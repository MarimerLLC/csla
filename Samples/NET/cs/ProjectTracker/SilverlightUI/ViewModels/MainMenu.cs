using System;

namespace SilverlightUI.ViewModels
{
  public class MainMenu
  {
    public void ShowProjectList()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(Views.ProjectList).AssemblyQualifiedName,
        "projectListViewSource",
        new ProjectList(),
        "Main");
    }

    public void ShowResourceList()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(Views.ResourceList).AssemblyQualifiedName,
        "resourceListViewSource",
        new ResourceList(),
        "Main");
    }
  }
}
