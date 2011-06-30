using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfUI.ViewModels
{
  public class ResourceList : ViewModel<ProjectTracker.Library.ResourceList>
  {
    public ResourceList()
    {
      BeginRefresh(ProjectTracker.Library.ResourceList.GetResourceList);
    }
  }
}
