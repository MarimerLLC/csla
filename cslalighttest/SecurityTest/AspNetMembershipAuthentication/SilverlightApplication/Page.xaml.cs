using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightClassLibrary;

namespace SilverlightApplication
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
      Csla.DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://localhost:3372/WcfPortal.svc";
    }

    private void btnSuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        txtSuccessfulLogin.Text = String.Empty;

        SLPrincipal.Logout();
        SLPrincipal.Login("TestUser", "1234", (o, e2) =>
        {
          if (Csla.ApplicationContext.User.Identity.Name == "TestUser"
            && Csla.ApplicationContext.User.Identity.IsAuthenticated
            && ((SLPrincipal.LoginEventArgs)e2).LoginSucceded)
          {
            txtSuccessfulLogin.Text = "Pass";
          }
          else
          {
            txtSuccessfulLogin.Text = "Fail";
          }
        });
      }
      catch(Exception ex)
      {
        txtSuccessfulLogin.Text = "Fail";
      }
    }

    private void btnUnsuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        txtUnsuccessfulLogin.Text = String.Empty;

        SLPrincipal.Logout();
        SLPrincipal.Login("invaliduser", "invalidpassword", (o, e2) =>
        {
          if (!Csla.ApplicationContext.User.Identity.IsAuthenticated
             && Csla.ApplicationContext.User.Identity.Name == ""
             && !((SLPrincipal.LoginEventArgs)e2).LoginSucceded)
          {
            txtUnsuccessfulLogin.Text = "Pass";
          }
          else
          {
            txtUnsuccessfulLogin.Text = "Fail";
          }
        });
      }
      catch (Exception ex)
      {
        txtUnsuccessfulLogin.Text = "Fail";
      }
    }

    private void btnRoles_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        txtRoles.Text = String.Empty;

        SLPrincipal.Logout();
        SLPrincipal.Login("TestUser", "1234", (o, e2) =>
        {
          if (Csla.ApplicationContext.User.IsInRole("User")
            && Csla.ApplicationContext.User.IsInRole("Admin")
            && !Csla.ApplicationContext.User.IsInRole("invalidrole"))
          {
            txtRoles.Text = "Pass";
          }
          else
          {
            txtRoles.Text = "Fail";
          }
        });
      }
      catch (Exception ex)
      {
        txtRoles.Text = "Fail";
      }
    }
  }
}
