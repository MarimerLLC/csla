using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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

    public Xaml.NavigationHelper NavigationHelper { get; private set; }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      NavigationHelper = new Xaml.NavigationHelper().OnNavigatedTo(this, e);
      base.OnNavigatedTo(e);
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      var vm = new ViewModels.DashboardViewModel();
      DataContext = vm;
    }

    private void ShowProjectList(object sender, RoutedEventArgs e)
    {
      App.NavigateTo(typeof(Views.ProjectList));
    }

    private void ShowResourceList(object sender, RoutedEventArgs e)
    {
      App.NavigateTo(typeof(Views.ResourceList));
    }

    private void ViewRoles(object sender, RoutedEventArgs e)
    {
      App.NavigateTo(typeof(Views.RoleList));
    }
  }
}
