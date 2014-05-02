using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleApp
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

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.  The Parameter
    /// property is typically used to configure the page.</param>
    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
      ProgressDisplay.IsActive = true;
      try
      {
        var obj = await Library.CustomerEdit.GetCustomerEditAsync(441);
        obj.BeginEdit();
        this.DataContext = obj;
      }
      catch (Exception ex)
      {
        this.DataContext = null;
      }
      finally
      {
        ProgressDisplay.IsActive = false;
      }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      var obj = this.DataContext as Library.CustomerEdit;
      if (obj != null)
      {
        obj.CancelEdit();
        obj.BeginEdit();
      }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
      var obj = this.DataContext as Library.CustomerEdit;
      if (obj != null)
      {
        obj.ApplyEdit();
        ProgressDisplay.IsActive = true;
        obj = await obj.SaveAsync();
        obj.BeginEdit();
        this.DataContext = obj;
        ProgressDisplay.IsActive = false;
      }
    }
  }
}
