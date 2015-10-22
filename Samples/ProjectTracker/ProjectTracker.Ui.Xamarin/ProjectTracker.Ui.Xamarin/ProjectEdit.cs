using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
	public class ProjectEdit : ContentPage
	{
    private Library.ProjectInfo _projectInfo;
    public ProjectEdit(object obj)
    {
      Title = "Edit project";

      var tbi = new ToolbarItem();
      tbi.Text = "Save";
      tbi.Clicked += Save_Clicked;
      ToolbarItems.Add(tbi);

      _projectInfo = (Library.ProjectInfo)obj;
    }

    private async void Save_Clicked(object sender, System.EventArgs e)
    {
      var project = BindingContext as Library.ProjectEdit;
      if (project != null)
      {
        project = await project.SaveAsync();
        BindingContext = project;
      }
    }

    public ProjectEdit ()
		{
      throw new System.Exception("Invalid operation");
		}

    protected async override void OnAppearing()
    {
      base.OnAppearing();

      if (BindingContext == null)
      {
        var project = await Library.ProjectEdit.GetProjectAsync(_projectInfo.Id);
        BindingContext = project;

        var grid = new Grid();
        grid.VerticalOptions = LayoutOptions.StartAndExpand;
        grid.Children.Add(new Label { Text = "Name" }, 0, 0);
        grid.Children.Add(new Editor { Text = project.Name }, 1, 0);
        grid.Children.Add(new Label { Text = "Description" }, 0, 1);
        grid.Children.Add(new Editor { Text = project.Description }, 1, 1);
        grid.Children.Add(new Label { Text = "Started" }, 0, 2);
        grid.Children.Add(new Editor { Text = project.Started.ToString() }, 1, 2);
        grid.Children.Add(new Label { Text = "Ended" }, 0, 3);
        grid.Children.Add(new Editor { Text = project.Ended.ToString() }, 1, 3);
        Content = grid;
      }
    }
  }
}
