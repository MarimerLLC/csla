using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WinRtUI.ViewModel
{
  public class ProjectTrackerDataSource
  {
    public static ObservableCollection<DataGroup> GetGroups(string p)
    {
      return new ProjectTrackerDataSource().ItemGroups;
    }

    private ObservableCollection<DataGroup> _itemGroups = new ObservableCollection<DataGroup>();
    public ObservableCollection<DataGroup> ItemGroups
    {
      get { return this._itemGroups; }
    }

    public ProjectTrackerDataSource()
    {
      _itemGroups.Add(new DataGroup(
        "Projects",
        "Projects",
        "",
        "Assets/MediumGray.png",
        "View or edit project information"));
      _itemGroups.Add(new DataGroup(
        "Resources",
        "Resources",
        "",
        "Assets/DarkGray.png",
        "View or edit resource information"));
      _itemGroups.Add(new DataGroup(
        "Roles",
        "Roles",
        "",
        "Assets/LightGray.png",
        "View or edit roles"));
      LoadDashboard();
    }

    private async void LoadDashboard()
    {
      var db = await ProjectTracker.Library.Dashboard.GetDashboardAsync();
      _itemGroups.Where(r => r.UniqueId == "Projects").First().Subtitle = string.Format("{0} projects ({1} open)", db.ProjectCount, db.OpenProjectCount);
      _itemGroups.Where(r => r.UniqueId == "Resources").First().Subtitle = string.Format("{0} resources", db.ResourceCount);
    }
  }

  public class ItemList : ObservableCollection<DataGroup>
  {
    private static List<string> _imageList = new List<string>
      {
        "Assets/LightGray.png",
        "Assets/MediumGray.png",
        "Assets/DarkGray.png"
      };

    public ItemList(ProjectTracker.Library.ProjectList list)
    {
      int imageIndex = 0;
      foreach (var item in list)
      {
        Add(new DataGroup(item.Id.ToString(), item.Name, "Project", _imageList[imageIndex], ""));
        imageIndex++;
        if (imageIndex >= _imageList.Count) imageIndex = 0;
      }
    }
  }

  public class DataGroup : WinRtUI.Common.BindableBase
  {
    private static Uri _baseUri = new Uri("ms-appx:///");

    public DataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
    {
      this._uniqueId = uniqueId;
      this._title = title;
      this._subtitle = subtitle;
      this._description = description;
      this._imagePath = imagePath;
    }

    private string _uniqueId = string.Empty;
    public string UniqueId
    {
      get { return this._uniqueId; }
      set { this.SetProperty(ref this._uniqueId, value); }
    }

    private string _title = string.Empty;
    public string Title
    {
      get { return this._title; }
      set { this.SetProperty(ref this._title, value); }
    }

    private string _subtitle = string.Empty;
    public string Subtitle
    {
      get { return this._subtitle; }
      set { this.SetProperty(ref this._subtitle, value); }
    }

    private string _description = string.Empty;
    public string Description
    {
      get { return this._description; }
      set { this.SetProperty(ref this._description, value); }
    }

    private ImageSource _image = null;
    private String _imagePath = null;
    public ImageSource Image
    {
      get
      {
        if (this._image == null && this._imagePath != null)
        {
          this._image = new BitmapImage(new Uri(_baseUri, this._imagePath));
        }
        return this._image;
      }

      set
      {
        this._imagePath = null;
        this.SetProperty(ref this._image, value);
      }
    }

    public void SetImage(String path)
    {
      this._image = null;
      this._imagePath = path;
      this.OnPropertyChanged("Image");
    }
  }
}
