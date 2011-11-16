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
  public class MainViewModel : DependencyObject, INotifyPropertyChanged
  {
    DispatcherTimer _closeTimer = new DispatcherTimer();
    DateTime _statusClose = DateTime.MaxValue;

    private static MainViewModel _presenter;

    public MainViewModel()
    {
      DesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
      _presenter = this;

      // set up timer to close status display
      _closeTimer.Tick += new EventHandler(CloseTimer_Tick);
      _closeTimer.Interval = new TimeSpan(1000);
      if (!DesignMode)
        _closeTimer.Start();

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
          Shell.Instance.ShowView(
            typeof(Views.StatusDisplay).AssemblyQualifiedName,
            "statusViewSource",
            status,
            "Status");
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
            case "Status":
              _statusClose = DateTime.Now.Add(new TimeSpan(0, 0, 5));
              if (view.Model != null)
                AppBusy = ((Bxf.Status)view.Model).IsBusy;
              else
                AppBusy = false;
              StatusContent = view.ViewInstance;
              break;
            default:
              break;
          }
        };

      Shell.Instance.ShowStatus(new Status { Text = "Initializing..." });
      MainPageViewModel = new ViewModels.MainPageViewModel();
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
      control.DataContext = viewmodel;
    }

    /// <summary>
    /// Handle navigated event, ensuring the new view
    /// is properly initialized with a viewmodel.
    /// </summary>
    public void Navigated(object sender, NavigationEventArgs e)
    {
      if (e.Content == null)
        return;

      var viewName = e.Uri.OriginalString;
      if (viewName.Contains("/MainPage.xaml"))
        return;

      // get query parameter
      string queryString = null;
      var param = viewName.IndexOf("?");
      if (param > 0)
      {
        queryString = viewName.Substring(param + 1);
        viewName = viewName.Substring(0, param);
      }

      // setup viewmodel for pages
      InitializeViewModel(viewName, queryString, (Control)e.Content);
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


    void CloseTimer_Tick(object sender, EventArgs e)
    {
      if (DateTime.Now > _statusClose && !AppBusy)
      {
        _statusClose = DateTime.MaxValue;
        Shell.Instance.ShowView(null, "", null, "Status");
      }
    }

    private UserControl _statusContent;
    public UserControl StatusContent
    {
      get { return _statusContent; }
      set { _statusContent = value; OnPropertyChanged("StatusContent"); }
    }

    private UserControl _userContent;
    public UserControl UserContent
    {
      get { return _userContent; }
      set { _userContent = value; OnPropertyChanged("UserContent"); }
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
  }
}
