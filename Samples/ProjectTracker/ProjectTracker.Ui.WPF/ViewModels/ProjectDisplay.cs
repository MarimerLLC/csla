using System;

namespace WpfUI.ViewModels
{
  public class ProjectDisplay : ViewModel<ProjectTracker.Library.ProjectEdit>
  {
    public ProjectDisplay(int id)
    {
      ManageObjectLifetime = false;
      var task = RefreshAsync<ProjectTracker.Library.ProjectEdit>(async () =>
        await ProjectTracker.Library.ProjectGetter.GetExistingProject(id));
    }

    private ProjectTracker.Library.ProjectResourceEdit _resource;
    public ProjectTracker.Library.ProjectResourceEdit SelectedResource
    {
      get { return _resource; }
      set { _resource = value; OnPropertyChanged(nameof(SelectedResource)); }
    }

    public void ShowResource()
    {
      if (SelectedResource != null)
        Bxf.Shell.Instance.ShowView(
          typeof(Views.ResourceDisplay).AssemblyQualifiedName,
          "resourceDisplayViewSource",
          new ResourceDisplay(SelectedResource.ResourceId),
          "Main");
    }
  }
}
