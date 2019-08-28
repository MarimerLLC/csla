using System;

namespace WpfUI.ViewModels
{
  public class ResourceDisplay : ViewModel<ProjectTracker.Library.ResourceEdit>
  {
    public ResourceDisplay(int id)
    {
      ManageObjectLifetime = false;
      var task = RefreshAsync<ProjectTracker.Library.ResourceEdit>(async () =>
        await ProjectTracker.Library.ResourceGetter.GetExistingResource(id));
    }

    private ProjectTracker.Library.ResourceAssignmentEdit _assignment;
    public ProjectTracker.Library.ResourceAssignmentEdit SelectedAssignment
    {
      get { return _assignment; }
      set { _assignment = value; OnPropertyChanged(nameof(SelectedAssignment)); }
    }

    public void ShowProject()
    {
      if (SelectedAssignment != null)
        Bxf.Shell.Instance.ShowView(
          typeof(Views.ProjectDisplay).AssemblyQualifiedName,
          "projectDisplayViewSource",
          new ProjectDisplay(SelectedAssignment.ProjectId),
          "Main");
    }
  }
}
