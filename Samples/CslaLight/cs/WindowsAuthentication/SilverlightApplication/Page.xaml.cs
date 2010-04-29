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
      Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://localhost/SilverlightApplicationWeb/WcfPortal.svc";
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

    private void btnFetch_Click(object sender, RoutedEventArgs e)
    {
      ClassA1.GetA("1", (o1, e1) =>
        {
          if (e1.Error == null)
          {
            txtFetch.Text = "Succefful fetch " + e1.Object.A;
          }
          else
          {
            txtFetch.Text = "Failed fetch";
          }
        });
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      ClassA1.DeleteA("1", (o1, e1) =>
       {
         if (e1.Error == null)
         {
           txtDelete.Text = "Succefful delete ";
         }
         else
         {
           txtDelete.Text = "Failed delete";
         }
       });
    }

    private void btnCreate_Click(object sender, RoutedEventArgs e)
    {
      ClassA1.CreateA("1", (o1, e1) =>
      {
        if (e1.Error == null)
        {
          txtCreate.Text = "Succefful create " + e1.Object.A;
        }
        else
        {
          txtCreate.Text = "Failed create";
        }
      });
    }

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
      ClassA1 a = new ClassA1();
      a.MarkMeDirty();
      a.BeginSave(true, (o1, e1) =>
      {
        if (e1.Error == null)
        {
          txtUpdate.Text = "Succefful save " + (e1.NewObject as ClassA1).A;
        }
        else
        {
          txtUpdate.Text = "Failed save" + (e1.NewObject as ClassA1).A;
        }
      });
    }


  }
}
