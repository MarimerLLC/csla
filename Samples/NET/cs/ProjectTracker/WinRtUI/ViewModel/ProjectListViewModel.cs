using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;

namespace WinRtUI.ViewModel
{
  public class ProjectListViewModel : ViewModel<ProjectTracker.Library.ProjectList>
  {
    protected async override Task<ProjectTracker.Library.ProjectList> DoInitAsync()
    {
      return await ProjectTracker.Library.ProjectList.GetProjectListAsync();
    }

    protected override void OnRefreshed()
    {
      base.OnRefreshed();
      OnPropertyChanged("Items");
    }

    public ObservableCollection<ProjectInfoViewModel> Items
    {
      get
      {
        ObservableCollection<ProjectInfoViewModel> result = null;
        if (Model != null)
          result = new ObservableCollection<ProjectInfoViewModel>(
            Model.Select(r => new ProjectInfoViewModel(r)));

        return result;
      }
    }
  }

  public class ProjectInfoViewModel : ViewModel<ProjectTracker.Library.ProjectInfo>
  {
    private static Uri _baseUri = new Uri("ms-appx:///");
    private static List<string> _imageList = new List<string>
      {
        "Assets/LightGray.png",
        "Assets/MediumGray.png",
        "Assets/DarkGray.png"
      };
    private static int _imageIndex = 0;

    public ProjectInfoViewModel(ProjectTracker.Library.ProjectInfo info)
    {
      Model = info;
      _imagePath = _imageList[_imageIndex];
      _imageIndex++;
      if (_imageIndex >= _imageList.Count) _imageIndex = 0;
    }

    private ProjectEditViewModel _ProjectEdit = null;
    public ProjectEditViewModel ProjectEditViewModel
    {
      get
      {
        if (_ProjectEdit == null)
        {
          IsBusy = true;
          new ProjectEditViewModel(Model.Id, this).InitAsync().ContinueWith(t =>
          {
            ProjectEditViewModel = (ProjectEditViewModel)t.Result;
            IsBusy = false;
          },
              TaskContinuationOptions.ExecuteSynchronously);
        }
        return _ProjectEdit;
      }
      private set
      {
        _ProjectEdit = value;
        OnPropertyChanged("ProjectEditViewModel");
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

    internal void UpdateInfo(ProjectTracker.Library.ProjectEdit projectEdit)
    {
      Model.SetName(projectEdit);
    }
  }

  public class ProjectEditViewModel : ViewModel<ProjectTracker.Library.ProjectEdit>
  {
    private int _ProjectId;
    private ProjectInfoViewModel _parent;

    public ProjectEditViewModel(int id, ProjectInfoViewModel parent)
    {
      _ProjectId = id;
      _parent = parent;
    }

    protected override async Task<ProjectTracker.Library.ProjectEdit> DoInitAsync()
    {
      var result = await ProjectTracker.Library.ProjectEdit.GetProjectAsync(_ProjectId);
      return result;
    }

    public new async Task<ProjectTracker.Library.ProjectEdit> SaveAsync()
    {
      var result = await base.SaveAsync();
      _parent.UpdateInfo(Model);
      return result;
    }
  }
}
