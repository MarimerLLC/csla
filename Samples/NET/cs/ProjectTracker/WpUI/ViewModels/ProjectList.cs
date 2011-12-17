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

    protected override void OnModelChanged(ProjectTracker.Library.ProjectList oldValue, ProjectTracker.Library.ProjectList newValue)
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
            Model.Select(r => new ProjectInfo(r, this)));
      }
    }

    public bool CanAdd
    {
      get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectTracker.Library.ProjectEdit)); }
    }

    public void AddItem()
    {
      App.ViewModel.ShowView("/ProjectEdit.xaml");
    }

    public void ShowDetail(object sender, Bxf.Xaml.ExecuteEventArgs e)
    {
      var item = ((FrameworkElement)e.TriggerSource).DataContext as ProjectInfo;
      if (item != null)
        App.ViewModel.ShowView("/ProjectDetails.xaml?id=" + item.Model.Id);
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
        App.ViewModel.ShowView("/ProjectDisplay.xaml?id=" + Model.Id);
      }

      public void EditItem()
      {
        App.ViewModel.ShowView("/ProjectEdit.xaml?id=" + Model.Id);
      }

      public void RemoveItem()
      {
        App.ViewModel.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting item..." });
        ProjectTracker.Library.ProjectEdit.DeleteProject(Model.Id, (o, e) =>
        {
          if (e.Error != null)
          {
            App.ViewModel.ShowError(e.Error.Message, "Failed to delete item");
            App.ViewModel.ShowStatus(new Bxf.Status { IsOk = false, Text = "Item NOT deleted" });
          }
          else
          {
            Parent.Model.RemoveChild(Model.Id);
            App.ViewModel.ShowStatus(new Bxf.Status { Text = "Item deleted" });
          }
        });
      }
    }
  }
}
