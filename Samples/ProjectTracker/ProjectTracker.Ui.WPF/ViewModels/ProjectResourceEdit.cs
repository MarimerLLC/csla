using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WpfUI.ViewModels
{
  public class ProjectResourceEdit : ViewModelLocal<ProjectTracker.Library.ProjectResourceEdit>
  {
    public ProjectResourceEdit(ProjectGetter.ProjectEdit parent)
    {
      Parent = parent;
      ResourceList = new ResourceList();
    }

    public ProjectResourceEdit(ProjectGetter.ProjectEdit parent, ProjectTracker.Library.ProjectResourceEdit projectResource)
    {
      Parent = parent;
      EditMode = true;
      Model = projectResource;
    }

    public ProjectGetter.ProjectEdit Parent { get; private set; }
    public bool EditMode { get; private set; }

    private bool _showResourceList;
    public bool ShowResourceList
    {
      get { return _showResourceList; }
      set { _showResourceList = value; OnPropertyChanged("ShowResourceList"); }
    }

    private ResourceList _resourceList;
    public ResourceList ResourceList
    {
      get { return _resourceList; }
      set 
      { 
        _resourceList = value; 
        OnPropertyChanged("ResourceList");
        ShowResourceList = (ResourceList != null);
      }
    }

    public ProjectTracker.Library.RoleList RoleList
    {
      get { return ProjectTracker.Library.RoleList.GetCachedList(); }
    }

    private ProjectTracker.Library.ResourceInfo _selectedResource;
    public ProjectTracker.Library.ResourceInfo SelectedResource
    {
      get { return _selectedResource; }
      set 
      { 
        _selectedResource = value; 
        OnPropertyChanged(nameof(SelectedResource));
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        CreateProjectResource();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      }
    }

    public async Task CreateProjectResource()
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Creating new resource..."});
      var result = await ProjectTracker.Library.ProjectResourceEditCreator.GetProjectResourceEditCreatorAsync(SelectedResource.Id);
      Model = result.Result;
    }

    public void Save()
    {
      if (Model != null)
        Model.ApplyEdit();
      if (EditMode)
        Parent.CommitEditResource(Model);
      else
        Parent.CommitAddResource(Model);
    }

    public void Cancel()
    {
      if (Model != null)
        Model.CancelEdit();
      Parent.CancelAddResource();
    }
  }
}
