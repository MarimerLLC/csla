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

    public override void NavigatingBackTo()
    {
      OnPropertyChanged("Assignments");
    }

    public ObservableCollection<AssignmentInfo> Assignments
    {
      get
      {
        var result = new ObservableCollection<AssignmentInfo>();
        if (Model != null)
          foreach (var item in Model.Resource.Assignments)
            result.Add(new AssignmentInfo(Model.Resource, item));
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

    public void Save()
    {
      if (!Model.Resource.IsDirty)
      {
        Bxf.Shell.Instance.ShowView(null, null);
      }
      else if (CanSaveResource)
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

    public void Close()
    {
      Bxf.Shell.Instance.ShowView(null, null);
    }

    public void AddNewAssignment()
    {
      Bxf.Shell.Instance.ShowView("/ResourceAssignmentEdit.xaml", null,
        new ResourceAssignmentEdit(Model.Resource), null);
    }

    /// <summary>
    /// Child viewmodel
    /// </summary>
    public class AssignmentInfo : ViewModelLocal<ProjectTracker.Library.ResourceAssignmentEdit>
    {
      public ProjectTracker.Library.ResourceEdit ParentResource { get; private set; }

      public AssignmentInfo(ProjectTracker.Library.ResourceEdit parent, ProjectTracker.Library.ResourceAssignmentEdit model)
      {
        ParentResource = parent;
        Model = model;
      }

      public string ProjectName
      {
        get { return Model.ProjectName; }
      }

      public string RoleName
      {
        get { return Model.RoleName; }
      }

      public void EditAssignment()
      {
        Bxf.Shell.Instance.ShowView("/ResourceAssignmentEdit.xaml", null, 
          new ResourceAssignmentEdit(ParentResource, Model), null);
      }
    }
  }
}
