using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ProjectResourceEdit : ViewModelLocalEdit<ProjectTracker.Library.ProjectResourceEdit>
  {
    public ProjectTracker.Library.ProjectEdit ParentProject { get; private set; }
    public bool EditMode { get; private set; }

    public ProjectResourceEdit(ProjectTracker.Library.ProjectEdit parent)
    {
      ManageObjectLifetime = true;
      EditMode = false;
      ParentProject = parent;
      ProjectTracker.Library.ResourceList.GetResourceList((o, e) =>
      {
        if (e.Error != null)
          Bxf.Shell.Instance.ShowError("Error retrieving resource list", "Data Error");
        else
          Resources = e.Object;
      });
    }

    public ProjectResourceEdit(ProjectTracker.Library.ProjectResourceEdit item)
    {
      ManageObjectLifetime = true;
      EditMode = true;
      ParentProject = (ProjectTracker.Library.ProjectEdit)((ProjectTracker.Library.ProjectResources)item.Parent).Parent;
      Model = item;
    }

    public void CreateProjectResource()
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Creating new resource..." });
      DisplayIndex = 1;
      ProjectTracker.Library.ProjectResourceEditCreator.GetProjectResourceEditCreator(SelectedResource.Id, (o, e) =>
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

    private ProjectTracker.Library.ResourceList _resources;
    public ProjectTracker.Library.ResourceList Resources
    {
      get { return _resources; }
      private set
      {
        _resources = value;
        OnPropertyChanged("Resources");
      }
    }

    private ProjectTracker.Library.ResourceInfo _selectedResource;
    public ProjectTracker.Library.ResourceInfo SelectedResource
    {
      get { return _selectedResource; }
      set
      {
        _selectedResource = value;
        OnPropertyChanged("SelectedResource");
        CreateProjectResource();
      }
    }

    public ProjectTracker.Library.RoleList RoleList
    {
      get { return ProjectTracker.Library.RoleList.GetList(); }
    }

    public void Accept()
    {
      if (Model != null)
      {
        Model.ApplyEdit();
        if (!EditMode)
          ParentProject.Resources.Add(Model);
        Model = null;
      }
      Close();
    }

    public void Remove()
    {
      if (Model != null)
      {
        Model.ApplyEdit();
        if (EditMode)
          ParentProject.Resources.Remove(Model);
        Model = null;
      }
      Close();
    }

    public void Close()
    {
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
  }
}
