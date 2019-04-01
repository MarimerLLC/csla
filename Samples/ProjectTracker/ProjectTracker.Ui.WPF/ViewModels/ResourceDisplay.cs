using System;

namespace WpfUI.ViewModels
{
  public class ResourceDisplay : ViewModel<ProjectTracker.Library.ResourceGetter>
  {
    public ResourceDisplay(int id)
    {
      ManageObjectLifetime = false;
      BeginRefresh(callback => ProjectTracker.Library.ResourceGetter.GetExistingResource(id, callback));
    }

    private ProjectTracker.Library.ResourceAssignmentEdit _assignment;
    public ProjectTracker.Library.ResourceAssignmentEdit SelectedAssignment
    {
      get { return _assignment; }
      set { _assignment = value; OnPropertyChanged("SelectedAssignment"); }
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
