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
      Csla.DataPortal.ProxyTypeName = "Local";
    }

    private void btnLocalSuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      txtLocalSuccessfulLogin.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", (o, e2) =>
      {
        if (Csla.ApplicationContext.User.Identity.Name == "TestUser"
          && Csla.ApplicationContext.User.Identity.IsAuthenticated
          && ((SLPrincipal.LoginEventArgs)e2).LoginSucceded)
        {
          txtLocalSuccessfulLogin.Text = "Pass";
        }
        else
        {
          txtLocalSuccessfulLogin.Text = "Fail";
        }
      });

    }

    private void btnLocalUnsuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      txtLocalUnsuccessfulLogin.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("invaliduser", "invalidpassword", (o, e2) =>
      {
        if (Csla.ApplicationContext.User.Identity.GetType() == typeof(Csla.Security.UnauthenticatedIdentity)
          && !((SLPrincipal.LoginEventArgs)e2).LoginSucceded)
        {
          txtLocalUnsuccessfulLogin.Text = "Pass";
        }
        else
        {
          txtLocalUnsuccessfulLogin.Text = "Fail";
        }
      });
    }

    private void btnLocalRoles_Click(object sender, RoutedEventArgs e)
    {
      txtLocalRoles.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", (o, e2) =>
      {
        if (Csla.ApplicationContext.User.IsInRole("User")
          && Csla.ApplicationContext.User.IsInRole("Admin")
          && !Csla.ApplicationContext.User.IsInRole("invalidrole"))
        {
          txtLocalRoles.Text = "Pass";
        }
        else
        {
          txtLocalRoles.Text = "Fail";
        }
      });
    }
  }
}
