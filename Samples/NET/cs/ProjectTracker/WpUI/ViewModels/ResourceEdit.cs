namespace WpUI.ViewModels
{
  public class ResourceEdit : ViewModel<ProjectTracker.Library.ResourceGetter>
  {
    public ResourceEdit(string queryString)
    {
      if (string.IsNullOrEmpty(queryString))
      {
        BeginRefresh(callback => ProjectTracker.Library.ResourceGetter.CreateNewResource(callback));
      }
      else
      {
        var p = queryString.Split('=');
        var resourceId = int.Parse(p[1]);
        ManageObjectLifetime = false;
        BeginRefresh(callback => ProjectTracker.Library.ResourceGetter.GetExistingResource(resourceId, callback));
      }
    }

    public bool CanSaveResource
    {
      get
      {
        if (Model != null)
          return Model.Resource.IsSavable;
        else
          return false;
      }
    }

    internal void Save()
    {
      if (CanSaveResource)
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Saving..." });
        Model.Resource.BeginSave((o, e) =>
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
        if (Model.Resource.IsValid)
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
