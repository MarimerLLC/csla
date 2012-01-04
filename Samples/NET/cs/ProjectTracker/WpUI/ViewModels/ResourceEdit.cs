using System.Collections.ObjectModel;
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

    protected override void OnModelChanged(ProjectTracker.Library.ResourceGetter oldValue, ProjectTracker.Library.ResourceGetter newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged("Assignments");
    }

    public override void NavigatingTo()
    {
      if (App.ViewModel.MainPageViewModel.ProjectsChanged || Model != null && Model.Resource != null && Model.Resource.IsDirty)
        OnPropertyChanged("Assignments");
    }

    public ObservableCollection<AssignmentInfo> Assignments
    {
      get
      {
        var result = new ObservableCollection<AssignmentInfo>();
        if (Model != null)
          foreach (var item in Model.Resource.Assignments)
            result.Add(new AssignmentInfo(this, item));
        return result;
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
        App.ViewModel.MainPageViewModel.ResourcesChanged = true;
        Model.Resource.BeginSave((o, e) =>
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
        if (Model.Resource.IsValid)
          Bxf.Shell.Instance.ShowError("Not authorized", "Save");
        else
          Bxf.Shell.Instance.ShowError("Invalid data", "Save");
      }
    }

    internal void Close()
    {
      Bxf.Shell.Instance.ShowView(null, null);
    }

    internal void CommitEditAssignment(ProjectTracker.Library.ResourceAssignmentEdit item)
    {
      Bxf.Shell.Instance.ShowView(null, null);
    }

    internal void CommitAddAssignment(ProjectTracker.Library.ResourceAssignmentEdit item)
    {
      Model.Resource.Assignments.Add(item);
      Model.Resource.Assignments.ApplyEdit();
      Bxf.Shell.Instance.ShowView(null, null);
    }

    internal void CancelAddEditAssignment()
    {
      throw new System.NotImplementedException();
    }

    /// <summary>
    /// Child viewmodel
    /// </summary>
    public class AssignmentInfo : ViewModelLocal<ProjectTracker.Library.ResourceAssignmentEdit>
    {
      public ResourceEdit Parent { get; private set; }

      public AssignmentInfo(ResourceEdit parent, ProjectTracker.Library.ResourceAssignmentEdit model)
      {
        Parent = parent;
        Model = model;
      }

      public void EditAssignment()
      {
        Bxf.Shell.Instance.ShowView("/ResourceAssignmentEdit.xaml", null, 
          new ResourceAssignmentEdit(Parent, Model), null);
      }
    }
  }
}
