using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using ProjectTracker.Library;
using System.Threading.Tasks;
using ProjectTracker.Ui.Xamarin;

namespace XamarinFormsUi.ViewModels
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
        newValue.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(ItemList));
      OnPropertyChanged(nameof(ItemList));
    }

    internal async void EditItem(ProjectTracker.Library.ProjectInfo item)
    {
      await App.NavigateTo(typeof(Views.ProjectEdit), item.Id);
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

    public void AddItem()
    {
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
      }

      public async void EditItem()
      {
        await App.NavigateTo(typeof(Views.ProjectEdit), 1);
      }

      public async void RemoveItem()
      {
        await ProjectTracker.Library.ProjectEdit.DeleteProjectAsync(Model.Id);
        Parent.Model.RemoveChild(Model.Id);
      }
    }
  }
}
