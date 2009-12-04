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


    public Page()
    {
      this.Loaded += new System.Windows.RoutedEventHandler(Page_Loaded);
      InitializeComponent();
    }

    private void CslaDataProvider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {

    }

    private void CslaDataProvider_DataChanged(object sender, System.EventArgs e)
    {
      CslaDataProvider provider = (CslaDataProvider)(this.Resources["CompanyData"]);
      if (provider.Error != null)
      {
        System.Windows.Browser.HtmlPage.Window.Alert(provider.Error.Message);
      }
    }

    private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      this.objectBusy.IsRunning = true;
      SamplePrincipal.Login("admin", "admin", (o1, e1) => ShowData());
    }

    private bool ShowData()
    {
      this.objectBusy.IsRunning = false;
      CslaDataProvider provider = (CslaDataProvider)(this.Resources["CompanyData"]);
      provider.FactoryParameters.Add(2);
      provider.Refresh();
      return true;
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