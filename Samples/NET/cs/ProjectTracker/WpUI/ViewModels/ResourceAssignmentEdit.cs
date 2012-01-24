using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ResourceAssignmentEdit : ViewModelLocalEdit<ProjectTracker.Library.ResourceAssignmentEdit>
  {
    public ProjectTracker.Library.ResourceEdit ParentResource { get; private set; }
    public bool EditMode { get; private set; }

    public ResourceAssignmentEdit(ProjectTracker.Library.ResourceEdit parent)
    {
      ManageObjectLifetime = true;
      EditMode = false;
      ParentResource = parent;
      ProjectTracker.Library.ProjectList.GetProjectList((o, e) =>
      {
        if (e.Error != null)
          Bxf.Shell.Instance.ShowError("Error retrieving project list", "Data Error");
        else
        {
          var list = new ObservableCollection<ProjectInfo>();
          foreach (var item in e.Object)
            list.Add(new ProjectInfo(item, this));
          Projects = list;
        }
      });
    }

    public ResourceAssignmentEdit(ProjectTracker.Library.ResourceAssignmentEdit item)
    {
      ManageObjectLifetime = true;
      EditMode = true;
      ParentResource = (ProjectTracker.Library.ResourceEdit)((ProjectTracker.Library.ResourceAssignments)item.Parent).Parent;
      Model = item;
    }

    public void CreateResourceAssignment()
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Creating new assignment..." });
      DisplayIndex = 1;
      ProjectTracker.Library.ResourceAssignmentEditCreator.GetResourceAssignmentEditCreator(SelectedProject.Id, (o, e) =>
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
        if (e.Error != null)
          Bxf.Shell.Instance.ShowError(e.Error.Message, "Data error");
        else
          Model = e.Object.Result;
      });
    }

    private int _panel;
    public int DisplayIndex
    {
      get { return _panel; }
      set { _panel = value; OnPropertyChanged("DisplayIndex"); }
    }

    private ObservableCollection<ProjectInfo> _projects;
    public ObservableCollection<ProjectInfo> Projects
    {
      get { return _projects; }
      private set 
      { 
        _projects = value; 
        OnPropertyChanged("Projects");
      }
    }

    private ProjectInfo _selectedProject;
    public ProjectInfo SelectedProject
    {
      get { return _selectedProject; }
      set
      {
        _selectedProject = value;
        OnPropertyChanged("SelectedProject");
        CreateResourceAssignment();
      }
    }

    public ProjectTracker.Library.RoleList RoleList
    {
      get { return ProjectTracker.Library.RoleList.GetList(); }
    }

    public new void Save()
    {
      if (Model != null)
      {
        Model.ApplyEdit();
        if (!EditMode)
          ParentResource.Assignments.Add(Model);
        Model = null;
      }
      Bxf.Shell.Instance.ShowView(null, null);
    }

    public void Remove()
    {
      if (Model != null)
      {
        Model.ApplyEdit();
        if (EditMode)
          ParentResource.Assignments.Remove(Model);
        Model = null;
        Bxf.Shell.Instance.ShowView(null, null);
      }
    }

    public new void Cancel()
    {
      if (Model != null)
      {
        Model.CancelEdit();
        Model = null;
      }
      Bxf.Shell.Instance.ShowView(null, null);
    }

    public override void NavigatedAway()
    {
      if (Model != null)
      {
        Model.CancelEdit();
        Model = null;
      }
    }

    public class ProjectInfo
    {
      public ProjectInfo(ProjectTracker.Library.ProjectInfo item, ResourceAssignmentEdit parent)
      {
        Id = item.Id;
        ProjectName = item.Name;
        Parent = parent;
      }

      public int Id { get; private set; }
      public ResourceAssignmentEdit Parent { get; private set; }
      public string ProjectName { get; set; }
    }
  }
}
