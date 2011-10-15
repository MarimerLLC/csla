namespace WpUI.ViewModels
{
  public class ProjectEdit : ViewModel<ProjectTracker.Library.ProjectGetter>
  {
    public ProjectEdit(string queryString)
    {
      var p = queryString.Split('=');
      var projectId = int.Parse(p[1]);
      ManageObjectLifetime = false;
      BeginRefresh(callback => ProjectTracker.Library.ProjectGetter.GetExistingProject(projectId, callback));
    }

    internal void Close()
    {
      Bxf.Shell.Instance.ShowView(null, "Dialog");
    }
  }
}
