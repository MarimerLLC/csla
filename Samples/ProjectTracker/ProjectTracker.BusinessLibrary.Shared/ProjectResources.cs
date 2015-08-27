using System;
using System.Linq;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResources : BusinessListBase<ProjectResources, ProjectResourceEdit>
  {
#if !NETFX_CORE
    public ProjectResourceEdit Assign(int resourceId)
    {
      var resource = ProjectResourceEditCreator.GetProjectResourceEditCreator(resourceId).Result;
      this.Add(resource);
      return resource;
    }
#elif __ANDROID__
      public async System.Threading.Tasks.Task<ProjectResourceEdit> AssignAsync(int resourceId)
      {
          var resource = (await ProjectResourceEditCreator.GetProjectResourceEditCreatorAsync(resourceId)).Result;
          this.Add(resource);
          return resource;
      }
#endif

    public void Remove(int resourceId)
    {
      var item = (from r in this
                    where r.ResourceId == resourceId
                    select r).FirstOrDefault();
      if (item != null)
        Remove(item);
    }

    public bool Contains(int resourceId)
    {
      var item = (from r in this
                  where r.ResourceId == resourceId
                  select r).Count();
      return item > 0;
    }

    public bool ContainsDeleted(int resourceId)
    {
      var item = (from r in DeletedList
                  where r.ResourceId == resourceId
                  select r).Count();
      return item > 0;
    }

#if !NETFX_CORE
    private void Child_Fetch(int projectId)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IAssignmentDal>();
        var data = dal.FetchForProject(projectId);
        var rlce = RaiseListChangedEvents;
        RaiseListChangedEvents = false;
        foreach (var item in data)
          Add(DataPortal.FetchChild<ProjectResourceEdit>(item));
        RaiseListChangedEvents = rlce;
      }
    }
#endif
  }
}