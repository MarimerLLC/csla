using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ResourceAssignmentEdit : ViewModelLocalEdit<ProjectTracker.Library.ResourceAssignmentEdit>
  {
    public ResourceEdit Parent { get; private set; }
    public bool EditMode { get; private set; }

    public ResourceAssignmentEdit(ResourceEdit parent)
    {
      ManageObjectLifetime = true;
      EditMode = false;
      Parent = parent;
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

    public ResourceAssignmentEdit(ResourceEdit parent, ProjectTracker.Library.ResourceAssignmentEdit item)
    {
      ManageObjectLifetime = true;
      EditMode = true;
      Parent = parent;
      Model = item;
    }

    public void CreateResourceAssignment()
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Creating new assignment..." });
      ProjectTracker.Library.ResourceAssignmentEditCreator.GetResourceAssignmentEditCreator(SelectedProject.Id, (o, e) =>
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
        if (e.Error != null)
          Bxf.Shell.Instance.ShowError(e.Error.Message, "Data error");
        else
          Model = e.Object.Result;
      });
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

    private ProjectTracker.Library.ProjectInfo _selectedProject;
    public ProjectTracker.Library.ProjectInfo SelectedProject
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
        Model.ApplyEdit();
      if (EditMode)
        Parent.CommitEditAssignment(Model);
      else
        Parent.CommitAddAssignment(Model);
      Model = null;
    }

    public new void Cancel()
    {
      if (Model != null)
        Model.CancelEdit();
      Parent.CancelAddEditAssignment();
      Model = null;
    }

    public override void NavigatedAway()
    {
      Cancel();
    }

    public class ProjectInfo
    {
      public ProjectInfo(ProjectTracker.Library.ProjectInfo item, ResourceAssignmentEdit parent)
      {
        ProjectName = item.Name;
        Parent = parent;
      }

      public ResourceAssignmentEdit Parent { get; private set; }
      public string ProjectName { get; set; }

      public void SelectProject()
      {
        Bxf.Shell.Instance.ShowError("Hi", "");        
      }
    }
  }
}
