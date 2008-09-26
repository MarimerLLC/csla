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
        if (e1.ControlTypeName == typeof(ControlTwo).AssemblyQualifiedName)
          e1.Parameters = "Parameter=" + (new Random()).Next(1,100).ToString();
      };
    }

    private void ControlOne_Click(object sender, RoutedEventArgs e)
    {
      Navigator.Current.Navigate(typeof(ControlOne).AssemblyQualifiedName);
    }

    private void ControlTwo_Click(object sender, RoutedEventArgs e)
    {
      Navigator.Current.Navigate(typeof(ControlTwo).AssemblyQualifiedName);
    }
  }
}
