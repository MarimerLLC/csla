using System;
using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class ResourceList : ContentPage
  {
    public ResourceList()
    {
      Title = "Resource List";
    }

    private bool _loaded = false;

    protected async override void OnAppearing()
    {
      if (!_loaded)
      {
        _loaded = true;
        var resources = await Library.ResourceList.GetResourceListAsync();
        var list = new ListView();
        list.ItemTemplate = new DataTemplate(() =>
        {
          var cell = new TextCell();
          cell.SetBinding<Library.ResourceInfo>(TextCell.TextProperty, m => m.Name);
          return cell;
        });
        list.ItemsSource = resources;

        var stack = new StackLayout();
        stack.Children.Add(list);
        Content = stack;
      }
      base.OnAppearing();
    }
  }
}
