using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace SilverlightUI.ViewModels
{
  public class ProjectList : ViewModel<ProjectTracker.Library.ProjectList>
  {
    public ProjectList()
    {
      BeginRefresh(ProjectTracker.Library.ProjectList.GetProjectList);
    }

    protected override void OnModelChanged(ProjectTracker.Library.ProjectList oldValue, ProjectTracker.Library.ProjectList newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged("ItemList");
    }

    public ObservableCollection<ProjectInfo> ItemList
    {
      get
      {
        if (Model == null)
          return null;
        else
          return new ObservableCollection<ProjectInfo>(
            Model.Select(r => new ProjectInfo(r)));
      }
    }

    public bool CanAdd
    {
      get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectTracker.Library.ProjectEdit)); }
    }

    public void AddItem()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(Views.ProjectEdit).AssemblyQualifiedName,
        "projectGetterViewSource",
        new ProjectGetter(),
        "Main");
    }

    public class ProjectInfo : ViewModelLocal<ProjectTracker.Library.ProjectInfo>
    {
      public ProjectInfo(ProjectTracker.Library.ProjectInfo info)
      {
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

      public void RemoveItem()
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Getting item to delete..." });
        ProjectTracker.Library.ProjectEdit.GetProject(Model.Id, (o, e) =>
          {
            if (e.Error != null)
            {
              Bxf.Shell.Instance.ShowError(e.Error.Message, "Failed to delete item");
              Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsOk = false, Text = "Item NOT deleted" });
            }
            else
            {
              e.Object.Delete();
              Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting item..." });
              e.Object.BeginSave((s, a) =>
                {
                  if (a.Error != null)
                  {
                    Bxf.Shell.Instance.ShowError(a.Error.Message, "Failed to delete item");
                    Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsOk = false, Text = "Item NOT deleted" });
                  }
                  else
                  {
                    Bxf.Shell.Instance.ShowStatus(new Bxf.Status { Text = "Item deleted" });
                    Bxf.Shell.Instance.ShowView(
                      typeof(Views.ProjectList).AssemblyQualifiedName,
                      "projectListViewSource",
                      new ProjectList(),
                      "Main");
                  }
                });
            }
          });
      }
    }
  }
}
