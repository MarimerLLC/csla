using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WinRTUI.ViewModel
{
  public class ProjectTrackerDataSource
  {
    public static async Task<ObservableCollection<Data.SampleDataGroup>> GetGroupsAsync()
    {
      //return await Task.Run<ObservableCollection<Data.SampleDataGroup>>(() => new ProjectTrackerDataSource().ItemGroups);
      return new ProjectTrackerDataSource().ItemGroups;
    }

    private ObservableCollection<Data.SampleDataGroup> _itemGroups = new ObservableCollection<Data.SampleDataGroup>();
    public ObservableCollection<Data.SampleDataGroup> ItemGroups
    {
      get { return this._itemGroups; }
    }

    public ProjectTrackerDataSource()
    {
      _itemGroups.Add(new Data.SampleDataGroup(
        "Projects",
        "Projects",
        "Project info",
        "Assets/MediumGray.png",
        "View or edit project information"));
      _itemGroups.Add(new Data.SampleDataGroup(
        "Resources",
        "Resources",
        "Resource info",
        "Assets/DarkGray.png",
        "View or edit resource information"));
      _itemGroups.Add(new Data.SampleDataGroup(
        "Roles",
        "Roles",
        "Role info",
        "Assets/LightGray.png",
        "View or edit roles"));
      LoadDetails();
    }

    private void LoadDetails()
    {
      LoadDashboard();
      LoadProjects(_itemGroups[0]);
      LoadResources(_itemGroups[1]);
      LoadRoles(_itemGroups[2]);
    }

    private async void LoadProjects(Data.SampleDataGroup sampleDataGroup)
    {
      var source = await ProjectTracker.Library.ProjectList.GetProjectListAsync();
      foreach (var sourceItem in source)
      {
        sampleDataGroup.Items.Add(new Data.SampleDataItem(
          sourceItem.Id.ToString(), sourceItem.Name, sourceItem.Name, "Assets/MediumGray.png", null, null));
      }
    }

    private async void LoadResources(Data.SampleDataGroup sampleDataGroup)
    {
      var source = await ProjectTracker.Library.ResourceList.GetResourceListAsync();
      foreach (var sourceItem in source)
      {
        sampleDataGroup.Items.Add(new Data.SampleDataItem(
          sourceItem.Id.ToString(), sourceItem.Name, sourceItem.Name, "Assets/DarkGray.png", null, null));
      }
    }

    private async void LoadRoles(Data.SampleDataGroup sampleDataGroup)
    {
      var source = await ProjectTracker.Library.RoleList.GetListAsync();
      foreach (var sourceItem in source)
      {
        sampleDataGroup.Items.Add(new Data.SampleDataItem(
          sourceItem.Key.ToString(), sourceItem.Value, sourceItem.Value, "Assets/LightGray.png", null, null));
      }
    }

    private async void LoadDashboard()
    {
      var db = await ProjectTracker.Library.Dashboard.GetDashboardAsync();
      _itemGroups.Where(r => r.UniqueId == "Projects").First().Subtitle = string.Format("{0} projects ({1} open)", db.ProjectCount, db.OpenProjectCount);
      _itemGroups.Where(r => r.UniqueId == "Resources").First().Subtitle = string.Format("{0} resources", db.ResourceCount);
    }
  }
}
