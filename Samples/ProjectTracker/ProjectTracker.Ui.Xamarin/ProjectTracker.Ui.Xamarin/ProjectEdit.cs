using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class ProjectEdit : ContentPage
  {
    private Library.ProjectEdit _project;
    private PropertyStatus _nameStatus;
    Label label = new Label();
    public ProjectEdit(Library.ProjectEdit obj)
    {
      _project = obj;
      _nameStatus = new PropertyStatus(_project, "Name");
      _nameStatus.PropertyChanged += _nameStatus_PropertyChanged;
      label.IsVisible = !_nameStatus.IsValid;

      Title = "Edit project";

      var tbi = new ToolbarItem();
      tbi.Text = "Save";
      tbi.Clicked += Save_Clicked;
      ToolbarItems.Add(tbi);
    }

    private void _nameStatus_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      label.IsVisible = !_nameStatus.IsValid;
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      if (BindingContext == null)
      {
        BindingContext = _project;

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = 150 });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.VerticalOptions = LayoutOptions.StartAndExpand;
        grid.Children.Add(new Label { Text = "Name" }, 0, 0);
        var editor = new Editor();
        editor.SetBinding(Editor.TextProperty, "Name");
        grid.Children.Add(editor, 1, 0);
        label.IsVisible = false;
        label.Text = "Error in value";
        label.TextColor = Color.Red;
        grid.Children.Add(label, 1, 1);
        grid.Children.Add(new Label { Text = "Description" }, 0, 2);
        grid.Children.Add(new Editor { Text = _project.Description }, 1, 2);
        grid.Children.Add(new Label { Text = "Started" }, 0, 3);
        grid.Children.Add(new Editor { Text = _project.Started.HasValue ? _project.Started.GetValueOrDefault().ToString("d") : string.Empty }, 1, 3);
        grid.Children.Add(new Label { Text = "Ended" }, 0, 4);
        grid.Children.Add(new Editor { Text = _project.Ended.HasValue ? _project.Ended.GetValueOrDefault().ToString("d") : string.Empty }, 1, 4);
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

  public class PropertyStatus : INotifyPropertyChanged
  {
    Library.ProjectEdit _project;
    string _propertyName;
    public PropertyStatus(Library.ProjectEdit project, string propertyName)
    {
      _propertyName = propertyName;
      _project = project;
      _project.PropertyChanged += _project_PropertyChanged;

    }

    public bool IsValid { get; private set; }

    private void _project_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == _propertyName)
      {
        var br = _project.BrokenRulesCollection.Where(r => r.Property == _propertyName);
        IsValid = br.Count() == 0;
        OnPropertyChanged("IsValid");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
