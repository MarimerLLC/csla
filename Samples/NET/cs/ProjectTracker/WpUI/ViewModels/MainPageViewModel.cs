using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using Bxf;
using System.Windows.Data;
using System.Collections.Generic;

namespace WpUI.ViewModels
{
  public class MainPageViewModel : INotifyPropertyChanged, IShowStatus, IViewModel
  {
    public bool ProjectsChanged { get; set; }
    public bool ResourcesChanged { get; set; }

    public void LoadData()
    {
      if (MainViews == null)
      {
        MainViews = new ObservableCollection<PanoramaItem>
        {
          new PanoramaItem { Header = "welcome", Content = new Views.Welcome() },
          new PanoramaItem { Header = "projects", Content = new Views.ProjectList() },
          new PanoramaItem { Header = "resources", Content = new Views.ResourceList() }
        };
        var welcomeView = (Views.Welcome)MainViews.First(r => r.Content is Views.Welcome).Content;
        welcomeView.DataContext = new ViewModels.Welcome();
      }

      if (ProjectsChanged)
      {
        ProjectsChanged = false;
        var projectView = (Views.ProjectList)MainViews.First(r => r.Content is Views.ProjectList).Content;
        projectView.DataContext = new ViewModels.ProjectList(); // new List<object> { new ViewModels.ProjectList() };
      }

      if (ResourcesChanged)
      {
        ResourcesChanged = false;
        var resourceView = (Views.ResourceList)MainViews.First(r => r.Content is Views.ResourceList).Content;
        resourceView.DataContext = new List<object> { new ViewModels.ResourceList() };
      }
    }

    public bool IsDataLoaded
    {
      get { return MainViews != null && !ProjectsChanged && !ResourcesChanged; }
    }

    public void Refresh()
    {
      ProjectsChanged = true;
      ResourcesChanged = true;
      LoadData();
    }

    private ObservableCollection<PanoramaItem> _mainViews;
    public ObservableCollection<PanoramaItem> MainViews
    {
      get { return _mainViews; }
      set
      {
        _mainViews = value;
        OnPropertyChanged("MainViews");
      }
    }

    public void LoginOut()
    {
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Logging out" });
        ProjectTracker.Library.Security.PTPrincipal.Logout();
      }
      else
      {
        Bxf.Shell.Instance.ShowView("/Login.xaml", null, null, null);
      }
    }

    public void ViewRoles()
    {
      Bxf.Shell.Instance.ShowView("/RoleListEdit.xaml", null, null, null);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private Views.StatusDisplay _statusDisplay;
    public Views.StatusDisplay StatusContent
    {
      get { return _statusDisplay; }
      set { _statusDisplay = value; OnPropertyChanged("StatusContent"); }
    }

    public void ShowStatus(Status status)
    {
      if (status.IsBusy)
        StatusContent = new Views.StatusDisplay { DataContext = status };
      else
        StatusContent = null;
    }

    public void NavigatingTo()
    {
      Refresh();
    }

    public void NavigatingBackTo()
    {
      LoadData();
    }

    public void NavigatedAway()
    {
    }
  }
}
