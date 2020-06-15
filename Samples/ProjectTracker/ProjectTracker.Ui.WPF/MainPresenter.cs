using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Bxf;
using Csla;

namespace WpfUI
{
  public class MainPresenter : DependencyObject, INotifyPropertyChanged
  {
    private readonly DispatcherTimer _closeTimer = new DispatcherTimer();
    private DateTime _errorClose = DateTime.MaxValue;
    private DateTime _statusClose = DateTime.MaxValue;

    public MainPresenter()
    {
      DesignMode = DesignerProperties.GetIsInDesignMode(this);

      _closeTimer.Tick += new EventHandler(CloseTimer_Tick);
      _closeTimer.Interval = new TimeSpan(1000);
      if (!DesignMode)
        _closeTimer.Start();

      var presenter = (IPresenter)Shell.Instance;

      presenter.OnShowError += (message, title) =>
        {
          Shell.Instance.ShowView(
            typeof(Views.ErrorDisplay).AssemblyQualifiedName,
            "errorViewSource", 
            new ViewModels.Error { ErrorContent = message },
            "Error");
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
          switch (region)
          {
            case "Main":
              MainContent = view.ViewInstance;
              break;
            case "Menu":
              MenuContent = view.ViewInstance;
              break;
            case "User":
              UserContent = view.ViewInstance;
              break;
            case "Error":
              _errorClose = DateTime.Now.Add(new TimeSpan(0, 0, 5));
              ErrorContent = view.ViewInstance;
              break;
            case "Status":
              _statusClose = DateTime.Now.Add(new TimeSpan(0, 0, 5));
              if (view.Model != null)
                AppBusy = ((Status)view.Model).IsBusy;
              else
                AppBusy = false;
              StatusContent = view.ViewInstance;
              break;
            default:
              break;
          }
        };

      ProjectTracker.Library.Security.PTPrincipal.Logout();
      LoadRoleListCache();

      Shell.Instance.ShowView(
        typeof(Views.UserDisplay).AssemblyQualifiedName,
        "userViewSource",
        new ViewModels.User(),
        "User");

      Shell.Instance.ShowView(
        typeof(Views.Dashboard).AssemblyQualifiedName,
        "dashboardViewSource",
        new ViewModels.Dashboard(),
        "Main");

      ShowMenu();

      Shell.Instance.ShowStatus(new Status { Text = "Ready" });
    }

    private void LoadRoleListCache()
    {
      try
      {
        var task = ProjectTracker.Library.RoleList.CacheListAsync();
      }
      catch (DataPortalException ex)
      {
        Shell.Instance.ShowError(ex.Message, "Retrieve RoleList");
      }
    }

    public static void ShowMenu()
    {
      Shell.Instance.ShowView(
        typeof(Views.MainMenu).AssemblyQualifiedName,
        "mainMenuViewSource",
        new ViewModels.MainMenu(),
        "Menu");
    }

    void CloseTimer_Tick(object sender, EventArgs e)
    {
      if (DateTime.Now > _statusClose && !AppBusy)
      {
        _statusClose = DateTime.MaxValue;
        Shell.Instance.ShowView(null, "", null, "Status");
      }
      if (DateTime.Now > _errorClose)
      {
        _errorClose = DateTime.MaxValue;
        Shell.Instance.ShowView(null, "", null, "Error");
      }
    }

    private UserControl _mainContent;
    public UserControl MainContent
    {
      get { return _mainContent; }
      set { _mainContent = value; OnPropertyChanged(nameof(MainContent)); }
    }

    private UserControl _menuContent;
    public UserControl MenuContent
    {
      get { return _menuContent; }
      set { _menuContent = value; OnPropertyChanged(nameof(MenuContent)); }
    }

    private UserControl _statusContent;
    public UserControl StatusContent
    {
      get { return _statusContent; }
      set { _statusContent = value; OnPropertyChanged(nameof(StatusContent)); }
    }

    private UserControl _errorContent;
    public UserControl ErrorContent
    {
      get { return _errorContent; }
      set { _errorContent = value; OnPropertyChanged(nameof(ErrorContent)); }
    }

    private UserControl _userContent;
    public UserControl UserContent
    {
      get { return _userContent; }
      set { _userContent = value; OnPropertyChanged(nameof(UserContent)); }
    }

    private bool _appBusy;
    public bool AppBusy
    {
      get { return _appBusy; }
      set { _appBusy = value; OnPropertyChanged(nameof(AppBusy)); }
    }

    private bool _designMode;
    public bool DesignMode
    {
      get { return _designMode; }
      private set { _designMode = value; OnPropertyChanged(nameof(DesignMode)); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
