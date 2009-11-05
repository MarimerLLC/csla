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
using System.ComponentModel;

namespace Sample
{
  public partial class Page : UserControl
  {
    private Company _companyData;
    private bool _isCompanyDeleted = false;


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

    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (_companyData.IsDirty)
      {
        this.CancelButton.IsEnabled = true;
      }
      if (_companyData.IsValid)
      {
        this.SaveButton.IsEnabled = true;
      }
      else
      {
        this.SaveButton.IsEnabled = false;
      }
    }

    private void AttachHandlers(Company companyInfo)
    {
      if (_companyData != null)
      {
        _companyData.PropertyChanged -= HandlePropertyChanged;
      }
      _companyData = companyInfo;
      if (companyInfo != null)
      {
        _companyData.BeginEdit();
        _companyData.PropertyChanged += HandlePropertyChanged;
        SetInitialButtonStates();
      }
      else
      {
        DisableButtons();
      }
    }

    private void SetInitialButtonStates()
    {
      this.SaveButton.IsEnabled = false;
      this.CancelButton.IsEnabled = false;
      this.CreateButton.IsEnabled = true;
      this.DeleteButton.IsEnabled = true;
    }

    private void DisableButtons()
    {
      this.SaveButton.IsEnabled = false;
      this.CancelButton.IsEnabled = false;
      this.CreateButton.IsEnabled = false;
      this.DeleteButton.IsEnabled = false;
    }

    private void SetButtonsForDelete()
    {
      this.SaveButton.IsEnabled = true;
      this.CancelButton.IsEnabled = true;
      this.CreateButton.IsEnabled = false;
      this.DeleteButton.IsEnabled = false;
    }

    private void BindData(object sender, Csla.DataPortalResult<Company> e)
    {
      if (e.Error == null && e.Object != null)
      {
        SetInitialButtonStates();
        AttachHandlers(e.Object);
      }
      else
      {
        System.Windows.Browser.HtmlPage.Window.Alert(e.Error.Message);
        DisableButtons();
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
          SetInitialButtonStates();
        }
      }
    }

    private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      if (_companyData != null)
      {
        this.screenBusy.IsRunning = true;
        _isCompanyDeleted = _companyData.IsDeleted;
        Company tempCompany = _companyData.Clone();
        tempCompany.ApplyEdit();
        tempCompany.BeginSave(QueryCompleted);
      }
    }

    private void QueryCompleted(object sender, Csla.Core.SavedEventArgs e)
    {
      this.screenBusy.IsRunning = false;
      if (e.Error == null && e.NewObject != null)
      {
        AttachHandlers((Company)e.NewObject);
        this.LayoutRoot.DataContext = _companyData;
        if (_isCompanyDeleted)
        {
          ShowData(this, EventArgs.Empty);
        }
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
        SetButtonsForDelete();
      }
    }

  }
} //end of root namespace