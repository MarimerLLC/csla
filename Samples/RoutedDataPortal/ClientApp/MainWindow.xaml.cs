using BusinessLibrary;
using Csla.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApp
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
      Clear();

      var url = this.ServerUrl.Text;
      var version = this.AppVersion.Text;
      WriteLine($"URL: {url}");
      WriteLine($"Version: {version}");
      WriteLine();

      try
      {
        CustomProxy.ServerUrl = url;
        CslaConfiguration.Configure().VersionRoutingTag(version);

        var obj = await Csla.DataPortal.FetchAsync<TestClass>();
        WriteLine($"Created from {obj.CreatedFrom}");
      }
      catch (Exception ex)
      {
        WriteLine($"EXCEPTION: {ex.ToString()}");
      }
      WriteLine();
    }

    private void WriteLine(string text)
    {
      Write(text);
      WriteLine();
    }

    private void WriteLine()
    {
      Write(Environment.NewLine);
    }

    private void Write(string text)
    {
      OutputText.Text += text;
    }

    private void Clear()
    {
      OutputText.Text = "";
    }
  }
}
