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

namespace BasicBinding
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class BlankPage : Page
  {
    public BlankPage()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.  The Parameter
    /// property is typically used to configure the page.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      DataItem.GetDataItem("Rocky", (o, a) =>
      {
        if (a.Error != null)
          System.Diagnostics.Debug.Assert(false);
        this.DataContext = a.Object;
      });
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      var obj = (DataItem)this.DataContext;
      Label1.Text = obj.Name;
    }
  }
}
