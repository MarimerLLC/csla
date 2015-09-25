using Csla;
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

    protected async override void OnAppearing()
    {
      var projects = await Library.ProjectList.GetProjectListAsync();
      var list = new ListView();
      list.ItemTemplate = new DataTemplate(() =>
      {
        var cell = new TextCell();
        cell.SetBinding<Library.ProjectInfo>(TextCell.TextProperty, m => m.Name);
        return cell;
      });
      list.ItemsSource = projects;

      var stack = (StackLayout)Content;
      stack.Children.Add(list);

      base.OnAppearing();
    }
  }
}
