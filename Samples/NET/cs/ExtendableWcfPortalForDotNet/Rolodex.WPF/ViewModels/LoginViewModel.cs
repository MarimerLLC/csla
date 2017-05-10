﻿using System;
using System.Collections.Generic;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Rolodex.Business.Security;
using Rolodex.Silverlight.Core;
using Rolodex.Silverlight.Events;
using Rolodex.Silverlight.Views;

namespace Rolodex.Silverlight.ViewModels
{
  public class LoginViewModel : ViewModel<RolodexPrincipal, LoginView>
  {
    public LoginViewModel(IEventAggregator eventAggregator)
      : base(eventAggregator)
    {
      LoginCommand = new DelegateCommand<object>(Login, CanLogin);
      View = new LoginView();
    }

    #region Properties

    private string _userID = string.Empty;

    public string UserID
    {
      get { return _userID; }
      set
      {
        if (_userID == value)
          return;
        _userID = value;
        OnPropertyChanged("UserID");
        LoginCommand.RaiseCanExecuteChanged();
      }
    }

    private string _status = "Please provide user name and password, then click Login";

    public string Status
    {
      get { return _status; }
      private set
      {
        if (_status == value)
          return;
        _status = value;
        OnPropertyChanged("Status");
      }
    }

    #endregion

    #region LoginCommand

    public DelegateCommand<object> LoginCommand { get; private set; }

    private void Login(object parameter)
    {
      base.ShowPleaseWaitMessage();
      try
      {
        RolodexPrincipal.Login(_userID, this.View.UserPwdBox.Password);
        if (!Csla.ApplicationContext.User.Identity.IsAuthenticated)
        {
          Status = "Invalid user ID or password";
        }
        else
        {
          Status = "Login successfull.";
        }
        base.HidePleaseWaitMessage();
        if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        {
          CurrentEventAggregator.GetEvent<ShowCompaniesListEvent>().Publish(EventArgs.Empty);
        }
      }
      catch (Exception ex)
      {
        ErrorHandler.HandleException(ex);
      }
    }

    private bool CanLogin(object parameter)
    {
      if (string.IsNullOrEmpty(_userID))
      {
        return false;
      }
      return true;
    }

    private List<string> _themes;

    public List<string> Themes
    {
      get
      {
        if (_themes == null)
        {
          _themes = new List<string>();
          _themes.Add("ShinyBlue");
          _themes.Add("ShinyRed");
        }
        return _themes;
      }
    }

    private string _themeName = "ShinyBlue";

    public string ThemeName
    {
      get { return _themeName; }
      set
      {
        _themeName = value;
        Csla.ApplicationContext.LocalContext["theme"] = value;
        OnPropertyChanged("ThemeName");
        ApplyTheme();
      }
    }

    #endregion
  }
}