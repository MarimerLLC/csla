﻿using System;
using System.ComponentModel;

namespace WpfUI.ViewModels
{
  public class Login : INotifyPropertyChanged
  {
    private string _username;
    public string Username
    {
      get { return _username; }
      set { _username = value; OnPropertyChanged("Username"); }
    }

    private string _password;
    public string Password
    {
      get { return _password; }
      set { _password = value; OnPropertyChanged("Password"); }
    }

    public void LoginUser()
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Validating credentials..." });
      ProjectTracker.Library.Security.PTPrincipal.BeginLogin(Username, Password);
      Bxf.Shell.Instance.ShowView(null, "Main");
    }

    public void Cancel()
    {
      Bxf.Shell.Instance.ShowView(null, "Main");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
