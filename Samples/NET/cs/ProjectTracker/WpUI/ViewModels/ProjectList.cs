using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WpUI.ViewModels
{
  public class ProjectList : ViewModel<ProjectTracker.Library.ProjectList>
  {
    public ProjectList()
    {
      BeginRefresh(ProjectTracker.Library.ProjectList.GetProjectList);
    }

    protected override void OnModelChanged(
      ProjectTracker.Library.ProjectList oldValue, ProjectTracker.Library.ProjectList newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      if (newValue != null)
        newValue.CollectionChanged += (sender, args) => OnPropertyChanged("ItemList");
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
      if (CanAdd)
        Bxf.Shell.Instance.ShowView("/ProjectEdit.xaml", null, null, null);
    }

    public class ProjectInfo : ViewModelLocal<ProjectTracker.Library.ProjectInfo>
    {
      public ProjectInfo(ProjectTracker.Library.ProjectInfo info)
      {
        Model = info;
      }

      public void DisplayItem()
      {
        Bxf.Shell.Instance.ShowView("/ProjectDetails.xaml?id=" + Model.Id, null, null, null);
      }
    }
  }
}
