using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using Bxf;
using System.Windows.Data;
using System.Collections.Generic;

namespace WpUI.ViewModels
{
  public class MainPageViewModel : INotifyPropertyChanged
  {
    public MainPageViewModel()
    {
      LoadData();
    }

    public void ReloadMainView()
    {
      IsDataLoaded = false;
      LoadData();
    }

    public bool IsDataLoaded { get; private set; }

    public void LoadData()
    {
      if (IsDataLoaded)
      {
        while (MainViews.Count > 1)
          MainViews.RemoveAt(MainViews.Count - 1);
      }
      else
      {
        MainViews = new ObservableCollection<PanoramaItem>();
      }

      var welcomeView = new Views.Welcome();
      ((CollectionViewSource)welcomeView.Resources["welcomeViewSource"]).Source = 
        new List<object> { new ViewModels.Welcome() };
      MainViews.Add(new PanoramaItem { Header = "welcome", Content = welcomeView });

      var projectView = new Views.ProjectList();
      ((CollectionViewSource)projectView.Resources["projectListViewSource"]).Source =
        new List<object> { new ViewModels.ProjectList() };
      MainViews.Add(new PanoramaItem { Header = "projects", Content = projectView });

      var resourceView = new Views.ResourceList();
      ((CollectionViewSource)resourceView.Resources["resourceListViewSource"]).Source = 
        new List<object> { new ViewModels.ResourceList() };
      MainViews.Add(new PanoramaItem { Header = "resources", Content = resourceView });

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
  }
}
