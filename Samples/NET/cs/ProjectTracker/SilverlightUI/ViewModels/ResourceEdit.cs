﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace SilverlightUI.ViewModels
{
  /// <summary>
  /// Manages the ProjectEdit operation
  /// </summary>
  public class ResourceGetter : ViewModel<ProjectTracker.Library.ResourceGetter>
  {
    public ResourceGetter()
    {
      BeginRefresh(callback => ProjectTracker.Library.ResourceGetter.CreateNewResource(callback));
    }
    
    public ResourceGetter(ProjectTracker.Library.ResourceInfo info)
    {
      BeginRefresh(callback => ProjectTracker.Library.ResourceGetter.GetExistingResource(info.Id, callback));
    }

    protected override void OnModelChanged(ProjectTracker.Library.ResourceGetter oldValue, ProjectTracker.Library.ResourceGetter newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged("RoleList");
      OnPropertyChanged("ResourceEditViewModel");
    }

    public List<ResourceEdit> ResourceEditViewModel
    {
      get
      {
        if (Model == null)
          return null;
        else
          return new List<ResourceEdit> { new ResourceEdit(this, Model.Resource) };
      }
    }

    public ProjectTracker.Library.RoleList RoleList
    {
      get
      {
        if (Model == null)
          return null;
        else
          return Model.RoleList;
      }
    }

    /// <summary>
    /// Manages adding or editing of a ResourceEdit object
    /// </summary>
    public class ResourceEdit : ViewModelLocalEdit<ProjectTracker.Library.ResourceEdit>
    {
      public ResourceEdit(ResourceGetter parent, ProjectTracker.Library.ResourceEdit resource)
      {
        Parent = parent;
        Model = resource;
      }

      public ResourceGetter Parent { get; private set; }

      private UserControl _childEditContent;
      public UserControl ChildEditContent
      {
        get { return _childEditContent; }
        set { _childEditContent = value; OnPropertyChanged("ChildEditContent"); }
      }

      protected override void OnModelChanged(ProjectTracker.Library.ResourceEdit oldValue, ProjectTracker.Library.ResourceEdit newValue)
      {
        base.OnModelChanged(oldValue, newValue);
        Model.Assignments.CollectionChanged += (o, e) => OnPropertyChanged("AssignmentList");
        OnPropertyChanged("AssignmentList");
      }

      public ObservableCollection<AssignmentDisplay> AssignmentList
      {
        get
        {
          return new ObservableCollection<AssignmentDisplay>(
            Model.Assignments.Select(r => new AssignmentDisplay(this, r)));
        }
      }

      public void AddAssignment()
      {
        ShowAssignmentEdit(new ViewModels.ResourceAssignmentEdit(this));
      }

      public void CommitAddAssignment(ProjectTracker.Library.ResourceAssignmentEdit assignment)
      {
        ChildEditContent = null;
        Model.Assignments.Add(assignment);
      }

      public void EditAssignment(ProjectTracker.Library.ResourceAssignmentEdit assignment)
      {
        ShowAssignmentEdit(new ResourceAssignmentEdit(this, assignment));
      }

      public void CommitEditAssignment(ProjectTracker.Library.ResourceAssignmentEdit ResourceResource)
      {
        ChildEditContent = null;
      }

      private void ShowAssignmentEdit(ResourceAssignmentEdit viewmodel)
      {
        var ctl = new Views.ResourceAssignmentEdit();
        var cvs = (CollectionViewSource)ctl.Resources["resourceAssignmentEditViewSource"];
        cvs.Source = new List<object> { viewmodel };
        ChildEditContent = ctl;
      }

      public void CancelAddAssignment()
      {
        ChildEditContent = null;
      }

      /// <summary>
      /// Manages display of a ResourceResourceEdit object
      /// </summary>
      public class AssignmentDisplay : ViewModelLocal<ProjectTracker.Library.ResourceAssignmentEdit>
      {
        public AssignmentDisplay(ResourceEdit parent, ProjectTracker.Library.ResourceAssignmentEdit assignment)
        {
          Parent = parent;
          ManageObjectLifetime = false;
          Model = assignment;
        }

        public ResourceEdit Parent { get; private set; }

        public void EditItem()
        {
          Parent.EditAssignment(this.Model);
        }

        public void RemoveItem()
        {
          Model.Parent.RemoveChild(Model);
        }
      }
    }
  }
}
