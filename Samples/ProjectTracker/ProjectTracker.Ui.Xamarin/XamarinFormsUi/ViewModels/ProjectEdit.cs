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
    public int ProjectId { get; set; }

    public ProjectEdit() { }

    public ProjectEdit(int id)
    {
      ProjectId = id;
    }

    protected override async Task<ProjectTracker.Library.ProjectEdit> DoInitAsync()
    {
      Model = await ProjectTracker.Library.ProjectEdit.GetProjectAsync(ProjectId);
      return Model;
    }
  }
}
