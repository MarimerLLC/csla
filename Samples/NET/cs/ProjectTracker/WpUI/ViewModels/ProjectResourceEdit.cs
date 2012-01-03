using System;
using System.Linq;
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
  public class ProjectResourceEdit : ViewModelLocalEdit<ProjectTracker.Library.ProjectResourceEdit>
  {
    public ProjectEdit Parent { get; private set; }
    public bool EditMode { get; private set; }

    public ProjectResourceEdit(ProjectEdit parent)
    {
      ManageObjectLifetime = true;
      EditMode = false;
      Parent = parent;
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

    public ProjectResourceEdit(ProjectEdit parent, ProjectTracker.Library.ProjectResourceEdit item)
    {
      ManageObjectLifetime = true;
      EditMode = true;
      Parent = parent;
      Model = item;
    }

    private bool _showResourceList;
    public bool ShowResourceList
    {
      get { return _showResourceList; }
      set { _showResourceList = value; OnPropertyChanged("ShowResourceList"); }
    }

    public void CreateProjectResource()
    {
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Creating new resource..." });
      ProjectTracker.Library.ProjectResourceEditCreator.GetProjectResourceEditCreator(SelectedResource.Id, (o, e) =>
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
        if (e.Error != null)
          Bxf.Shell.Instance.ShowError(e.Error.Message, "Data error");
        else
          Model = e.Object.Result;
      });
    }

    private ObservableCollection<ResourceInfo> _resources;
    public ObservableCollection<ResourceInfo> Resources
    {
      get { return _resources; }
      private set 
      { 
        _resources = value; 
        OnPropertyChanged("Resources");
        ShowResourceList = (Resources != null);
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

    public void Save()
    {
      if (Model != null)
        Model.ApplyEdit();
      if (!EditMode)
        Parent.CommitEditResource(Model);
      else
        Parent.CommitAddResource(Model);
      Model = null;
    }

    public void Cancel()
    {
      if (Model != null)
        Model.CancelEdit();
      Parent.CancelAddEditResource();
      Model = null;
    }

    public override void NavigatedAway()
    {
      Cancel();
    }

    public class ResourceInfo
    {
      public ResourceInfo(ProjectTracker.Library.ResourceInfo item, ProjectResourceEdit parent)
      {
        FullName = item.Name;
        Parent = parent;
      }

      public ProjectResourceEdit Parent { get; private set; }
      public string FullName { get; set; }

      public void SelectResource()
      {
        Bxf.Shell.Instance.ShowError("Hi", "");        
      }
    }
  }
}
