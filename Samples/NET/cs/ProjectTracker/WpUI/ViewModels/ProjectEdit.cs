using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ProjectEdit : ViewModel<ProjectTracker.Library.ProjectGetter>
  {
    public ProjectEdit(string queryString)
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

    public override void NavigatingBackTo()
    {
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

    public bool CanSaveProject
    {
      get 
      {
        if (Model != null)
          return Model.Project.IsSavable;
        else
          return false;
      }
    }

    public void Save()
    {
      if (!Model.Project.IsDirty)
      {
        Bxf.Shell.Instance.ShowView(null, null);
      }
      else if (CanSaveProject)
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Saving..." });
        App.ViewModel.MainPageViewModel.ProjectsChanged = true;
        Model.Project.BeginSave((o, e) =>
          {
            Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
            if (e.Error != null)
              Bxf.Shell.Instance.ShowError(e.Error.Message, "Data error");
            else
              Bxf.Shell.Instance.ShowView(null, null);
          });
      }
      else
      {
        if (Model.Project.IsValid)
          Bxf.Shell.Instance.ShowError("Not authorized", "Save");
        else
          Bxf.Shell.Instance.ShowError("Invalid data", "Save");
      }
    }

    public void Close()
    {
      Bxf.Shell.Instance.ShowView(null, null);
    }

    public void AddNewResource()
    {
      Bxf.Shell.Instance.ShowView("/ProjectResourceEdit.xaml", null,
        new ProjectResourceEdit(Model.Project), null);
    }

    /// <summary>
    /// Child viewmodel
    /// </summary>
    public class ResourceInfo : ViewModelLocal<ProjectTracker.Library.ProjectResourceEdit>
    {
      public ResourceInfo(ProjectTracker.Library.ProjectResourceEdit model)
      {
        Model = model;
      }

      public string FullName
      {
        get { return Model.FirstName + " " + Model.LastName; }
      }

      public string RoleName
      {
        get { return Model.RoleName; }
      }

      public void EditResource()
      {
        Bxf.Shell.Instance.ShowView("/ProjectResourceEdit.xaml", null, 
          new ProjectResourceEdit(Model), null);
      }
    }
  }
}
