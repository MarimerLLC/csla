using System.Windows.Controls;

namespace WpUI.ViewModels
{
  public class ViewModelFactory
  {
    /// <summary>
    /// Create viewmodel objects for each view.
    /// </summary>
    public object CreateViewModel(Control control, string queryString)
    {
      object result = null;

      if (control is WpUI.MainPage)
        result = App.ViewModel.MainPageViewModel;
      
      else if (control is Views.Login)
        result = new ViewModels.Login();
      
      else if (control is Views.ProjectDetails)
        result = new ViewModels.ProjectDetail(queryString);
      
      else if (control is Views.ProjectEdit)
        result = new ViewModels.ProjectEdit(queryString);
      
      else if (control is Views.ResourceDetails)
        result = new ViewModels.ResourceDetail(queryString);
      
      else if (control is Views.ResourceEdit)
        result = new ViewModels.ResourceEdit(queryString);
      
      else if (control is Views.RoleListEdit)
        result = new ViewModels.RoleListEdit();

      else
        result = ((NavigationShell)Bxf.Shell.Instance).PendingView.Model;

      ((NavigationShell)Bxf.Shell.Instance).PendingView = null;

      return result;
    }
  }
}
