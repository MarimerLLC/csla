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
using System.Windows.Browser;
using Csla.Silverlight;

namespace NavigationApp
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(Page_Loaded);
    }

    void Page_Loaded(object sender, RoutedEventArgs e)
    {
      Navigator.Current.ContentPlaceholder = this.PlaceHolder;
      Navigator.Current.BeforeNavigation += (o1, e1) =>
      {
        if (e1.ControlTypeName == typeof(ControlTwo).AssemblyQualifiedName && e1.IsInitiatedByBrowserButton == false)
          e1.Parameters = "Parameter=" + (new Random()).Next(1, 100).ToString();
        if (e1.ControlTypeName.ToUpper().Contains("ControlThree".ToUpper()))
        {
          e1.Cancel = true;
        }
        if (e1.ControlTypeName.ToUpper().Contains("ControlFour".ToUpper()))
        {
          e1.Cancel = true;
          BoomarkInformation bookmark = new BoomarkInformation("NavigationApp.ControlOne, NavigationApp, Version=..., Culture=neutral, PublicKeyToken=null", "", "COntrol One - Redirected");
          e1.RedirectToOnCancel = bookmark;
        }
      };
      Navigator.Current.ProcessInitialNavigation();
    }
  }
}
