using System;
using System.ComponentModel;
using Bxf;

namespace WpUI.ViewModels
{
  public class Login : INotifyPropertyChanged, IShowStatus
  {
    private string _username = "manager";
    public string Username
    {
      get { return _username; }
      set { _username = value; OnPropertyChanged("Username"); }
    }

    private string _password = "manager";
    public string Password
    {
      get { return _password; }
      set { _password = value; OnPropertyChanged("Password"); }
    }

    public void LoginUser()
    {
      App.ViewModel.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Validating credentials..." });
      ProjectTracker.Library.Security.PTPrincipal.BeginLogin(Username, Password);
      App.ViewModel.Back();
    }

    public void Cancel()
    {
      App.ViewModel.Back();
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
