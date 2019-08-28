using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfUI.ViewModels
{
  /// <summary>
  /// Manages the ProjectEdit operation
  /// </summary>
  public class ProjectGetter : ViewModel<ProjectTracker.Library.ProjectEdit>
  {
    public ProjectGetter()
    {
      var task = RefreshAsync<ProjectTracker.Library.ProjectEdit>(async () =>
        await ProjectTracker.Library.ProjectGetter.CreateNewProject());
    }
    
    public ProjectGetter(ProjectTracker.Library.ProjectInfo info)
    {
      var task = RefreshAsync<ProjectTracker.Library.ProjectEdit>(async () =>
        await ProjectTracker.Library.ProjectGetter.GetExistingProject(info.Id));
    }

    protected override void OnModelChanged(ProjectTracker.Library.ProjectEdit oldValue, ProjectTracker.Library.ProjectEdit newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged(nameof(ProjectEditViewModel));
    }

    public List<ProjectEdit> ProjectEditViewModel
    {
      get
      {
        if (Model == null)
          return null;
        else
          return new List<ProjectEdit> { new ProjectEdit(this, Model) };
      }
    }

    public ProjectTracker.Library.RoleList RoleList
    {
      get => ProjectTracker.Library.RoleList.GetCachedList();
    }

    /// <summary>
    /// Manages adding or editing of a ProjectEdit object
    /// </summary>
    public class ProjectEdit : ViewModelLocalEdit<ProjectTracker.Library.ProjectEdit>
    {
      public ProjectEdit(ProjectGetter parent, ProjectTracker.Library.ProjectEdit project)
      {
        Parent = parent;
        Model = project;
      }

      public ProjectGetter Parent { get; private set; }

      private UserControl _childEditContent;
      public UserControl ChildEditContent
      {
        get { return _childEditContent; }
        set { _childEditContent = value; OnPropertyChanged(nameof(ChildEditContent)); }
      }

      protected override void OnModelChanged(ProjectTracker.Library.ProjectEdit oldValue, ProjectTracker.Library.ProjectEdit newValue)
      {
        base.OnModelChanged(oldValue, newValue);
        Model.Resources.CollectionChanged += (o, e) => OnPropertyChanged(nameof(ProjectResourceList));
        OnPropertyChanged(nameof(ProjectResourceList));
      }

      public ObservableCollection<ProjectResourceDisplay> ProjectResourceList
      {
        get
        {
          return new ObservableCollection<ProjectResourceDisplay>(
            Model.Resources.Select(r => new ProjectResourceDisplay(this, r)));
        }
      }

      public void AddResource()
      {
        ShowResourceEdit(new ViewModels.ProjectResourceEdit(this));
      }

      public void CommitAddResource(ProjectTracker.Library.ProjectResourceEdit projectResource)
      {
        ChildEditContent = null;
        Model.Resources.Add(projectResource);
      }

      public void EditResource(ProjectTracker.Library.ProjectResourceEdit projectResource)
      {
        ShowResourceEdit(new ProjectResourceEdit(this, projectResource));
      }

      public void CommitEditResource(ProjectTracker.Library.ProjectResourceEdit projectResource)
      {
        ChildEditContent = null;
      }

      private void ShowResourceEdit(ProjectResourceEdit viewmodel)
      {
        var ctl = new Views.ProjectResourceEdit();
        var cvs = (CollectionViewSource)ctl.Resources["projectResourceEditViewSource"];
        cvs.Source = new List<object> { viewmodel };
        ChildEditContent = ctl;
      }

      public void CancelAddResource()
      {
        ChildEditContent = null;
      }

      /// <summary>
      /// Manages display of a ProjectResourceEdit object
      /// </summary>
      public class ProjectResourceDisplay : ViewModelLocal<ProjectTracker.Library.ProjectResourceEdit>
      {
        public ProjectResourceDisplay(ProjectEdit parent, ProjectTracker.Library.ProjectResourceEdit projectResource)
        {
          Parent = parent;
          ManageObjectLifetime = false;
          Model = projectResource;
        }

        public ProjectEdit Parent { get; private set; }

        public void EditItem()
        {
          Parent.EditResource(this.Model);
        }

        public void RemoveItem()
        {
          Model.Parent.RemoveChild(Model);
        }
      }
    }
  }
}
