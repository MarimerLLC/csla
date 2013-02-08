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
    protected async override Task<ProjectTracker.Library.ResourceList> DoInitAsync()
    {
      return await ProjectTracker.Library.ResourceList.GetResourceListAsync();
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

    private ResourceEditViewModel _resourceEdit = null;
    public ResourceEditViewModel ResourceEditViewModel
    {
      get
      {
        if (_resourceEdit == null)
        {
          IsBusy = true;
          new ResourceEditViewModel(Model.Id, this).InitAsync().ContinueWith(t => 
              {
                ResourceEditViewModel = (ResourceEditViewModel)t.Result;
                IsBusy = false;
              }, 
              TaskContinuationOptions.ExecuteSynchronously);
        }
        return _resourceEdit;
      }
      private set
      {
        _resourceEdit = value;
        OnPropertyChanged("ResourceEditViewModel");
      }
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

    internal void UpdateInfo(ProjectTracker.Library.ResourceEdit resourceEdit)
    {
      Model.SetName(resourceEdit);
    }
  }

  public class ResourceEditViewModel : ViewModel<ProjectTracker.Library.ResourceEdit>
  {
    private int _resourceId;
    private ResourceInfoViewModel _parent;

    public ResourceEditViewModel(int id, ResourceInfoViewModel parent)
    {
      _resourceId = id;
      _parent = parent;
    }

    protected override async Task<ProjectTracker.Library.ResourceEdit> DoInitAsync()
    {
      var result = await ProjectTracker.Library.ResourceEdit.GetResourceEditAsync(_resourceId);
      return result;
    }

    public new async Task<ProjectTracker.Library.ResourceEdit> SaveAsync()
    {
      var result = await base.SaveAsync();
      _parent.UpdateInfo(Model);
      return result;
    }
  }
}
