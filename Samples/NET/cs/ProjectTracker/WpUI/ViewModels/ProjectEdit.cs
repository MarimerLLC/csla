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
      if (CanSaveProject)
      {
        App.ViewModel.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Saving..." });
        App.ViewModel.MainPageViewModel.ProjectsChanged = true;
        Model.Project.BeginSave((o, e) =>
          {
            App.ViewModel.ShowStatus(new Bxf.Status());
            if (e.Error != null)
              App.ViewModel.ShowError(e.Error.Message, "Data error");
            else
              App.ViewModel.Back();
          });
      }
      else
      {
        if (Model.Project.IsValid)
          App.ViewModel.ShowError("Not authorized", "Save");
        else
          App.ViewModel.ShowError("Invalid data", "Save");
      }
    }

    public void Close()
    {
      App.ViewModel.Back();
    }

    public void AddNewResource()
    {
      App.ViewModel.ShowView("/ProjectResourceEdit.xaml?mode=new");
    }


    public class ResourceInfo : ViewModelLocal<ProjectTracker.Library.ProjectResourceEdit>
    {
      public ResourceInfo(ProjectTracker.Library.ProjectResourceEdit model)
      {
        ManageObjectLifetime = false;
        Model = model;
      }

      public string FullName
      {
        get { return Model.FirstName + " " + Model.LastName; }
      }

      public void EditResource()
      {
        App.ViewModel.ShowView("/ProjectResourceEdit.xaml?id=" + Model.ResourceId);
      }
    }
  }
}
