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
        {
          var list = new ObservableCollection<ResourceInfo>();
          foreach (var item in e.Object)
            list.Add(new ResourceInfo(item, this));
          Resources = list;
        }
      });
    }

    public ProjectResourceEdit(ProjectTracker.Library.ProjectEdit parent, ProjectTracker.Library.ProjectResourceEdit item)
    {
      ManageObjectLifetime = true;
      EditMode = true;
      ParentProject = parent;
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

    private ObservableCollection<ResourceInfo> _resources;
    public ObservableCollection<ResourceInfo> Resources
    {
      get { return _resources; }
      private set 
      { 
        _resources = value; 
        OnPropertyChanged("Resources");
      }
    }

    private ResourceInfo _selectedResource;
    public ResourceInfo SelectedResource
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

    public new void Save()
    {
      if (Model != null)
      {
        Model.ApplyEdit();
        if (!EditMode)
          ParentProject.Resources.Add(Model);
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
          ParentProject.Resources.Remove(Model);
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

    public class ResourceInfo
    {
      public ResourceInfo(ProjectTracker.Library.ResourceInfo item, ProjectResourceEdit parent)
      {
        Id = item.Id;
        FullName = item.Name;
        Parent = parent;
      }

      public int Id { get; private set; }
      public ProjectResourceEdit Parent { get; private set; }
      public string FullName { get; set; }
    }
  }
}
