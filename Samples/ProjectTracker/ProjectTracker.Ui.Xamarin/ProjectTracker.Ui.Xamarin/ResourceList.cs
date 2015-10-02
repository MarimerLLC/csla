using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class ResourceList : ContentPage
  {
    public ResourceList()
    {
      Title = "Resource List";

      Content = new StackLayout();
    }

    protected async override void OnAppearing()
    {
      var resources = await Library.ResourceList.GetResourceListAsync();
      var list = new ListView();
      list.ItemTemplate = new DataTemplate(() =>
      {
        var cell = new TextCell();
        cell.SetBinding<Library.ResourceInfo>(TextCell.TextProperty, m => m.Name);
        return cell;
      });
      list.ItemsSource = resources;

      var stack = (StackLayout)Content;
      stack.Children.Add(list);

      base.OnAppearing();
    }
  }
}
