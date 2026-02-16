using System.Linq;
using Csla;
using System.Collections.Generic;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  public class ProjectList : ReadOnlyListBase<ProjectList, ProjectInfo>
  {
    public void RemoveChild(int projectId)
    {
      var iro = IsReadOnly;
      IsReadOnly = false;
      try
      {
        ProjectInfo? item = this.Where(r => r.Id == projectId).FirstOrDefault();
        if (item != null)
        {
          var index = this.IndexOf(item);
          Remove(item);
        }
      }
      finally
      {
        IsReadOnly = iro;
      }
    }

    [Fetch]
    private void Fetch([Inject] IProjectDal dal, [Inject] IChildDataPortal<ProjectInfo> portal)
    {
      Fetch(null, dal, portal);
    }

    [Fetch]
    private void Fetch(string? name, [Inject] IProjectDal dal, [Inject] IChildDataPortal<ProjectInfo> portal)
    {
      using (LoadListMode)
      {
        List<Dal.ProjectDto> list = name == null ? dal.Fetch() : dal.Fetch(name);
        foreach (var item in list)
          Add(portal.FetchChild(item));
      }
    }
  }
}