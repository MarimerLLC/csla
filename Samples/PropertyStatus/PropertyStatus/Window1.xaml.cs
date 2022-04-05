using System.Windows;
using Csla;

namespace PropertyStatus
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
      var portal = App.ApplicationContext.GetRequiredService<IDataPortal<Blah>>();
      Blah ex = portal.Fetch();
      ex.Data = "example...";
      DataContext = ex;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
    }
  }
}
