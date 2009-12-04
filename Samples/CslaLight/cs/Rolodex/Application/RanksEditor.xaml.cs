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
  public partial class RanksEditor : UserControl
  {
    public RanksEditor()
    {
      InitializeComponent();
    }

    public event EventHandler CloseRequested;

    private void CslaDataProvider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Error" && ((Csla.Silverlight.CslaDataProvider)sender).Error != null)
        System.Windows.Browser.HtmlPage.Window.Alert(((Csla.Silverlight.CslaDataProvider)sender).Error.Message);
    }

    private void AddRank_Click(object sender, RoutedEventArgs e)
    {

    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      if (CloseRequested != null)
        CloseRequested.Invoke(this, EventArgs.Empty);
    }
  }
}
