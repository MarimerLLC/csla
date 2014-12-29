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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinRTUI
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
      this.DataContext = await new OrderVm().InitAsync();
    }

    private async void SaveObject(object sender, RoutedEventArgs e)
    {
      await ((OrderVm)this.DataContext).SaveAsync();
    }

    private void CancelEdit(object sender, RoutedEventArgs e)
    {
      ((OrderVm)this.DataContext).Cancel();
    }

    private void EditLineItem(object sender, ItemClickEventArgs e)
    {
      var lineItem = e.ClickedItem as BusinessLibrary.LineItem;
      if (lineItem != null)
      {
        var uc = new LineItemEditor(sender, lineItem);
        uc.Closed += (o, a) =>
          {
            SaveCancelRegion.Visibility = Windows.UI.Xaml.Visibility.Visible;
          };
        SaveCancelRegion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        uc.Show();
      }
    }
  }
}
