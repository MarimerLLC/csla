using System;
using System.Linq;
using System.Threading.Tasks;
using Csla;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResources : BusinessListBase<ProjectResources, ProjectResourceEdit>
  {
    public ProjectResourceEdit Assign(int resourceId)
    {
      var resource = ProjectResourceEditCreator.GetProjectResourceEditCreator(resourceId).Result;
      this.Add(resource);
      return resource;
    }

    public async Task<ProjectResourceEdit> AssignAsync(int resourceId)
    {
      var resourceCreator = await ProjectResourceEditCreator.GetProjectResourceEditCreatorAsync(resourceId);
      var resourceEdit = resourceCreator.Result;
      this.Add(resourceEdit);
      return resourceEdit;
    }

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

    [FetchChild]
    private void Fetch(int projectId, [Inject] IAssignmentDal dal)
    {
      var data = dal.FetchForProject(projectId);
      using (LoadListMode)
      {
        foreach (var item in data)
          Add(DataPortal.FetchChild<ProjectResourceEdit>(item));
      }
    }
  }
}