using System;

namespace WpfUI.ViewModels
{
  public class RoleListEdit : ViewModelEdit<ProjectTracker.Library.Admin.RoleEditList>
  {
    public RoleListEdit()
    {
      BeginRefresh(ProjectTracker.Library.Admin.RoleEditList.GetRoles);
    }
  }
}
