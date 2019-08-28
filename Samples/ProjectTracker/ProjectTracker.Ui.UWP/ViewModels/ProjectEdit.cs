namespace UwpUI.ViewModels
{
  public class ProjectEdit : ViewModel<ProjectTracker.Library.ProjectEdit>
  {
    public ProjectEdit(int id)
    {
      var task = RefreshAsync<ProjectTracker.Library.ProjectEdit>(async () =>
        await ProjectTracker.Library.ProjectEdit.GetProjectAsync(id));
    }
  }
}
