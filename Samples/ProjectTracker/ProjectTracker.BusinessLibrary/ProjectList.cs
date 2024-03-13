using System;
using System.Linq;
using Csla;
using System.Collections.Generic;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectList : ReadOnlyListBase<ProjectList, ProjectInfo>
  {
    public void RemoveChild(int projectId)
    {
      var iro = IsReadOnly;
      IsReadOnly = false;
      try
      {
        var item = this.Where(r => r.Id == projectId).FirstOrDefault();
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
    private void Fetch(string name, [Inject] IProjectDal dal, [Inject] IChildDataPortal<ProjectInfo> portal)
    {
      using (LoadListMode)
      {
        List<Dal.ProjectDto> list = null;
        if (name == null)
          list = dal.Fetch();
        else
          list = dal.Fetch(name);
        foreach (var item in list)
          Add(portal.FetchChild(item));
      }
    }
  }
}