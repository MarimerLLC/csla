
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

    public RoleEdit(ProjectTracker.Library.Admin.RoleEditList parent, ProjectTracker.Library.Admin.RoleEdit role)
    {
      ParentList = parent;
      EditMode = true;
      ManageObjectLifetime = true;
      Model = role;
    }

    public new void Save()
    {
      if (Model != null)
      {
        Model.ApplyEdit();
        if (!EditMode)
          ParentList.Add(Model);
      }
      Bxf.Shell.Instance.ShowView(null, null);
    }

    public new void Cancel()
    {
      if (Model != null)
        Model.CancelEdit();
      Bxf.Shell.Instance.ShowView(null, null);
    }

    internal void Remove()
    {
      if (Model != null)
        ParentList.Remove(Model);
      Bxf.Shell.Instance.ShowView(null, null);
    }
  }
}
