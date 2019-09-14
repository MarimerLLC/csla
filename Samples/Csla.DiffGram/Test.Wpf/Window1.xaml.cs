using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test.Wpf
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      var dp = Resources["Order"] as Csla.Xaml.CslaDataProvider;
      using (dp.DeferRefresh())
      {
        dp.ObjectType = typeof(Test.Library.OrderEdit);
        dp.FactoryMethod = "GetObject";
        dp.FactoryParameters.Clear();
        dp.FactoryParameters.Add(123);
      }
    }
  }
}
