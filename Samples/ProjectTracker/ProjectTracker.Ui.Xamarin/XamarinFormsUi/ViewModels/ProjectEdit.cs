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

    public int ProjectId { get; set; }

    public ProjectEdit()
    {
      SaveItemCommand = new Command(async () => await SaveAsync());
      AssignResourceCommand = new Command(() => { });
    }

    protected override async Task<ProjectTracker.Library.ProjectEdit> DoInitAsync()
    {
      return await ProjectTracker.Library.ProjectEdit.GetProjectAsync(ProjectId);
    }
  }
}
