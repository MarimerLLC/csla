
namespace WpUI.ViewModels
{
  public class RoleListEdit : ViewModelEdit<ProjectTracker.Library.Admin.RoleEditList>
  {
    public RoleListEdit()
    {
      BeginRefresh(ProjectTracker.Library.Admin.RoleEditList.GetRoles);
    }

    protected override void OnSaved()
    {
      base.OnSaved();
      Bxf.Shell.Instance.ShowView(null, null);
    }

    private ProjectTracker.Library.Admin.RoleEdit _selectedItem;
    public ProjectTracker.Library.Admin.RoleEdit SelectedItem
    {
      get { return _selectedItem; }
      set 
      { 
        _selectedItem = value; 
        OnPropertyChanged("SelectedItem");
        EditItem();
      }
    }

    public void EditItem()
    {
      if (CanEditObject)
        Bxf.Shell.Instance.ShowView("/RoleEdit.xaml", null, 
          new RoleEdit(Model, SelectedItem), null);
    }

    public void AddItem()
    {
      if (CanEditObject)
        Bxf.Shell.Instance.ShowView("/RoleEdit.xaml", null,
          new RoleEdit(Model), null);
    }
  }
}
