using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class ResourceEdit : ContentPage
  {
    private Library.ResourceEdit _resource;
    public ResourceEdit(Library.ResourceEdit obj)
    {
      _resource = obj;

      Title = "Edit resource";

      var tbi = new ToolbarItem();
      tbi.Text = "Save";
      tbi.Clicked += Save_Clicked;
      ToolbarItems.Add(tbi);
    }

    protected async override void OnAppearing()
    {
      base.OnAppearing();

      if (BindingContext == null)
      {
        var resource = await Library.ResourceEdit.GetResourceEditAsync(_resource.Id);
        BindingContext = resource;

        var grid = new Grid();
        grid.VerticalOptions = LayoutOptions.StartAndExpand;
        grid.Children.Add(new Label { Text = "First name" }, 0, 0);
        grid.Children.Add(new Editor { Text = resource.FirstName}, 1, 0);
        grid.Children.Add(new Label { Text = "Last name" }, 0, 1);
        grid.Children.Add(new Editor { Text = resource.LastName }, 1, 1);
        grid.Children.Add(new Label { Text = "Description" }, 0, 2);
        Content = grid;
      }
    }

    private async void Save_Clicked(object sender, System.EventArgs e)
    {
      var resource = BindingContext as Library.ResourceEdit;
      if (resource != null)
      {
        resource = await resource.SaveAsync();
        BindingContext = resource;
      }
    }

    public ResourceEdit()
    {
      throw new System.Exception("Invalid operation");
    }
  }
}
