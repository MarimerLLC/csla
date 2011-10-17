using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ProjectDetail : ViewModel<ProjectTracker.Library.ProjectGetter>
  {
    public ProjectDetail(string queryString)
    {
      if (string.IsNullOrEmpty(queryString))
      {
        BeginRefresh(callback => ProjectTracker.Library.ProjectGetter.CreateNewProject(callback));
      }
      else
      { 
        var p = queryString.Split('=');
        var projectId = int.Parse(p[1]);
        ManageObjectLifetime = false;
        BeginRefresh(callback => ProjectTracker.Library.ProjectGetter.GetExistingProject(projectId, callback));
      }
    }

    protected override void OnModelChanged(ProjectTracker.Library.ProjectGetter oldValue, ProjectTracker.Library.ProjectGetter newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged("Resources");
    }

    public ObservableCollection<ResourceInfo> Resources
    {
      get 
      {
        var result = new ObservableCollection<ResourceInfo>();
        if (Model != null)
          foreach (var item in Model.Project.Resources)
            result.Add(new ResourceInfo(item));
        return result;
      }
    }

    internal void Delete()
    {
      Model.Project.Delete();
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting project" });
      Model.Project.BeginSave((o, e) =>
        {
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
          if (e.Error != null)
            Bxf.Shell.Instance.ShowError(e.Error.Message, "Project delete");
          else
            Bxf.Shell.Instance.ShowView(null, "Dialog");
        });
    }

    internal void Edit()
    {
      Bxf.Shell.Instance.ShowView(
        "/ProjectEdit.xaml?id=" + Model.Project.Id, null, null, "Dialog");
    }

    internal void Close()
    {
      Bxf.Shell.Instance.ShowView(null, "Dialog");
    }

    public class ResourceInfo : ViewModelLocal<ProjectTracker.Library.ProjectResourceEdit>
    {
      public ResourceInfo(ProjectTracker.Library.ProjectResourceEdit model)
      {
        ManageObjectLifetime = false;
        Model = model;
      }

      public void ShowResource()
      {
        Bxf.Shell.Instance.ShowView(
          "/ResourceDetails.xaml?id=" + Model.ResourceId, null, null, "Dialog");
      }
    }
  }
}
