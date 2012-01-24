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

    public new bool CanDelete
    {
      get
      {
        return Csla.Rules.BusinessRules.HasPermission(
          Csla.Rules.AuthorizationActions.DeleteObject, Model.Resource);
      }
    }

    public void Delete()
    {
      if (CanDelete)
      {
        var dialog = new Confirm { Title = "Resource", Prompt = "Delete resource?" };
        Bxf.Shell.Instance.ShowView(null, null, dialog, "confirm");
        if (dialog.Result)
        {
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting resource" });
          Model.Resource.Delete();
          Model.Resource.BeginSave((o, e) =>
            {
              App.ViewModel.MainPageViewModel.ResourcesChanged = true;
              Bxf.Shell.Instance.ShowStatus(new Bxf.Status());
              if (e.Error != null)
                Bxf.Shell.Instance.ShowError(e.Error.Message, "Resource delete");
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

    public bool CanEdit
    {
      get
      {
        return Csla.Rules.BusinessRules.HasPermission(
          Csla.Rules.AuthorizationActions.EditObject, Model.Resource);
      }
    }

    internal void Edit()
    {
      if (CanEdit)
        Bxf.Shell.Instance.ShowView("/ResourceEdit.xaml?id=" + Model.Resource.Id, null, null, null);
      else
        Bxf.Shell.Instance.ShowError("Not authorized to edit resource", "Authorization");
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
