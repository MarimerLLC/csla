using System;

namespace WpfUI.ViewModels
{
  public class RoleListEdit : ViewModelEdit<ProjectTracker.Library.Admin.RoleEditList>
  {
    public RoleListEdit()
    {
      var task = RefreshAsync<ProjectTracker.Library.Admin.RoleEditList>(
        async () => await ProjectTracker.Library.Admin.RoleEditList.GetRolesAsync());
    }
  }
}
