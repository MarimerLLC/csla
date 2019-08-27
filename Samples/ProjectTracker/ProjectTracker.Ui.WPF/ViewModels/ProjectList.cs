using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace WpfUI.ViewModels
{
  public class ProjectList : ViewModel<ProjectTracker.Library.ProjectList>
  {
    public ProjectList()
    {
      var task = RefreshAsync<ProjectTracker.Library.ProjectList>(async () =>
        await ProjectTracker.Library.ProjectList.GetProjectListAsync());
    }

    protected override void OnModelChanged(ProjectTracker.Library.ProjectList oldValue, ProjectTracker.Library.ProjectList newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      if (newValue != null)
        newValue.CollectionChanged += (sender, args) => OnPropertyChanged("ItemList");
      OnPropertyChanged(nameof(ItemList));
    }

    public ObservableCollection<ProjectInfo> ItemList
    {
      get
      {
        if (Model == null)
          return null;
        else
          return new ObservableCollection<ProjectInfo>(
            Model.Select(r => new ProjectInfo(r, this)));
      }
    }

    public bool CanAdd
    {
      get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectTracker.Library.ProjectEdit)); }
    }

    public static void AddItem()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(Views.ProjectEdit).AssemblyQualifiedName,
        "projectGetterViewSource",
        new ProjectGetter(),
        "Main");
    }

    public class ProjectInfo : ViewModelLocal<ProjectTracker.Library.ProjectInfo>
    {
      public ProjectList Parent { get; private set; }

      public ProjectInfo(ProjectTracker.Library.ProjectInfo info, ProjectList parent)
      {
        Parent = parent;
        Model = info;
      }

      public bool CanEdit
      {
        get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectTracker.Library.ProjectEdit)); }
      }

      public new bool CanRemove
      {
        get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ProjectTracker.Library.ProjectEdit)); }
      }

      public void DisplayItem()
      {
        Bxf.Shell.Instance.ShowView(
          typeof(Views.ProjectDisplay).AssemblyQualifiedName,
          "projectDisplayViewSource",
          new ProjectDisplay(Model.Id),
          "Main");
      }

      public void EditItem()
      {
        Bxf.Shell.Instance.ShowView(
          typeof(Views.ProjectEdit).AssemblyQualifiedName,
          "projectGetterViewSource",
          new ProjectGetter(Model),
          "Main");
      }

      public async void RemoveItem()
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting item..." });
        try
        {
          await ProjectTracker.Library.ProjectEdit.DeleteProjectAsync(Model.Id);
          Parent.Model.RemoveChild(Model.Id);
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status { Text = "Item deleted" });
        }
        catch (Exception ex)
        {
          Bxf.Shell.Instance.ShowError(ex.Message, "Failed to delete item");
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsOk = false, Text = "Item NOT deleted" });
        }
      }
    }
  }
}
