using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpUI
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

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
      ErrorText.Text += Csla.ApplicationContext.User.Identity.ToString();
      ErrorText.Text += Environment.NewLine;
      ErrorText.Text += Environment.NewLine;
      try
      {
        //var obj = new ProjectTracker.Library.Dashboard();
        //var obj2 = Csla.Core.ObjectCloner.Clone(obj);

        //this.DataContext = await ProjectTracker.Library.Dashboard.GetDashboardAsync();

        var vm = await new ViewModel.DashboardViewModel().InitAsync();
        this.DataContext = vm;

        if (vm.Error == null)
        {
          ErrorText.Text += "Success";
          ErrorText.Text += Environment.NewLine;
        }
        else
        {
          ShowError(vm.Error);
        }
      }
      catch (Exception ex)
      {
        ShowError(ex);
      }
    }

    private void ShowError(Exception ex)
    {
      //await new MessageDialog(ex.ToString(), "Data error").ShowAsync();
      ErrorText.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
      ErrorText.Text += ex.ToString();
    }
  }
}
