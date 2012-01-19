using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ResourceDetail : ViewModel<ProjectTracker.Library.ResourceGetter>
  {
    private int _resourceId;

    public ResourceDetail(string queryString)
    {
      ManageObjectLifetime = false;

      var p = queryString.Split('=');
      _resourceId = int.Parse(p[1]);

      BeginRefresh(callback => ProjectTracker.Library.ResourceGetter.GetExistingResource(_resourceId, callback));
    }

    protected override void OnModelChanged(ProjectTracker.Library.ResourceGetter oldValue, ProjectTracker.Library.ResourceGetter newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged("Assignments");
    }

    public override void NavigatingBackTo()
    {
      if (App.ViewModel.MainPageViewModel.ResourcesChanged)
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Reloading..." });
        BeginRefresh(callback => ProjectTracker.Library.ResourceGetter.GetExistingResource(_resourceId, callback));
      }
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
            Bxf.Shell.Instance.ShowView(null, null);
        });
    }

    internal void Edit()
    {
      Bxf.Shell.Instance.ShowView("/ResourceEdit.xaml?id=" + Model.Resource.Id, null, null, null);
    }

    internal void Close()
    {
      Bxf.Shell.Instance.ShowView(null, null);
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
        Bxf.Shell.Instance.ShowView("/ProjectDetails.xaml?id=" + Model.ProjectId, null, null, null);
      }
    }
  }
}
