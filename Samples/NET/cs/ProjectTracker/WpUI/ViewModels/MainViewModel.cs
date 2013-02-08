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
    public bool AppBusy
    {
      get { return _busyCount > 0; }
    }

    public bool DesignMode { get; private set; }

    public MainViewModel()
    {
      _presenter = this;

      DesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);

      // use shell that understands WP7 navigation model
      Bxf.Shell.Instance = new ViewModels.NavigationShell();

      var presenter = (IPresenter)Bxf.Shell.Instance;
      presenter.OnShowError += (message, title) =>
        {
          MessageBox.Show(message, title, MessageBoxButton.OK);
        };

      presenter.OnShowStatus += (status) =>
        {
          var oldBusy = AppBusy;
          if (status.IsBusy)
            _busyCount++;
          else
            _busyCount--;
          if (oldBusy != AppBusy)
            OnPropertyChanged("AppBusy");

          // if no status to display, but app still busy then exit
          if (string.IsNullOrEmpty(status.Text) && AppBusy)
            return;

          if (_currentViewModel != null)
            _currentViewModel.ShowStatus(status);
          if (_currentViewModel != MainPageViewModel)
            MainPageViewModel.ShowStatus(status);
        };

      presenter.OnShowView += (view, region) =>
        {
          if (region == "confirm")
          {
            var c = (Confirm)view.Model;
            c.Result = MessageBox.Show(c.Prompt, c.Title, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
          }
          else
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
          }
        };
    }

    /// <summary>
    /// Handle navigated event, ensuring the new view
    /// is properly initialized with a viewmodel.
    /// </summary>
    public void Navigated(object sender, NavigationEventArgs e)
    {
      // indicate navigation
      var oldVm = _currentViewModel as IViewModel;
      if (oldVm != null)
        oldVm.NavigatedAway();

      _currentViewModel = null;
      if (e.Content == null)
        return;
      _currentView = (Control)e.Content;

      object newViewModel = null;
      if (e.NavigationMode == NavigationMode.Back)
      {
        // use existing viewmodel for view
        newViewModel = _currentView.DataContext;
      }
      else
      {
        // get query parameter
        var uri = e.Uri.OriginalString;
        string queryString = null;
        var param = uri.IndexOf("?");
        if (param > 0)
          queryString = uri.Substring(param + 1);

        // create and/or initialize viewmodel
        var vmf = new ViewModelFactory();
        newViewModel = vmf.CreateViewModel(_currentView, queryString);
      }

      // update current view
      _currentViewModel = newViewModel as IShowStatus;

      // indicate navigation
      var newVm = _currentViewModel as IViewModel;
      if (newVm != null)
        if (e.NavigationMode == NavigationMode.Back)
        newVm.NavigatingBackTo();
      else
          newVm.NavigatingTo();

      // set view datacontext if necessary
      if (!ReferenceEquals(_currentView.DataContext, _currentViewModel))
        _currentView.DataContext = _currentViewModel;
    }

    public void ShowView(string viewName)
    {
      Bxf.Shell.Instance.ShowView(viewName, null, null, null);
    }

    public void Back()
    {
      ShowView(null);
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
