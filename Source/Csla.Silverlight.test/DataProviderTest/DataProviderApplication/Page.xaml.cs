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
using ClassLibrary.Business;

namespace DataProviderApplication
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
    }

    private bool _editing = false;

    private void CslaDataProvider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Error" && ((Csla.Silverlight.CslaDataProvider)sender).Error != null)
        System.Windows.Browser.HtmlPage.Window.Alert(((Csla.Silverlight.CslaDataProvider)sender).Error.Message);

    }

    private void ContactsGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
      _editing = true;
    }

    private void ContactsGrid_CancelingEdit(object sender, DataGridEndingEditEventArgs e)
    {
      _editing = false;
    }

    private void ContactsGrid_CommittingEdit(object sender, DataGridEndingEditEventArgs e)
    {
      _editing = false;
    }

    private void ContactsGrid_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Delete && !_editing)
      {
        if (ContactsGrid.SelectedItem != null)
        {
          ((Customer)((Csla.Silverlight.CslaDataProvider)this.Resources["CustomerData"]).Data).Contacts.Remove((CustomerContact)ContactsGrid.SelectedItem);
        }
      }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      //BusinessPrincipal.Login("SergeyB", "1234", "admin;user", (o2, e2) =>
      //{
      //  if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
      //  {
      //    Customer.GetCustomer(((new Random()).Next(1, 10)), (o1, e1) =>
      //      {
      //        if (e1.Error == null)
      //          ((Csla.Silverlight.CslaDataProvider)this.Resources["CustomerData"]).Data = e1.Object;
      //        else
      //          System.Windows.Browser.HtmlPage.Window.Alert("Unable to get data");
      //      });
      //  }
      //  else
      //  {
      //    System.Windows.Browser.HtmlPage.Window.Alert("Unable to login: ");
      //  }
      //});
    }

    private void ContactsDetailsGrid_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Delete && !_editing)
      {
        if (ContactsGrid.SelectedItem != null)
        {
          ((CustomerList)((Csla.Silverlight.CslaDataProvider)this.Resources["CustomerListData"]).Data).Remove((Customer)ContactsDetailsGrid.SelectedItem);
        }
      }
    }

    private void CslaDataProvider_DataChanged(object sender, EventArgs e)
    {
      if (((Csla.Silverlight.CslaDataProvider)sender).Data != null && Csla.ApplicationContext.GlobalContext.Count > 0)
      {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        builder.AppendLine("Global Context Information");
        builder.AppendLine("");
        foreach (var oneKey in Csla.ApplicationContext.GlobalContext.Keys)
        {
          builder.AppendLine(oneKey.ToString() + " = " + Csla.ApplicationContext.GlobalContext[oneKey].ToString());
        }
        System.Windows.Browser.HtmlPage.Window.Alert(builder.ToString());
      }
      Csla.ApplicationContext.GlobalContext.Clear();
    }
  }
}
