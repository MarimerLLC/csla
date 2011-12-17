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

    internal void Save()
    {
      if (CanSaveProject)
      {
        App.ViewModel.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Saving..." });
        Model.Project.BeginSave((o, e) =>
          {
            App.ViewModel.ShowStatus(new Bxf.Status());
            if (e.Error != null)
              App.ViewModel.ShowError(e.Error.Message, "Data error");
            else
              App.ViewModel.ShowView(null);
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

    internal void Close()
    {
      App.ViewModel.ShowView(null);
    }
  }
}
