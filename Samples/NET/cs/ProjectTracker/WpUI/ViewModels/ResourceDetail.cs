using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ResourceDetail : ViewModel<ProjectTracker.Library.ResourceGetter>
  {
    public ResourceDetail(string queryString)
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

    protected override void OnModelChanged(ProjectTracker.Library.ResourceGetter oldValue, ProjectTracker.Library.ResourceGetter newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged("Resources");
    }

    public ObservableCollection<AssignmentInfo> Assignments
    {
      get
      {
        var result = new ObservableCollection<AssignmentInfo>();
        if (Model != null)
          foreach (var item in Model.Resource.Assignments)
            result.Add(new AssignmentInfo(item));
        return result;
      }
    }

    internal void Delete()
    {
      Model.Resource.Delete();
      Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting resource" });
      Model.Resource.BeginSave((o, e) =>
        {
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
          if (e.Error != null)
            Bxf.Shell.Instance.ShowError(e.Error.Message, "Resource delete");
          else
            Bxf.Shell.Instance.ShowView(null, "Dialog");
        });
    }

    internal void Edit()
    {
      Bxf.Shell.Instance.ShowView(
        "/ResourceEdit.xaml?id=" + Model.Resource.Id, null, null, "Dialog");
    }

    internal void Close()
    {
      Bxf.Shell.Instance.ShowView(null, "Dialog");
    }

    public class AssignmentInfo : ViewModelLocal<ProjectTracker.Library.ResourceAssignmentEdit>
    {
      public AssignmentInfo(ProjectTracker.Library.ResourceAssignmentEdit model)
      {
        ManageObjectLifetime = false;
        Model = model;
      }

      public void ShowProject()
      {
        Bxf.Shell.Instance.ShowView(
          "/ProjectDetails.xaml?id=" + Model.ProjectId, null, null, "Dialog");
      }
    }
  }
}
