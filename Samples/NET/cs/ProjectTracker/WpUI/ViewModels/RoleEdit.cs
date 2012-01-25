
namespace WpUI.ViewModels
{
  public class RoleEdit : ViewModelLocalEdit<ProjectTracker.Library.Admin.RoleEdit>
  {
    public bool EditMode { get; set; }
    public ProjectTracker.Library.Admin.RoleEditList ParentList { get; set; }

    public RoleEdit(ProjectTracker.Library.Admin.RoleEditList parent)
    {
      ParentList = parent;
      EditMode = false;
      ManageObjectLifetime = true;
      Model = new ProjectTracker.Library.Admin.RoleEdit();
    }

    public RoleEdit(ProjectTracker.Library.Admin.RoleEdit role)
    {
      ParentList = (ProjectTracker.Library.Admin.RoleEditList)role.Parent;
      EditMode = true;
      ManageObjectLifetime = true;
      Model = role;
    }

    public void Accept()
    {
      if (Model != null)
      {
        Model.ApplyEdit();
        if (!EditMode)
          ParentList.Add(Model);
      }
      Model = null;
      Close();
    }

    public void Close()
    {
      Bxf.Shell.Instance.ShowView(null, null);
    }

    public override void NavigatedAway()
    {
      if (Model != null)
        Model.CancelEdit();
    }

    public void Remove()
    {
      if (Model != null)
        ParentList.Remove(Model);
      Model = null;
      Close();
    }
  }
}
