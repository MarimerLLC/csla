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
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Saving..." });
        Model.Project.BeginSave((o, e) =>
          {
            Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
            if (e.Error != null)
              Bxf.Shell.Instance.ShowError(e.Error.Message, "Data error");
            else
              Bxf.Shell.Instance.ShowView(null, "Dialog");
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

    internal void Close()
    {
      Bxf.Shell.Instance.ShowView(null, "Dialog");
    }
  }
}
