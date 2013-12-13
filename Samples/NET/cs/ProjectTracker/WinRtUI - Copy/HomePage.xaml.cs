using WinRtUI.Data;

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
using WinRtUI.ViewModel;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace WinRtUI
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HomePage : WinRtUI.Common.LayoutAwarePage
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var dataGroups = ProjectTrackerDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = dataGroups;
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
          // Determine what group the Button instance represents
          var group = (DataGroup)(sender as FrameworkElement).DataContext;

          // Navigate to the appropriate destination page, configuring the new page
          // by passing required information as a navigation parameter
          if (group.UniqueId == "Projects")
            this.Frame.Navigate(typeof(ProjectList));
          else if (group.UniqueId == "Resources")
            this.Frame.Navigate(typeof(ResourceList));
          else if (group.UniqueId == "Roles")
            this.Frame.Navigate(typeof(RoleList));
        }
    }
}
