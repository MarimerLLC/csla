using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinFormsUi.ViewModels
{
  public class ResourceEdit : ViewModel<ProjectTracker.Library.ResourceEdit>
  {
    public ICommand SaveItemCommand { get; private set; }
    public ICommand AssignResourceCommand { get; private set; }

    public ResourceEdit(int resourceId)
    {
      SaveItemCommand = new Command(async () => await SaveAsync());
      AssignResourceCommand = new Command(() => { });
      var task = RefreshAsync<ProjectTracker.Library.ProjectEdit>(async () =>
        await ProjectTracker.Library.ResourceEdit.GetResourceEditAsync(resourceId));
    }
  }
}
