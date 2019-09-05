using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using BusinessLibrary;
using Csla;
using Csla.Configuration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoExample
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      await RefreshData();
    }

    private async void Refresh(object sender, RoutedEventArgs e)
    {
      await RefreshData();
    }

    private async Task RefreshData()
    {
      this.InfoText.Text = "Loading ...";
      try
      {
        var obj = await DataPortal.FetchAsync<PersonList>();
        DataContext = obj;
        this.InfoText.Text = "Loaded";
      }
      catch (Exception ex)
      {
        OutputText.Text = ex.ToString();
      }
    }

    private void NewPerson(object sender, RoutedEventArgs e)
    {
      var rootFrame = Window.Current.Content as Frame;
      rootFrame.Navigate(typeof(EditPerson));
    }

    private void EditPerson(object sender, RoutedEventArgs e)
    {
      var id = (int)((Button)sender).Tag;
      var rootFrame = Window.Current.Content as Frame;
      rootFrame.Navigate(typeof(EditPerson), id);
    }
  }
}
