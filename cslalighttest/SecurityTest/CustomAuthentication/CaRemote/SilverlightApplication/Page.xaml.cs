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
using ClassLibrary;

namespace SilverlightApplication
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
      Csla.DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://localhost:4769/WcfPortal.svc";
    }

    private void btnRemoteSuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      txtRemoteSuccessfulLogin.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", (o, e2) =>
      { 
        if (Csla.ApplicationContext.User.Identity.Name == "TestUser"
          && Csla.ApplicationContext.User.Identity.IsAuthenticated
          && ((SLPrincipal.LoginEventArgs)e2).LoginSucceded)
        {
          txtRemoteSuccessfulLogin.Text = "Pass";
        }
        else
        {
          txtRemoteSuccessfulLogin.Text = "Fail";
        }
      });
    }

    private void btnRemoteUnsuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      txtRemoteUnsuccessfulLogin.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("invaliduser", "invalidpassword", (o, e2) =>
      {
        if (Csla.ApplicationContext.User.Identity.GetType() == typeof(Csla.Security.UnauthenticatedIdentity)
           && !((SLPrincipal.LoginEventArgs)e2).LoginSucceded)
        {
          txtRemoteUnsuccessfulLogin.Text = "Pass";
        }
        else
        {
          txtRemoteUnsuccessfulLogin.Text = "Fail";
        }
      });
    }

    private void btnRemoteRoles_Click(object sender, RoutedEventArgs e)
    {
      txtRemoteRoles.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", (o, e2) =>
      {
        if (Csla.ApplicationContext.User.IsInRole("User")
          && Csla.ApplicationContext.User.IsInRole("Admin")
          && !Csla.ApplicationContext.User.IsInRole("invalidrole"))
        {
          txtRemoteRoles.Text = "Pass";
        }
        else
        {
          txtRemoteRoles.Text = "Fail";
        }
      });
    }
  }
}
