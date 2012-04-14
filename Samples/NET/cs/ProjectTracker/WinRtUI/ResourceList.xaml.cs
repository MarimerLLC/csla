using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace WinRtUI
{
  /// <summary>
  /// A page that displays a group title, a list of items within the group, and details for
  /// the currently selected item.
  /// </summary>
  public sealed partial class ResourceList : WinRtUI.Common.LayoutAwarePage
  {
    public ResourceList()
    {
      this.InitializeComponent();
      itemDetailGrid.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.  The
    /// Parameter property provides the group to be displayed.</param>
    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
      // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
      // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
      this.Progress.IsIndeterminate = true;
      this.DataContext = await new ViewModel.ResourceListViewModel().InitAsync();
      this.Progress.IsIndeterminate = false;
      itemDetailGrid.Visibility = Visibility.Visible;

      // Select the first item automatically unless logical page navigation is
      // being used (see the logical page navigation #region below.)
      if (this.itemsViewSource.View != null)
        if (!this.UsingLogicalPageNavigation()) this.itemsViewSource.View.MoveCurrentToFirst();
    }

    #region Logical page navigation

    // Visual state management typically reflects the four application view states directly
    // (full screen landscape and portrait plus snapped and filled views.)  The split page is
    // designed so that the snapped and portrait view states each have two distinct sub-states:
    // either the item list or the details are displayed, but not both at the same time.
    //
    // This is all implemented with a single physical page that can represent two logical
    // pages.  The code below achieves this goal without making the user aware of the
    // distinction.

    /// <summary>
    /// Invoked to determine whether the page should act as one logical page or two.
    /// </summary>
    /// <param name="viewState">The view state for which the question is being posed, or null
    /// for the current view state.  This parameter is optional with null as the default
    /// value.</param>
    /// <returns>True when the view state in question is portrait or snapped, false
    /// otherwise.</returns>
    private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
    {
      if (viewState == null) viewState = ApplicationView.Value;
      return viewState == ApplicationViewState.FullScreenPortrait ||
          viewState == ApplicationViewState.Snapped;
    }

    /// <summary>
    /// Invoked when an item within the list is selected.
    /// </summary>
    /// <param name="sender">The GridView (or ListView when the application is Snapped)
    /// displaying the selected item.</param>
    /// <param name="e">Event data that describes how the selection was changed.</param>
    void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // Invalidate the view state when logical page navigation is in effect, as a change
      // in selection may cause a corresponding change in the current logical page.  When
      // an item is selected this has the effect of changing from displaying the item list
      // to showing the selected item's details.  When the selection is cleared this has the
      // opposite effect.
      if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();
    }

    /// <summary>
    /// Invoked when the page's back button is pressed.
    /// </summary>
    /// <param name="sender">The back button instance.</param>
    /// <param name="e">Event data that describes how the back button was clicked.</param>
    protected override void GoBack(object sender, RoutedEventArgs e)
    {
      if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
      {
        // When logical page navigation is in effect and there's a selected item that
        // item's details are currently displayed.  Clearing the selection will return to
        // the item list.  From the user's point of view this is a logical backward
        // navigation.
        this.itemListView.SelectedItem = null;
      }
      else
      {
        // When logical page navigation is not in effect, or when there is no selected
        // item, use the default back button behavior.
        base.GoBack(sender, e);
      }
    }

    /// <summary>
    /// Invoked to determine the name of the visual state that corresponds to an application
    /// view state.
    /// </summary>
    /// <param name="viewState">The view state for which the question is being posed.</param>
    /// <returns>The name of the desired visual state.  This is the same as the name of the
    /// view state except when there is a selected item in portrait and snapped views where
    /// this additional logical page is represented by adding a suffix of _Detail.</returns>
    protected override string DetermineVisualState(ApplicationViewState viewState)
    {
      // Update the back button's enabled state when the view state changes
      var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
      var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
      this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

      // Start with the default visual state name, and add a suffix when logical page
      // navigation is in effect and we need to display details instead of the list
      var defaultStateName = base.DetermineVisualState(viewState);
      return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
    }

    #endregion

    private async void SaveResource(object sender, RoutedEventArgs e)
    {
      var ctl = (Button)sender;
      var vm = (ViewModel.ResourceEditViewModel)ctl.DataContext;
      this.Progress.IsIndeterminate = true;
      await vm.SaveAsync();
      this.Progress.IsIndeterminate = false;
      await new Windows.UI.Popups.MessageDialog("Save completed", "Resource").ShowAsync();
    }
  }
}
