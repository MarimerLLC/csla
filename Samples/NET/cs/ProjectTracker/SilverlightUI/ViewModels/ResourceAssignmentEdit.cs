﻿using System;
using System.ComponentModel;

namespace SilverlightUI.ViewModels
{
  public class ResourceAssignmentEdit : ViewModelLocal<ProjectTracker.Library.ResourceAssignmentEdit>
  {
    public ResourceAssignmentEdit(ResourceGetter.ResourceEdit parent)
    {
      Parent = parent;
      ProjectList = new ProjectList();
    }

    public ResourceAssignmentEdit(ResourceGetter.ResourceEdit parent, ProjectTracker.Library.ResourceAssignmentEdit assignment)
    {
      Parent = parent;
      EditMode = true;
      Model = assignment;
    }

    public ResourceGetter.ResourceEdit Parent { get; private set; }
    public bool EditMode { get; private set; }

    private bool _showProjectList;
    public bool ShowProjectList
    {
      get { return _showProjectList; }
      set { _showProjectList = value; OnPropertyChanged("ShowProjectList"); }
    }

    private ProjectList _projectList;
    public ProjectList ProjectList
    {
      get { return _projectList; }
      set
      {
        _projectList = value;
        OnPropertyChanged("ProjectList");
        ShowProjectList = (ProjectList != null);
      }
    }

    public ProjectTracker.Library.RoleList RoleList
    {
      get { return ProjectTracker.Library.RoleList.GetList(); }
    }

    private ProjectTracker.Library.ProjectInfo _selectedProject;
    public ProjectTracker.Library.ProjectInfo SelectedProject
    {
      get { return _selectedProject; }
      set
      {
        _selectedProject = value;
        OnPropertyChanged("SelectedProject");
        CreateAssignment();
      }
    }

    public void CreateAssignment()
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

    public void Save()
    {
      if (Model != null)
        Model.ApplyEdit();
      if (EditMode)
        Parent.CommitEditAssignment(Model);
      else
        Parent.CommitAddAssignment(Model);
    }

    public void Cancel()
    {
      if (Model != null)
        Model.CancelEdit();
      Parent.CancelAddAssignment();
    }
  }
}

