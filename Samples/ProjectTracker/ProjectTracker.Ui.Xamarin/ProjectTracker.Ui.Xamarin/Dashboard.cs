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
      var mainGrid = new Grid();
      mainGrid.RowDefinitions.Add(new RowDefinition { Height = 20 });
      mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

      stackLayout = new StackLayout
      {
        VerticalOptions = LayoutOptions.Center,
        Spacing = 2,
        Children = {
            new Label {
              XAlign = TextAlignment.Center,
              Text = "Project Tracker - Loading ..."
            }
          }
      };

      mainGrid.Children.Add(stackLayout, 0, 1);
      Content = mainGrid;
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
          XAlign = TextAlignment.Start,
          TextColor = Color.Red,
          Text = ex.ToString()
        });
      }

      stackLayout.Children.Clear();
      stackLayout.VerticalOptions = LayoutOptions.Start;
      stackLayout.Children.Add(new Label
      {
        XAlign = TextAlignment.Start,
        Text = string.Format("Projects {0}", dashboard.ProjectCount)
      });
      stackLayout.Children.Add(new Label
      {
        XAlign = TextAlignment.Start,
        Text = string.Format("Open projects {0}", dashboard.OpenProjectCount)
      });
      stackLayout.Children.Add(new Label
      {
        XAlign = TextAlignment.Start,
        Text = string.Format("Resources {0}", dashboard.ResourceCount)
      });
    }
  }
}
