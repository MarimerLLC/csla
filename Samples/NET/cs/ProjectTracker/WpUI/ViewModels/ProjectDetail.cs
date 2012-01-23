using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ProjectDetail : ViewModel<ProjectTracker.Library.ProjectGetter>
  {
    private int _projectId;

    public ProjectDetail(string queryString)
    {
      ManageObjectLifetime = false;

      var p = queryString.Split('=');
      _projectId = int.Parse(p[1]);

      BeginRefresh(callback => ProjectTracker.Library.ProjectGetter.GetExistingProject(_projectId, callback));
    }

    protected override void OnModelChanged(ProjectTracker.Library.ProjectGetter oldValue, ProjectTracker.Library.ProjectGetter newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged("Resources");
    }

    public override void NavigatingBackTo()
    {
      if (App.ViewModel.MainPageViewModel.ProjectsChanged)
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Reloading..." });
        BeginRefresh(callback => ProjectTracker.Library.ProjectGetter.GetExistingProject(_projectId, callback));
      }
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

    public bool CanEdit
    {
      get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectTracker.Library.ProjectEdit)); }
    }

    public void Edit()
    {
      if (CanEdit)
        Bxf.Shell.Instance.ShowView("/ProjectEdit.xaml?id=" + Model.Project.Id, null, null, null);
      else
        Bxf.Shell.Instance.ShowError("Not authorized to edit projects", "Authorization");
    }

    public new bool CanDelete
    {
      get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ProjectTracker.Library.ProjectEdit)); }
    }

    public void Delete()
    {
      if (CanDelete)
      {
        var dialog = new Confirm { Title = "Project", Prompt = "Delete project?" };
        Bxf.Shell.Instance.ShowView(null, null, dialog, "confirm");
        if (dialog.Result)
        {
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting project" });
          Model.Project.Delete();
          Model.Project.BeginSave((o, e) =>
            {
              App.ViewModel.MainPageViewModel.ProjectsChanged = true;
              Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
              if (e.Error != null)
                Bxf.Shell.Instance.ShowError(e.Error.Message, "Project delete");
              else
                Bxf.Shell.Instance.ShowView(null, null);
            });
        }
      }
      else
      {
        Bxf.Shell.Instance.ShowError("Not authorized to delete projects", "Authorization");
      }
    }

    public void Close()
    {
      Bxf.Shell.Instance.ShowView(null, null);
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
        Bxf.Shell.Instance.ShowView("/ResourceDetails.xaml?id=" + Model.ResourceId, null, null, null);
      }
    }
  }
}
