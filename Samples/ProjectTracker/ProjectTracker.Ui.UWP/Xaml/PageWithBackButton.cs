using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UwpUI.Xaml
{
  public class NavigationHelper
  {
    public Page Page { get; set; }

    public NavigationHelper OnNavigatedTo(Page page, NavigationEventArgs e)
    {
      Page = page;
      if (Page.Frame.CanGoBack)
        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
      else
        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
      SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
      return this;
    }

    private void BackRequested(object sender, BackRequestedEventArgs e)
    {
      if (Page.Frame.CanGoBack) Page.Frame.GoBack();
    }
  }
}
