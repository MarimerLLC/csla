using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WpUI.ViewModels
{
  public class ResourceList : ViewModel<ProjectTracker.Library.ResourceList>
  {
    public ResourceList()
    {
      BeginRefresh(ProjectTracker.Library.ResourceList.GetResourceList);
    }

    protected override void OnModelChanged(ProjectTracker.Library.ResourceList oldValue, ProjectTracker.Library.ResourceList newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      if (newValue != null)
        newValue.CollectionChanged += (sender, args) => OnPropertyChanged("ItemList");
      OnPropertyChanged("ItemList");
    }

    public ObservableCollection<ResourceInfo> ItemList
    {
      get
      {
        if (Model == null)
          return null;
        else
          return new ObservableCollection<ResourceInfo>(
            Model.Select(r => new ResourceInfo(r, this)));
      }
    }

    public bool CanAdd
    {
      get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectTracker.Library.ResourceEdit)); }
    }

    public void AddItem()
    {
      App.ViewModel.ShowView("/ResourceEdit.xaml");
    }

    public void ShowDetail(object sender, Bxf.Xaml.ExecuteEventArgs e)
    {
      var item = ((FrameworkElement)e.TriggerSource).DataContext as ResourceInfo;
      if (item != null)
        App.ViewModel.ShowView("/ResourceDetails.xaml?id=" + item.Model.Id);
    }

    public class ResourceInfo : ViewModelLocal<ProjectTracker.Library.ResourceInfo>
    {
      public ResourceList Parent { get; private set; }

      public ResourceInfo(ProjectTracker.Library.ResourceInfo info, ResourceList parent)
      {
        Parent = parent;
        Model = info;
      }

      public bool CanEdit
      {
        get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectTracker.Library.ResourceEdit)); }
      }

      public new bool CanRemove
      {
        get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ProjectTracker.Library.ResourceEdit)); }
      }

      public void DisplayItem()
      {
        App.ViewModel.ShowView("/ResourceDisplay.xaml?id=" + Model.Id);
      }

      public void EditItem()
      {
        App.ViewModel.ShowView("/ResourceEdit.xaml?id=" + Model.Id);
      }

      public void RemoveItem()
      {
        App.ViewModel.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting item..." });
        ProjectTracker.Library.ResourceEdit.DeleteResource(Model.Id, (o, e) =>
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
