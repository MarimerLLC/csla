using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class ProjectList : ContentPage
  {
    public ProjectList ()
    {
      Title = "Project List";

      Content = new StackLayout();
    }

    private bool _loaded = false;

    protected async override void OnAppearing()
    {
      if (!_loaded)
      {
        _loaded = true;
        var projects = await Library.ProjectList.GetProjectListAsync();
        var list = new ListView();
        list.ItemTemplate = new DataTemplate(() =>
        {
          var cell = new TextCell();
          cell.SetBinding<Library.ProjectInfo>(TextCell.TextProperty, m => m.Name);
          return cell;
        });
        list.ItemsSource = projects;
        list.ItemTapped +=
          async (a, b) => await Navigation.PushAsync(new ProjectEdit(((ListView)a).SelectedItem));

        var stack = (StackLayout)Content;
        stack.Children.Add(list);
      }
      base.OnAppearing();
    }
  }
}
