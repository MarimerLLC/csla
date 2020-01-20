using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using ProjectTracker.Library;
using System.Threading.Tasks;
using ProjectTracker.Ui.Xamarin;

namespace XamarinFormsUi.ViewModels
{
  public class ResourceList : ViewModel<ProjectTracker.Library.ResourceList>
  {
    public ResourceList()
    {
      var task = RefreshAsync<ProjectTracker.Library.ResourceList>(async () =>
        await ProjectTracker.Library.ResourceList.GetResourceListAsync());
    }

    protected override void OnModelChanged(ProjectTracker.Library.ResourceList oldValue, ProjectTracker.Library.ResourceList newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      if (newValue != null)
        newValue.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(ItemList));
      OnPropertyChanged(nameof(ItemList));
    }

    internal async void EditItem(ProjectTracker.Library.ResourceInfo item)
    {
      await App.NavigateTo(typeof(Views.ResourceEdit), item.Id);
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
      }

      public async void EditItem()
      {
        await App.NavigateTo(typeof(Views.ResourceEdit), 1);
      }

      public async void RemoveItem()
      {
        await ProjectTracker.Library.ResourceEdit.DeleteResourceEditAsync(Model.Id);
        Parent.Model.RemoveChild(Model.Id);
      }
    }
  }
}
