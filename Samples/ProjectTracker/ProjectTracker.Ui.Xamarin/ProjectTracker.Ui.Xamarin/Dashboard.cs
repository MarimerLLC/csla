using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class Dashboard : ContentPage
  {
    StackLayout stackLayout = null;
    public Dashboard()
    {
      Title = "Dashboard";

      stackLayout = new StackLayout
      {
        VerticalOptions = LayoutOptions.Center,
        Spacing = 5,
        Children = {
            new Label {
              HorizontalTextAlignment = TextAlignment.Center,
              Text = "Project Tracker - Loading ..."
            }
          }
      };

      Content = stackLayout;
    }

    public async Task LoadData()
    {
      Library.Dashboard dashboard = null;
      try
      {
        dashboard = await Csla.DataPortal.FetchAsync<Library.Dashboard>();
      }
      catch (Exception ex)
      {
        stackLayout.Children.Add(new Label
        {
          HorizontalTextAlignment = TextAlignment.Start,
          TextColor = Color.Red,
          Text = ex.ToString()
        });
      }

      stackLayout.Children.Clear();
      stackLayout.VerticalOptions = LayoutOptions.Start;
      CreateRow("Projects", dashboard.ProjectCount, typeof(ProjectList));
      CreateRow("Open projects", dashboard.OpenProjectCount, typeof(ProjectList));
      CreateRow("Resources", dashboard.ResourceCount, typeof(ResourceList));
    }

    private void CreateRow(string label, int count, Type targetPage)
    {
      var child = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 5 };
      var button = new Button
      {
        Text = "View"
      };
      button.Clicked += async (a, b) => await Navigation.PushAsync(
        (Page)Activator.CreateInstance(targetPage));
      child.Children.Add(button);
      child.Children.Add(new Label
      {
        HorizontalTextAlignment = TextAlignment.Start,
        Text = string.Format("{0} {1}", label, count)
      });
      stackLayout.Children.Add(child);
    }
  }
}
