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
using Csla.Silverlight;
using Rolodex.Business.BusinessClasses;

namespace Rolodex
{
  public partial class CompanyEditor : UserControl
  {
    public CompanyEditor()
    {
      InitializeComponent();
    }

    private bool companyDataLoaded = false;

    public event EventHandler DataLoaded;
    private void OnDataLoaded()
    {
      if (companyDataLoaded && DataLoaded != null)
      {
        DataLoaded.Invoke(this, EventArgs.Empty);
      }
    }

    public event EventHandler CloseRequested;

    public void LoadCompanyData(int companyID)
    {
      CslaDataProvider provider = this.Resources["CompanyData"] as CslaDataProvider;
      provider.FactoryParameters.Add(companyID);
      provider.FactoryMethod = "GetCompany";
      provider.Refresh();
    }

    public void CreateNewCompanyData()
    {
      CslaDataProvider provider = ((CslaDataProvider)this.Resources["CompanyData"]);
      provider.FactoryMethod = "CreateCompany";
      provider.FactoryParameters.Clear();
      provider.Refresh();
    }

    private void CslaDataProvider_DataChanged(object sender, EventArgs e)
    {
      companyDataLoaded = true;
      OnDataLoaded();
      CslaDataProvider provider = ((CslaDataProvider)this.Resources["CompanyData"]);
      this.Contacts.SelectedItem = null;
      if (provider.Data != null && ((Company)(provider.Data)).Contacts.Count > 0)
      {
        this.Contacts.SelectedItem = ((Company)(provider.Data)).Contacts[0];
        this.Contacts.Focus();
      }
      if (provider.Error != null)
        System.Windows.Browser.HtmlPage.Window.Alert(((Csla.Silverlight.CslaDataProvider)sender).Error.Message);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      if (CloseRequested != null)
        CloseRequested.Invoke(this, EventArgs.Empty);

    }

    private void AddContactButton_Click(object sender, RoutedEventArgs e)
    {
      if (((CslaDataProvider)this.Resources["CompanyData"]).Data != null)
      {
        ((Company)((CslaDataProvider)this.Resources["CompanyData"]).Data).Contacts.AddNew();
      }
    }

    private void DeleteContact_Click(object sender, RoutedEventArgs e)
    {
      ((Company)((CslaDataProvider)this.Resources["CompanyData"]).Data).Contacts.Remove((CompanyContact)((Button)sender).Tag);
    }

    private void AddContactPhoneButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.Contacts.SelectedItem != null)
      {
        CompanyContact selectedContact = this.Contacts.SelectedItem as CompanyContact;
        selectedContact.ContactPhones.AddNew();
      }
    }

    private void DeleteContactPhone_Click(object sender, RoutedEventArgs e)
    {
      if (this.Contacts.SelectedItem != null)
      {
        CompanyContact selectedContact = this.Contacts.SelectedItem as CompanyContact;
        selectedContact.ContactPhones.Remove((CompanyContactPhone)((Button)sender).Tag);
      }
    }

    private void Contacts_SelectionChanged(object sender, EventArgs e)
    {
      if (this.Contacts.SelectedItem == null)
      {
        this.ContactsPhones.ItemsSource = null;
      }
      else
      {
        this.ContactsPhones.ItemsSource = ((CompanyContact)this.Contacts.SelectedItem).ContactPhones;
      }
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
      CslaDataProvider provider = ((CslaDataProvider)this.Resources["CompanyData"]);
      provider.FactoryMethod = "CreateCompany";
      provider.FactoryParameters.Clear();
      provider.Refresh();
    }
  }
}
