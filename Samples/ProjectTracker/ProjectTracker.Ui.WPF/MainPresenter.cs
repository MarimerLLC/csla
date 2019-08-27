using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Bxf;
using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace WpfUI
{
  public class MainPresenter : DependencyObject, INotifyPropertyChanged
  {
    DispatcherTimer _closeTimer = new DispatcherTimer();
    DateTime _errorClose = DateTime.MaxValue;
    DateTime _statusClose = DateTime.MaxValue;

    public MainPresenter()
    {
      DesignMode = DesignerProperties.GetIsInDesignMode(this);
      DoStartup();

      _closeTimer.Tick += new EventHandler(CloseTimer_Tick);
      _closeTimer.Interval = new TimeSpan(1000);
      if (!DesignMode)
        _closeTimer.Start();

      var presenter = (IPresenter)Bxf.Shell.Instance;

      LoadRoleListCache();

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
                AppBusy = ((Bxf.Status)view.Model).IsBusy;
              else
                AppBusy = false;
              StatusContent = view.ViewInstance;
              break;
            default:
              break;
          }
        };

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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        ProjectTracker.Library.RoleList.CacheListAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      }
      catch (DataPortalException ex)
      {
        Shell.Instance.ShowError(ex.Message, "Retrieve RoleList");
      }
    }

    private void DoStartup()
    {
      // basically a WPF "app builder" implementation
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddScoped((c) =>
        Startup.LoadAppConfiguration(Array.Empty<string>()));
      var startup = ActivatorUtilities.CreateInstance<Startup>(
        serviceCollection.BuildServiceProvider(), Array.Empty<object>());
      startup.ConfigureServices(serviceCollection);
      ProjectTracker.Ui.WPF.App.ServiceProvider =
        serviceCollection.BuildServiceProvider();
      startup.Configure();
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
      set { _mainContent = value; OnPropertyChanged("MainContent"); }
    }

    private UserControl _menuContent;
    public UserControl MenuContent
    {
      get { return _menuContent; }
      set { _menuContent = value; OnPropertyChanged("MenuContent"); }
    }

    private UserControl _statusContent;
    public UserControl StatusContent
    {
      get { return _statusContent; }
      set { _statusContent = value; OnPropertyChanged("StatusContent"); }
    }

    private UserControl _errorContent;
    public UserControl ErrorContent
    {
      get { return _errorContent; }
      set { _errorContent = value; OnPropertyChanged("ErrorContent"); }
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
  }
}
