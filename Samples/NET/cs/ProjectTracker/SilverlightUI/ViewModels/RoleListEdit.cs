using System;

namespace SilverlightUI.ViewModels
{
  public class RoleListEdit : ViewModelEdit<ProjectTracker.Library.Admin.RoleEditList>
  {
    public RoleListEdit()
    {
      BeginRefresh(ProjectTracker.Library.Admin.RoleEditList.GetRoles);
    }

    public override bool CanAddNew
    {
      get { return Csla.Rules.BusinessRules.HasPermission(
                     Csla.Rules.AuthorizationActions.CreateObject, 
                     typeof(ProjectTracker.Library.Admin.RoleEditList));
      }
    }

    public void AddNew()
    {
      Model.AddNew();
    }

    public override bool CanRemove
    {
      get { return Csla.Rules.BusinessRules.HasPermission(
                     Csla.Rules.AuthorizationActions.DeleteObject, 
                     typeof(ProjectTracker.Library.Admin.RoleEditList));
      }
    }

    public void Remove(object sender, Bxf.Xaml.ExecuteEventArgs e)
    {
      if (e.MethodParameter != null)
        Model.Remove((ProjectTracker.Library.Admin.RoleEdit)e.MethodParameter);
    }
  }
}
