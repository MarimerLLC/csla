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
using System.Security;

namespace SilverlightApplication
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();

      //Csla.DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      //Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://localhost:3833/WcfPortal.svc";
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
            && Csla.ApplicationContext.User.Identity.IsAuthenticated)
          {
            txtSuccessfulLogin.Text = "Pass";
          }
          else
          {
            txtSuccessfulLogin.Text = "Fail";
          }
        });
      }
      catch (Exception ex)
      {
        txtSuccessfulLogin.Text = "Fail " + ex.Message; ;
      }
    }

    private void btnUnsuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        txtUnsuccessfulLogin.Text = String.Empty;

        SLPrincipal.Logout();
        SLPrincipal.Login("invaliduser", "invalidpassword",(o, e2) =>
        {
          if (Csla.ApplicationContext.User.Identity.GetType() == typeof(Csla.Security.UnauthenticatedIdentity))
          {
            txtUnsuccessfulLogin.Text = "Pass";
          }
          else
          {
            txtUnsuccessfulLogin.Text = "Fail";
          }
        });
      }
      catch
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
      catch
      {
        txtRoles.Text = "Fail";
      }
    }    

    private void btnAuthorizationA_Click(object sender, RoutedEventArgs e)
    {
      txtAuthorizationA.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUserA", "1234", (o, e2) =>
      {
        bool pass = true;

        try
        {
          ClassA classA = new ClassA();
          classA.A = "test";
          classA.B = "test";
          if (classA.A != "test" || classA.B != "test")
            pass = false;

          ClassB classB = new ClassB();
          classB.A = "test";
          classB.B = "test";
          if (classB.A != "test" || classB.B != "test")
            pass = false;
        }
        catch (Exception)
        {
          pass = false;
        }

        if (pass)
        {
          txtAuthorizationA.Text = "Pass";
        }
        else
        {
          txtAuthorizationA.Text = "Fail";
        }
      });
    }

    private void btnAuthorizationB_Click(object sender, RoutedEventArgs e)
    {
      txtAuthorizationB.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", (o, e2) =>
      {
        bool pass = true;

        try
        {
          ClassA classA = new ClassA();
          try
          {
            classA.A = "test";
            pass = false;
          }
          catch (SecurityException)
          { }
          classA.B = "test";
          if (classA.B != "test")
            pass = false;

          ClassB classB = new ClassB();
          try
          {
            classB.A = "test";
            pass = false;
          }
          catch (SecurityException)
          { }
          classB.B = "test";
          if (classB.B != "test")
            pass = false;
        }
        catch (Exception)
        {
          pass = false;
        }

        if (pass)
        {
          txtAuthorizationB.Text = "Pass";
        }
        else
        {
          txtAuthorizationB.Text = "Fail";
        }
      });
    }

    private void btnAuthorizationC_Click(object sender, RoutedEventArgs e)
    {
      txtAuthorizationC.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", (o, e2) =>
      {
        bool pass = true;

        try
        {
          try
          {
            ClassA classA = new ClassA();
            pass = false;
          }
          catch (SecurityException)
          { }

          ClassB classB = new ClassB();
          classB.A = "test";
          classB.B = "test";
          if (classB.A != "test" || classB.B != "test")
            pass = false;
        }
        catch (Exception ex)
        {
          pass = false;
        }

        if (pass)
        {
          txtAuthorizationC.Text = "Pass";
        }
        else
        {
          txtAuthorizationC.Text = "Fail";
        }
      });
    }

    private void btnAuthorizationD_Click(object sender, RoutedEventArgs e)
    {
      txtAuthorizationD.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUserD", "1234", (o, e2) =>
      {
        bool pass = true;

        var identity = Csla.ApplicationContext.User.Identity as SLIdentity;

        if (identity != null)
        {
          if (identity.Name != "TestUserD")
            pass = false;
          if (identity.Extra != "Extra data")
            pass = false;
          if (identity.MoreData != "Even more data")
            pass = false;
        }
        else
          pass = false;

        if (pass)
        {
          txtAuthorizationD.Text = "Pass";
        }
        else
        {
          txtAuthorizationD.Text = "Fail";
        }
      });
    }
  }
}
