using System;
using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class ProjectList : ContentPage
  {
    public ProjectList ()
    {
      Title = "Project List";

      var tbi = new ToolbarItem();
      tbi.Text = "Add";
      tbi.Clicked += Add_Clicked;
      ToolbarItems.Add(tbi);
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
        list.ItemTapped += async (a, b) => 
        {
          var info = (Library.ProjectInfo)((ListView)a).SelectedItem;
          var project = await Library.ProjectEdit.GetProjectAsync(info.Id);
          await Navigation.PushAsync(new ProjectEdit(project));
        };

        var stack = new StackLayout();
        stack.Children.Add(list);
        Content = stack;
      }
      base.OnAppearing();
    }

    private async void Add_Clicked(object sender, System.EventArgs e)
    {
      try
      {
        var project = await Library.ProjectEdit.NewProjectAsync();
        var page = new ProjectEdit(project);
        await Navigation.PushAsync(page);
      }
      catch (Exception ex)
      {
        var obj = ex;
      }
    }
  }
}
