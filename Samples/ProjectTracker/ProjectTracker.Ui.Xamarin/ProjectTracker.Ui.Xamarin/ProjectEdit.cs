using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class ProjectEdit : ContentPage
  {
    private Library.ProjectEdit _project;
    public ProjectEdit(Library.ProjectEdit obj)
    {
      _project = obj;

      Title = "Edit project";

      var tbi = new ToolbarItem();
      tbi.Text = "Save";
      tbi.Clicked += Save_Clicked;
      ToolbarItems.Add(tbi);
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      if (BindingContext == null)
      {
        BindingContext = _project;

        var grid = new Grid();
        grid.VerticalOptions = LayoutOptions.StartAndExpand;
        grid.Children.Add(new Label { Text = "Name" }, 0, 0);
        var editor = new Editor();
        editor.SetBinding(Editor.TextProperty, "Name");
        grid.Children.Add(editor, 1, 0);
        grid.Children.Add(new Label { Text = "Description" }, 0, 1);
        grid.Children.Add(new Editor { Text = _project.Description }, 1, 1);
        grid.Children.Add(new Label { Text = "Started" }, 0, 2);
        grid.Children.Add(new Editor { Text = _project.Started.ToString() }, 1, 2);
        grid.Children.Add(new Label { Text = "Ended" }, 0, 3);
        grid.Children.Add(new Editor { Text = _project.Ended.ToString() }, 1, 3);
        Content = grid;
      }
    }

    private async void Save_Clicked(object sender, System.EventArgs e)
    {
      var project = BindingContext as Library.ProjectEdit;
      if (project != null)
      {
        project = await project.SaveAsync();
        BindingContext = project;
        await Navigation.PopAsync();
      }
    }

    public ProjectEdit ()
    {
      throw new System.Exception("Invalid operation");
    }
  }
}
