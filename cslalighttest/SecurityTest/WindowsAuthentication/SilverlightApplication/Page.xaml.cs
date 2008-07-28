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

    private void btnSuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      txtSuccessfulLogin.Text = String.Empty;
      SLPrincipal.Logout();
      SLPrincipal.Login((o, e2) =>
      {
        if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        {
          txtSuccessfulLogin.Text = "Pass";
        }
        else
        {
          txtSuccessfulLogin.Text = "Fail";
        }
      });
    }

    private void btnRoles_Click(object sender, RoutedEventArgs e)
    {
      txtRolesSuccessful.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login((o, e2) =>
      {
        if (Csla.ApplicationContext.User.IsInRole("Users"))
        {
          txtRolesSuccessful.Text = "Pass";
        }
        else
        {
          txtRolesSuccessful.Text = "Fail";
        }
      });
    }

    private void btnRolesUnsuccessful_Click(object sender, RoutedEventArgs e)
    {
      txtRolesUnsuccessful.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login((o, e2) =>
      {
        if (!Csla.ApplicationContext.User.IsInRole("shouldNotMatch"))
        {
          txtRolesUnsuccessful.Text = "Pass";
        }
        else
        {
          txtRolesUnsuccessful.Text = "Fail";
        }
      });
    }
  }
}
