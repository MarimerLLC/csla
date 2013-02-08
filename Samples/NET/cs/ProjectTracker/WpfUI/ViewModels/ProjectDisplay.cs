using System;

namespace WpfUI.ViewModels
{
  public class ProjectDisplay : ViewModel<ProjectTracker.Library.ProjectGetter>
  {
    public ProjectDisplay(int id)
    {
      ManageObjectLifetime = false;
      BeginRefresh(callback => ProjectTracker.Library.ProjectGetter.GetExistingProject(id, callback));
    }

    private ProjectTracker.Library.ProjectResourceEdit _resource;
    public ProjectTracker.Library.ProjectResourceEdit SelectedResource
    {
      get { return _resource; }
      set { _resource = value; OnPropertyChanged("SelectedResource"); }
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
