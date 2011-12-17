using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using Bxf;
using System.Windows.Data;
using System.Collections.Generic;

namespace WpUI.ViewModels
{
  public class MainPageViewModel : INotifyPropertyChanged, IShowStatus
  {
    public void ReloadMainView()
    {
      LoadData();
    }

    public bool IsDataLoaded { get; private set; }

    public void LoadData()
    {
      if (!IsDataLoaded)
      {
        MainViews = new ObservableCollection<PanoramaItem>
        {
          new PanoramaItem { Header = "welcome", Content = new Views.Welcome() },
          new PanoramaItem { Header = "projects", Content = new Views.ProjectList() },
          new PanoramaItem { Header = "resources", Content = new Views.ResourceList() }
        };
      }

      var welcomeView = (Views.Welcome)MainViews.First(r => r.Content is Views.Welcome).Content;
      welcomeView.DataContext = new ViewModels.Welcome();

      var projectView = (Views.ProjectList)MainViews.First(r => r.Content is Views.ProjectList).Content;
      ((CollectionViewSource)projectView.Resources["projectListViewSource"]).Source =
        new List<object> { new ViewModels.ProjectList() };

      var resourceView = (Views.ResourceList)MainViews.First(r => r.Content is Views.ResourceList).Content;
      ((CollectionViewSource)resourceView.Resources["resourceListViewSource"]).Source =
        new List<object> { new ViewModels.ResourceList() };

      IsDataLoaded = true;
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
  }
}
