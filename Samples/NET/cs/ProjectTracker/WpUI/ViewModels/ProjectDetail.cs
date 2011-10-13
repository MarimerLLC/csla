
namespace WpUI.ViewModels
{
  public class ProjectDetail : ViewModel<ProjectTracker.Library.ProjectGetter>
  {
    public ProjectDetail(string queryString)
    {
      var p = queryString.Split('=');
      var projectId = int.Parse(p[1]);
      ManageObjectLifetime = false;
      BeginRefresh(callback => ProjectTracker.Library.ProjectGetter.GetExistingProject(projectId, callback));
    }

    private ProjectTracker.Library.ProjectResourceEdit _resource;
    public ProjectTracker.Library.ProjectResourceEdit SelectedResource
    {
      get { return _resource; }
      set { _resource = value; OnPropertyChanged("SelectedResource"); }
    }

    public void ShowResource()
    {
      //if (SelectedResource != null)
      //  Bxf.Shell.Instance.ShowView(
      //    typeof(Views.ResourceDisplay).AssemblyQualifiedName,
      //    "resourceDisplayViewSource",
      //    new ResourceDisplay(SelectedResource.ResourceId),
      //    "Main");
    }
  }
}
