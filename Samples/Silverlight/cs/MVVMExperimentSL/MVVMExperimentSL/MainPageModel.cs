using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MVVMexperiment
{
  public class MainPageModel : FrameworkElement
  {
    public static readonly DependencyProperty CurrentControlProperty =
        DependencyProperty.Register("CurrentControl", typeof(UserControl), typeof(MainPageModel),
        new PropertyMetadata(null));

    public UserControl CurrentControl
    {
      get { return (UserControl)GetValue(CurrentControlProperty); }
      set { SetValue(CurrentControlProperty, value); }
    }

    private static MainPageModel _main;

    public MainPageModel()
    {
      _main = this;
      CurrentControl = new ListPage();
    }

    public static void ShowForm(UserControl form)
    {
      _main.CurrentControl = form;
    }
  }
}
