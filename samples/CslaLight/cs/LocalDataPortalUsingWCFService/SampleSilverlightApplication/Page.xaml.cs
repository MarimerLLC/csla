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
using WcfService.Business.Client;

namespace SampleSilverlightApplication
{
  public partial class Page : UserControl
  {

    private bool _reloadInitalCompany = false;

    public Page()
    {
      this.Loaded += new System.Windows.RoutedEventHandler(Page_Loaded);
      InitializeComponent();
    }

    private void CslaDataProvider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsBusy")
      {
        CslaDataProvider provider = (CslaDataProvider)(this.Resources["CompanyData"]);
        if (provider.Data is Csla.Core.ITrackStatus)
        {
          if (((Csla.Core.ITrackStatus)provider.Data).IsDeleted)
          {
            _reloadInitalCompany = true;
          }
          else
          {
            _reloadInitalCompany = false;
          }
        }
      }
    }

    private void CslaDataProvider_DataChanged(object sender, System.EventArgs e)
    {
      CslaDataProvider provider = (CslaDataProvider)(this.Resources["CompanyData"]);
      if (provider.Error != null)
      {
        System.Windows.Browser.HtmlPage.Window.Alert(provider.Error.Message);
      }
      else
      {
        if (_reloadInitalCompany)
        {
          StartShowingData();
        }
      }
    }

    private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      this.objectBusy.IsRunning = true;
      SamplePrincipal.Login("admin", "admin", (o1, e1) => StartShowingData());
    }

    private bool StartShowingData()
    {
      Dispatcher.BeginInvoke(ShowData);
      return true;
    }

    private void ShowData()
    {
      this.objectBusy.IsRunning = false;
      CslaDataProvider provider = (CslaDataProvider)(this.Resources["CompanyData"]);
      provider.FactoryMethod = "GetCompany";
      provider.FactoryParameters.Add(2);
      provider.Refresh();
    }

    private void CreateButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      CslaDataProvider provider = (CslaDataProvider)(this.Resources["CompanyData"]);
      provider.FactoryParameters.Clear();
      provider.FactoryMethod = "CreateCompany";
      provider.Refresh();
    }

  }
} //end of root namespace