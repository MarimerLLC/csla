using System;
using Bxf;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using WpUI.ViewModels;

namespace WpUI
{
  public class MainViewModel : DependencyObject, INotifyPropertyChanged, IShowStatus
  {
    private static MainViewModel _presenter;
    private Control _currentView;
    private IShowStatus _currentViewModel;

    public MainViewModel()
    {
      DesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
      _presenter = this;

      // use shell that understands WP7 navigation model
      Bxf.Shell.Instance = new ViewModels.NavigationShell();

      var presenter = (IPresenter)Bxf.Shell.Instance;
      presenter.OnShowError += (message, title) =>
        {
          try
          {
            MessageBox.Show(message, title, MessageBoxButton.OK);
          }
          catch
          { }
        };

      presenter.OnShowStatus += (status) =>
        {
          if (string.IsNullOrWhiteSpace(status.Text))
          {
            StatusContent = null;
            if (_currentViewModel != null)
              _currentViewModel.ShowStatus(status);
          }
          else
          {
            StatusContent = new Views.StatusDisplay { DataContext = status };
            if (_currentViewModel != null)
              _currentViewModel.ShowStatus(status);
          }
        };

      presenter.OnShowView += (view, region) =>
        {
          var nav = Application.Current.RootVisual as PhoneApplicationFrame;
          if (nav == null)
            return;

          switch (region)
          {
            case "Dialog":
              // Dialog region means showing a full page
              // instead of the panorama
              if (view != null && !string.IsNullOrEmpty(view.ViewName))
              {
                // navigate to new page
                nav.Navigate(new Uri("/Views" + view.ViewName, UriKind.Relative));
              }
              else
              {
                // navigate back, or to main page
                if (nav.CanGoBack)
                  nav.GoBack();
                else
                  nav.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
              }
              break;
            default:
              break;
          }
        };

      MainPageViewModel = new ViewModels.MainPageViewModel();
      _currentViewModel = null;
    }

    /// <summary>
    /// Initialize viewmodel objects for each view.
    /// </summary>
    private void InitializeViewModel(string viewName, string queryString, Control control)
    {
      object viewmodel = null;
      if (viewName.Contains("/Login.xaml"))
        viewmodel = new ViewModels.Login();
      else if (viewName.Contains("/ProjectDetails.xaml"))
        viewmodel = new ViewModels.ProjectDetail(queryString);
      else if (viewName.Contains("/ProjectEdit.xaml"))
        viewmodel = new ViewModels.ProjectEdit(queryString);
      else if (viewName.Contains("/ResourceDetails.xaml") || viewName.Contains("/ResourceEdit.xaml"))
        viewmodel = new ViewModels.ResourceDetail(queryString);
      _currentViewModel = viewmodel as IShowStatus;
      control.DataContext = viewmodel;
    }

    /// <summary>
    /// Handle navigated event, ensuring the new view
    /// is properly initialized with a viewmodel.
    /// </summary>
    public void Navigated(object sender, NavigationEventArgs e)
    {
      _currentViewModel = null;
      if (e.Content == null)
        return;

      _currentView = (Control)e.Content;

      var viewName = e.Uri.OriginalString;
      if (viewName.Contains("/MainPage.xaml"))
      {
        _presenter.MainPageViewModel.ReloadMainView();
        return;
      }

      // get query parameter
      string queryString = null;
      var param = viewName.IndexOf("?");
      if (param > 0)
      {
        queryString = viewName.Substring(param + 1);
        viewName = viewName.Substring(0, param);
      }

      // setup viewmodel for pages
      InitializeViewModel(viewName, queryString, _currentView);
    }

    public void LoginOut()
    {
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        ProjectTracker.Library.Security.PTPrincipal.Logout();
      Bxf.Shell.Instance.ShowView(
        "/Login.xaml",
        "loginViewSource",
        new ViewModels.Login(),
        "Dialog");
    }

    private MainPageViewModel _mainPageViewModel;
    public MainPageViewModel MainPageViewModel
    {
      get { return _mainPageViewModel; }
      set { _mainPageViewModel = value; OnPropertyChanged("MainPageViewModel"); }
    }

    public static void ReloadMainView()
    {
      _presenter.MainPageViewModel.ReloadMainView();
    }

    private bool _appBusy;
    public bool AppBusy
    {
      get { return _appBusy; }
      set { _appBusy = value; OnPropertyChanged("AppBusy"); }
    }

    private bool _designMode;
    public bool DesignMode
    {
      get { return _designMode; }
      private set { _designMode = value; OnPropertyChanged("DesignMode"); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool IsDataLoaded 
    {
      get { return MainPageViewModel.IsDataLoaded; } 
    }

    internal void LoadData()
    {
      MainPageViewModel.LoadData();
    }

    private Views.StatusDisplay _statusDisplay;
    public Views.StatusDisplay StatusContent
    {
      get { return _statusDisplay; }
      set { _statusDisplay = value; OnPropertyChanged("StatusContent"); }
    }

    public void ShowStatus(Status status)
    {
      StatusContent = new Views.StatusDisplay { DataContext = status };
    }
  }
}
