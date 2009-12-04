//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Csla.Silverlight;
using System.Collections.ObjectModel;
using Sample.Business;

namespace Sample
{
  public partial class Page : UserControl
  {
    private Company _companyData;
    private bool _reloadOldCompany = false;
    public Page()
    {
      this.Loaded += new System.Windows.RoutedEventHandler(Page_Loaded);
      InitializeComponent();
    }

    private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      this.screenBusy.IsRunning = true;
      SamplePrincipal.Login("admin", "admin", ShowData);
    }

    private void ShowData(object sender, EventArgs e)
    {
      this.screenBusy.IsRunning = true;
      Company.GetCompany(2, BindData);
    }

    private void BindData(object sender, Csla.DataPortalResult<Company> e)
    {
      if (e.Error == null && e.Object != null)
      {
        _companyData = e.Object;
        _companyData.BeginEdit();
      }
      else
      {
        System.Windows.Browser.HtmlPage.Window.Alert(e.Error.Message);
      }
      this.LayoutRoot.DataContext = _companyData;
      this.screenBusy.IsRunning = false;
    }

    private void CreateButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      this.screenBusy.IsRunning = true;
      Company.CreateCompany(BindData);
    }

    private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      if (_companyData != null)
      {
        if (_companyData.IsNew)
        {
          ShowData(this, EventArgs.Empty);
        }
        else
        {
          _companyData.CancelEdit();
          _companyData.BeginEdit();
        }
      }
    }

    private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      if (_companyData != null)
      {
        _reloadOldCompany = _companyData.IsDeleted;
        this.screenBusy.IsRunning = true;
        Company tempCompany  = _companyData.Clone();
        tempCompany.ApplyEdit();
        tempCompany.BeginSave(QueryCompleted);
      }
    }

    private void QueryCompleted(object sender, Csla.Core.SavedEventArgs e)
    {
      this.screenBusy.IsRunning = false;
      if (e.Error == null && e.NewObject != null)
      {
        _companyData = (Company)e.NewObject;
        _companyData.BeginEdit();
        this.LayoutRoot.DataContext = _companyData;
        if (_reloadOldCompany)
          ShowData(this, EventArgs.Empty);
      }
      else
      {
        System.Windows.Browser.HtmlPage.Window.Alert(e.Error.Message);
      }
    }

    private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      if (_companyData != null)
      {
        _companyData.Delete();
      }
    }

    private void AdminLoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      this.screenBusy.IsRunning = true;
      SamplePrincipal.Logout();
      SamplePrincipal.Login("admin", "admin", ShowData);
    }

    private void UserLoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      this.screenBusy.IsRunning = true;
      SamplePrincipal.Logout();
      SamplePrincipal.Login("user", "user", ShowData);
    }

    private void ReadOnlyLoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      this.screenBusy.IsRunning = true;
      SamplePrincipal.Logout();
      SamplePrincipal.Login("ReadOnlyUser", "readonly", ShowData);
    }
  }
} //end of root namespace