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
    private static MainViewModel _presenter;
    private Control _currentView;
    private IShowStatus _currentViewModel;
    private int _busyCount;

    public MainViewModel()
    {
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
          if (status.IsBusy)
            _busyCount++;
          else
            _busyCount--;
          if (_busyCount > 0)
            status.IsBusy = true;
          if (string.IsNullOrEmpty(status.Text) && status.IsBusy)
            return;

          if (_currentViewModel != null)
            _currentViewModel.ShowStatus(status);
          if (_currentViewModel != MainPageViewModel)
            MainPageViewModel.ShowStatus(status);
        };

      presenter.OnShowView += (view, region) =>
        {
          var nav = Application.Current.RootVisual as PhoneApplicationFrame;
          if (nav == null)
            return;

          if (view == null || string.IsNullOrEmpty(view.ViewName))
          {
            // navigate back, or to main page
            if (nav.CanGoBack)
              nav.GoBack();
            else
              nav.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
          }
          else
          {
            // navigate to new page
            nav.Navigate(new Uri("/Views" + view.ViewName, UriKind.Relative));
          }
        };
    }

    /// <summary>
    /// Initialize viewmodel objects for each view.
    /// </summary>
    private void InitializeViewModel(Control control, string queryString, NavigationMode navigationMode)
    {
      var oldVm = _currentViewModel as IViewModel;
      if (oldVm != null)
        oldVm.NavigatedAway();

      object viewmodel = null;
      if (control is WpUI.MainPage)
      {
        viewmodel = App.ViewModel.MainPageViewModel;
        App.ViewModel.MainPageViewModel.ReloadMainView();
      }

      else if (control is Views.Login)
      {
        viewmodel = new ViewModels.Login();
      }

      else if (control is Views.ProjectDetails)
      {
        if (navigationMode == NavigationMode.Back && !App.ViewModel.MainPageViewModel.ProjectsChanged)
          viewmodel = control.DataContext;
        else
          viewmodel = new ViewModels.ProjectDetail(queryString);
      }

      else if (control is Views.ProjectEdit)
      {
        if (navigationMode == NavigationMode.Back)
          viewmodel = control.DataContext;
        else
          viewmodel = new ViewModels.ProjectEdit(queryString);
      }

      else if (control is Views.ProjectResourceEdit)
      {
        viewmodel = ((NavigationShell)Bxf.Shell.Instance).PendingView.Model;
      }

      else if (control is Views.ResourceDetails)
      {
        if (navigationMode == NavigationMode.Back && !App.ViewModel.MainPageViewModel.ResourcesChanged)
          viewmodel = control.DataContext;
        else
          viewmodel = new ViewModels.ResourceDetail(queryString);
      }

      else if (control is Views.ResourceEdit)
      {
        if (navigationMode == NavigationMode.Back)
          viewmodel = control.DataContext;
        else
          viewmodel = new ViewModels.ResourceEdit(queryString);
      }

      // update current view
      _currentViewModel = viewmodel as IShowStatus;
      var newVm = _currentViewModel as IViewModel;
      // indicate navigation
      if (newVm != null)
        newVm.NavigatingTo();

      // set view datacontext if necessary
      if (!ReferenceEquals(control.DataContext, viewmodel))
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

      // get view name
      var viewName = e.Uri.OriginalString;

      // get query parameter
      string queryString = null;
      var param = viewName.IndexOf("?");
      if (param > 0)
      {
        queryString = viewName.Substring(param + 1);
        viewName = viewName.Substring(0, param);
      }
      
      // setup viewmodel for pages
      InitializeViewModel(_currentView, queryString, e.NavigationMode);
    }

    public void ShowView(string viewName)
    {
      Bxf.Shell.Instance.ShowView(viewName, null, null, null);
    }

    public void Back()
    {
      ShowView(null);
    }

    public void ShowStatus(Bxf.Status status)
    {
      Bxf.Shell.Instance.ShowStatus(status);
    }

    public void ShowError(string message, string title)
    {
      Bxf.Shell.Instance.ShowError(message, title);
    }


    public void LoginOut()
    {
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        ProjectTracker.Library.Security.PTPrincipal.Logout();
      Bxf.Shell.Instance.ShowView("/Login.xaml", null, null, null);
    }

    private MainPageViewModel _mainPageViewModel;
    public MainPageViewModel MainPageViewModel
    {
      get 
      {
        if (_mainPageViewModel == null)
          _mainPageViewModel = new MainPageViewModel();
        return _mainPageViewModel; 
      }
      set 
      { 
        _mainPageViewModel = value; 
        OnPropertyChanged("MainPageViewModel"); 
      }
    }

    public void ReloadMainView()
    {
      _presenter.MainPageViewModel.ReloadMainView();
    }

    public bool IsDataLoaded
    {
      get { return MainPageViewModel.IsDataLoaded; }
    }

    internal void LoadData()
    {
      MainPageViewModel.LoadData();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
