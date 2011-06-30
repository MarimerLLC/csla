using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfUI.ViewModels
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
