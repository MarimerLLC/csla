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
            Model.Select(r => new ResourceInfo(r)));
      }
    }

    public bool CanAdd
    {
      get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectTracker.Library.ResourceEdit)); }
    }

    public void AddItem()
    {
      Bxf.Shell.Instance.ShowView("/ResourceEdit.xaml", null, null, null);
    }

    public void ShowDetail(object sender, Bxf.Xaml.ExecuteEventArgs e)
    {
      var item = ((FrameworkElement)e.TriggerSource).DataContext as ResourceInfo;
      if (item != null)
        Bxf.Shell.Instance.ShowView("/ResourceDetails.xaml?id=" + item.Model.Id, null, null, null);
    }

    public class ResourceInfo : ViewModelLocal<ProjectTracker.Library.ResourceInfo>
    {
      public ResourceInfo(ProjectTracker.Library.ResourceInfo info)
      {
        Model = info;
      }

      public void DisplayItem()
      {
        Bxf.Shell.Instance.ShowView("/ResourceDisplay.xaml?id=" + Model.Id, null, null, null);
      }
    }
  }
}
