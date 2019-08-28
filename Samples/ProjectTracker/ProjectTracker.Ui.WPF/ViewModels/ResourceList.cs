using System;
using System.Linq;
using System.Collections.ObjectModel;
using ProjectTracker.Library;
using System.Threading.Tasks;
using Csla;

namespace WpfUI.ViewModels
{
  public class ResourceList : ViewModel<ProjectTracker.Library.ResourceList>
  {
    public ResourceList()
    {
      var task = RefreshAsync<ProjectTracker.Library.ResourceList>(
        async () => await ProjectTracker.Library.ResourceList.GetResourceListAsync());
    }

    protected override void OnModelChanged(ProjectTracker.Library.ResourceList oldValue, ProjectTracker.Library.ResourceList newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      if (newValue != null)
        newValue.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(ItemList));
      OnPropertyChanged(nameof(ItemList));
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
      get { return Csla.Rules.BusinessRules.HasPermission(
        Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectTracker.Library.ResourceEdit)); }
    }

    public void AddItem()
    {
      Bxf.Shell.Instance.ShowView(
        typeof(Views.ResourceEdit).AssemblyQualifiedName,
        "resourceGetterViewSource",
        new ResourceGetter(),
        "Main");
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
        Bxf.Shell.Instance.ShowView(
          typeof(Views.ResourceDisplay).AssemblyQualifiedName,
          "resourceDisplayViewSource",
          new ResourceDisplay(Model.Id),
          "Main");
      }

      public void EditItem()
      {
        Bxf.Shell.Instance.ShowView(
          typeof(Views.ResourceEdit).AssemblyQualifiedName,
          "resourceGetterViewSource",
          new ResourceGetter(Model),
          "Main");
      }

      public async void RemoveItem()
      {
        Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Getting item to delete..." });
        try
        {
          var obj = await ResourceEdit.GetResourceEditAsync(Model.Id);
          obj.Delete();
          Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting item..." });
          try
          {
            obj = await obj.SaveAsync();
            Parent.Model.RemoveChild(Model.Id);
            Bxf.Shell.Instance.ShowStatus(new Bxf.Status { Text = "Item deleted" });
          }
          catch (Exception ex)
          { 
              Bxf.Shell.Instance.ShowError(ex.Message, "Failed to delete item");
              Bxf.Shell.Instance.ShowStatus(new Bxf.Status { IsOk = false, Text = "Item NOT deleted" });
          }
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
