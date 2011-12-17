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
        App.ViewModel.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Saving..." });
        Model.Resource.BeginSave((o, e) =>
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
        if (Model.Resource.IsValid)
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
