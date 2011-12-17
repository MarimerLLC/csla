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
      App.ViewModel.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting resource" });
      Model.Resource.BeginSave((o, e) =>
        {
          App.ViewModel.ShowStatus(new Bxf.Status());
          if (e.Error != null)
            App.ViewModel.ShowError(e.Error.Message, "Resource delete");
          else
            App.ViewModel.ShowView(null);
        });
    }

    internal void Edit()
    {
      App.ViewModel.ShowView("/ResourceEdit.xaml?id=" + Model.Resource.Id);
    }

    internal void Close()
    {
      App.ViewModel.ShowView(null);
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
        App.ViewModel.ShowView("/ProjectDetails.xaml?id=" + Model.ProjectId);
      }
    }
  }
}
