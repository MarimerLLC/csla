using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WinRtUI.ViewModel
{
  public class ResourceListViewModel : ViewModel<ProjectTracker.Library.ResourceList>
  {
    public ResourceListViewModel()
    {
      BeginRefresh(ProjectTracker.Library.ResourceList.GetResourceList);
    }

    protected override void OnRefreshed()
    {
      base.OnRefreshed();
      OnPropertyChanged("Items");
    }

    public ObservableCollection<ResourceInfoViewModel> Items
    {
      get
      {
        ObservableCollection<ResourceInfoViewModel> result = null;
        if (Model != null)
          result = new ObservableCollection<ResourceInfoViewModel>(
            Model.Select(r => new ResourceInfoViewModel(r)));

        return result;
      }
    }
  }

  public class ResourceInfoViewModel : ViewModel<ProjectTracker.Library.ResourceInfo>
  {
    private static Uri _baseUri = new Uri("ms-appx:///");
    private static List<string> _imageList = new List<string>
      {
        "Assets/LightGray.png",
        "Assets/MediumGray.png",
        "Assets/DarkGray.png"
      };
    private static int _imageIndex = 0;

    public ResourceInfoViewModel(ProjectTracker.Library.ResourceInfo info)
    {
      Model = info;
      _imagePath = _imageList[_imageIndex];
      _imageIndex++;
      if (_imageIndex >= _imageList.Count) _imageIndex = 0;
    }

    private ImageSource _image = null;
    private String _imagePath = null;
    public ImageSource Image
    {
      get
      {
        if (this._image == null && this._imagePath != null)
          this._image = new BitmapImage(new Uri(_baseUri, this._imagePath));
        return this._image;
      }
      set
      {
        _imagePath = null;
        _image = value;
        OnPropertyChanged("Image");
      }
    }
  }
}
