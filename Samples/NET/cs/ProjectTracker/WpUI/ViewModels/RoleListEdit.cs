
namespace WpUI.ViewModels
{
  public class RoleListEdit : ViewModelEdit<ProjectTracker.Library.Admin.RoleEditList>
  {
    public RoleListEdit()
    {
      BeginRefresh(ProjectTracker.Library.Admin.RoleEditList.GetRoles);
    }

    public override void NavigatingBackTo()
    {
      base.NavigatingBackTo();
      SelectedItem = null;
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
      if (CanEditObject && SelectedItem != null)
        Bxf.Shell.Instance.ShowView("/RoleEdit.xaml", null, 
          new RoleEdit(SelectedItem), null);
    }

    public void AddItem()
    {
      if (CanEditObject)
        Bxf.Shell.Instance.ShowView("/RoleEdit.xaml", null,
          new RoleEdit(Model), null);
    }
  }
}
