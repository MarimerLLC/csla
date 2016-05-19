using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTracker.Library;

namespace XamarinFormsUi.ViewModels
{
  public class ProjectEdit : ViewModel<ProjectTracker.Library.ProjectEdit>
  {
    private int _projectId;

    public ProjectEdit(int id)
    {
      _projectId = id;
    }

    protected override async Task<ProjectTracker.Library.ProjectEdit> DoInitAsync()
    {
      Model = await ProjectTracker.Library.ProjectEdit.GetProjectAsync(_projectId);
      return Model;
    }
  }
}
