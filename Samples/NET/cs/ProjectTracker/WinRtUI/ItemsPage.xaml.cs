using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace WinRtUI
{
  /// <summary>
  /// A page that displays a collection of item previews.  In the Split Application this page
  /// is used to display and select one of the available groups.
  /// </summary>
  public sealed partial class ItemsPage : WinRtUI.Common.LayoutAwarePage
  {
    public ItemsPage()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.  The Parameter
    /// property provides the collection of items to be displayed.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      this.DefaultViewModel["Items"] = e.Parameter;

      //ProjectTracker.Library.ProjectList.GetProjectList((o, a) =>
      //  {
      //    if (a.Error != null)
      //      System.Diagnostics.Debug.Assert(false, a.Error.Message);
      //    else
      //      this.DataContext = a.Object;
      //  });
    }

    /// <summary>
    /// Invoked when an item is clicked.
    /// </summary>
    /// <param name="sender">The GridView (or ListView when the application is snapped)
    /// displaying the item clicked.</param>
    /// <param name="e">Event data that describes the item clicked.</param>
    void ItemView_ItemClick(object sender, ItemClickEventArgs e)
    {
      // Navigate to the appropriate destination page, configuring the new page
      // by passing required information as a navigation parameter
      this.Frame.Navigate(typeof(SplitPage), e.ClickedItem);
    }
  }
}
