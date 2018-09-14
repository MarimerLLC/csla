using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfUI.ViewModels
{
  public class MainMenu
  {
    private ObservableCollection<MenuItem> _menuItems;
    public ObservableCollection<MenuItem> MenuItems
    {
      get { return _menuItems; }
    }

    public MainMenu()
    {
      try
      {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        ProjectTracker.Library.RoleList.CacheListAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      }
      catch (Exception ex)
      {
        Bxf.Shell.Instance.ShowError(ex.Message, "Retrieve RoleList");
      }

      _menuItems = new ObservableCollection<MenuItem>
      {
        new MenuItem { 
          Label = "Project List", MethodName = "ShowProjectList", Method = ShowProjectList, 
          IsAuthorized = Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.GetObject, typeof(ProjectTracker.Library.ProjectList)) },
        new MenuItem { 
          Label = "Resource List", MethodName = "ShowResourceList", Method = ShowResourceList, 
          IsAuthorized = Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.GetObject, typeof(ProjectTracker.Library.ResourceList)) },
        new MenuItem { 
          Label = "Edit roles", MethodName = "ShowRoleListEdit", Method = ShowRoleListEdit, 
          IsAuthorized = Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectTracker.Library.Admin.RoleEditList)) },
      };
    }

    public void MenuItemSelected(object sender, Bxf.Xaml.ExecuteEventArgs e)
    {
      var listbox = (ListBox)e.TriggerSource;
      var menuItem = (MenuItem)listbox.SelectedItem;
      if (menuItem.IsAuthorized)
        menuItem.Method();
      else
        Bxf.Shell.Instance.ShowError("You are not authorized to perform this action", "Authorization error");
    }

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

    public void ShowRoleListEdit()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(Views.RoleListEdit).AssemblyQualifiedName,
        "roleListEditViewSource",
        new RoleListEdit(),
        "Main");
    }

    public class MenuItem : INotifyPropertyChanged
    {
      private string _label;
      public string Label
      {
        get { return _label; }
        set { _label = value; OnPropertyChanged("Label"); }
      }

      private string _methodName;
      public string MethodName
      {
        get { return _methodName; }
        set { _methodName = value; OnPropertyChanged("MethodName"); }
      }

      private Action _method;
      public Action Method
      {
        get { return _method; }
        set { _method = value; OnPropertyChanged("Method"); }
      }

      private bool _isAuthorized;
      public bool IsAuthorized
      {
        get { return _isAuthorized; }
        set { _isAuthorized = value; OnPropertyChanged("IsAuthorized"); }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      protected virtual void OnPropertyChanged(string propertyName)
      {
        if (PropertyChanged != null)
          PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
