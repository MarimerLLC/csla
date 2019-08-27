using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTracker.Library;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinFormsUi.ViewModels
{
  public class ProjectEdit : ViewModel<ProjectTracker.Library.ProjectEdit>
  {
    public ICommand SaveItemCommand { get; private set; }
    public ICommand AssignResourceCommand { get; private set; }

    public ProjectEdit(int projectId)
    {
      SaveItemCommand = new Command(async () => await SaveAsync());
      AssignResourceCommand = new Command(() => { });
      var task = RefreshAsync<ProjectTracker.Library.ProjectEdit>(async () =>
        await ProjectTracker.Library.ProjectEdit.GetProjectAsync(projectId));
    }
  }
}
