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

    private void btnAuthorizationA_Click(object sender, RoutedEventArgs e)
    {
      txtAuthorizationA.Text = String.Empty;

      SLPrincipal.Logout();
      SLPrincipal.Login("ClassARole;PropertyARole", (o, e2) =>
      {
        bool pass = true;

        try
        {
          ClassA1 classA1 = new ClassA1();
          classA1.A = "test";
          classA1.B = "test";
          if (classA1.A != "test" || classA1.B != "test")
            pass = false;

          ClassA2 classA2 = new ClassA2();
          classA2.A = "test";
          classA2.B = "test";
          if (classA2.A != "test" || classA2.B != "test")
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
      SLPrincipal.Login("ClassARole", (o, e2) =>
      {
        bool pass = true;

        try
        {
          ClassB1 classB1 = new ClassB1();
          try
          {
            classB1.A = "test";
            pass = false;
          }
          catch (SecurityException ex)
          { }
          classB1.B = "test";
          if (classB1.B != "test")
            pass = false;

          ClassB2 classB2 = new ClassB2();
          try
          {
            classB2.A = "test";
            pass = false;
          }
          catch (SecurityException ex)
          { }
          classB2.B = "test";
          if (classB2.B != "test")
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
      SLPrincipal.Login("PropertyARole", (o, e2) =>
      {
        bool pass = true;

        try
        {
          try
          {
            ClassC1 classC1 = new ClassC1();
            pass = false;
          }
          catch (SecurityException ex)
          { }

          ClassC2 classC2 = new ClassC2();
          classC2.A = "test";
          classC2.B = "test";
          if (classC2.A != "test" || classC2.B != "test")
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
      SLPrincipal.Login("", (o, e2) =>
      {
        bool pass = true;

        try
        {
          try
          {
            ClassD1 classD1 = new ClassD1();
            pass = false;
          }
          catch (SecurityException ex)
          { }

          ClassD2 classD2 = new ClassD2();
          try
          {
            classD2.A = "test";
            pass = false;
          }
          catch (SecurityException ex)
          { }
          classD2.B = "test";
          if (classD2.B != "test")
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
