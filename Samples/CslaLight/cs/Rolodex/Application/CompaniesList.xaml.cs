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

namespace Rolodex
{
  public partial class CompaniesList : UserControl
  {
    public event EventHandler<CompanySelectedEventArgs> CompanySelected;
    public event EventHandler NewCompanyRequested;

    public event EventHandler ShowRanksRequested;

    public CompaniesList()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (CompanySelected!=null)
        CompanySelected.Invoke(this, new CompanySelectedEventArgs((int)((Button)sender).Tag));
    }

    private void CslaDataProvider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      //if (e.PropertyName == "Error" && ((Csla.Silverlight.CslaDataProvider)sender).Error != null)
      //  System.Windows.Browser.HtmlPage.Window.Alert(((Csla.Silverlight.CslaDataProvider)sender).Error.Message);

    }

    private void EditRanks_Click(object sender, RoutedEventArgs e)
    {
      if (ShowRanksRequested != null)
        ShowRanksRequested.Invoke(this, EventArgs.Empty);
    }

    private void NewCompany_Click(object sender, RoutedEventArgs e)
    {
      if (NewCompanyRequested != null)
        NewCompanyRequested.Invoke(this, EventArgs.Empty);
    }

    private void CslaDataProvider_DataChanged(object sender, EventArgs e)
    {
      if (((Csla.Silverlight.CslaDataProvider)sender).Error != null)
        System.Windows.Browser.HtmlPage.Window.Alert(((Csla.Silverlight.CslaDataProvider)sender).Error.Message);
    }
  }
}
