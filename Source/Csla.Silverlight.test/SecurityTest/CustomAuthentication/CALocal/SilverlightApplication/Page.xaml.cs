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
using ClassLibrary;
using System.Security;

namespace SilverlightApplication
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
      Csla.DataPortal.ProxyTypeName = "Local";
    }

    private void btnSuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      txtSuccessfulLogin.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", "", (o, e2) =>
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

    private void btnUnsuccessfulLogin_Click(object sender, RoutedEventArgs e)
    {
      txtUnsuccessfulLogin.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("invaliduser", "invalidpassword", "", (o, e2) =>
      {
        if (Csla.ApplicationContext.User.Identity.GetType() == typeof(Csla.Security.UnauthenticatedIdentity)
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

    private void btnRoles_Click(object sender, RoutedEventArgs e)
    {
      txtRoles.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", "User;Admin", (o, e2) =>
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

    private void btnAuthorizationA_Click(object sender, RoutedEventArgs e)
    {
      txtAuthorizationA.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", "ClassARole;PropertyARole", (o, e2) =>
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
        catch (Exception ex)
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
      SLPrincipal.Login("TestUser", "1234", "ClassARole", (o, e2) =>
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
          catch (SecurityException ex)
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
          catch (SecurityException ex)
          { }
          classB.B = "test";
          if (classB.B != "test")
            pass = false;
        }
        catch (Exception ex)
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
      SLPrincipal.Login("TestUser", "1234", "PropertyARole", (o, e2) =>
      {
        bool pass = true;

        try
        {
          try
          {
            ClassA classA = new ClassA();
            pass = false;
          }
          catch (SecurityException ex)
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
      SLPrincipal.Login("TestUser", "1234", "", (o, e2) =>
      {
        bool pass = true;

        try
        {
          try
          {
            ClassA classA = new ClassA();
            pass = false;
          }
          catch (SecurityException ex)
          { }

          ClassB classB = new ClassB();
          try
          {
            classB.A = "test";
            pass = false;
          }
          catch (SecurityException ex)
          { }
          classB.B = "test";
          if (classB.B != "test")
            pass = false;
        }
        catch (Exception ex)
        {
          pass = false;
        }

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
