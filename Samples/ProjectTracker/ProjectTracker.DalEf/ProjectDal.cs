using System.Collections.Generic;
using System.Linq;
using ProjectTracker.Dal;
using System.Data;

namespace ProjectTracker.DalEf
{
  public class ProjectDal : IProjectDal
  {
    private readonly PTrackerEntities objectContext;

    public ProjectDal(PTrackerEntities context)
    {
      objectContext = context;
    }

    public List<ProjectDto> Fetch()
    {
      var result = from r in objectContext.Projects
                   select new ProjectDto
                   {
                     Id = r.Id,
                     Name = r.Name,
                     Description = r.Description,
                     Started = r.Started,
                     Ended = r.Ended,
                     LastChanged = r.LastChanged
                   };
      return result.ToList();
    }

    public List<ProjectDto> Fetch(string nameFilter)
    {
      var result = from r in objectContext.Projects
                   where r.Name.Contains(nameFilter)
                   select new ProjectDto
                   {
                     Id = r.Id,
                     Name = r.Name,
                     Description = r.Description,
                     Started = r.Started,
                     Ended = r.Ended,
                     LastChanged = r.LastChanged
                   };
      return result.ToList();
    }

    public ProjectDto Fetch(int id)
    {
      var result = (from r in objectContext.Projects
                    where r.Id == id
                    select new ProjectDto
                    {
                      Id = r.Id,
                      Name = r.Name,
                      Description = r.Description,
                      Started = r.Started,
                      Ended = r.Ended,
                      LastChanged = r.LastChanged
                    }).FirstOrDefault();
      if (result == null)
        throw new DataNotFoundException("Project");
      return result;
    }

    public bool Exists(int id)
    {
      var result = (from r in objectContext.Projects
                    where r.Id == id
                    select r.Id).Count() > 0;
      return result;
    }

    public void Insert(ProjectDto item)
    {
      var newItem = new Project
      {
        Name = item.Name,
        Description = item.Description,
        Started = item.Started,
        Ended = item.Ended
      };
      objectContext.AddToProjects(newItem);
      objectContext.SaveChanges();
      item.Id = newItem.Id;
      item.LastChanged = newItem.LastChanged;
    }

    public void Update(ProjectDto item)
    {
      var data = (from r in objectContext.Projects
                  where r.Id == item.Id
                  select r).FirstOrDefault();
      if (data == null)
        throw new DataNotFoundException("Project");
      if (!data.LastChanged.Matches(item.LastChanged))
        throw new ConcurrencyException("Project");

      data.Name = item.Name;
      data.Description = item.Description;
      data.Started = item.Started;
      data.Ended = item.Ended;
      var count = objectContext.SaveChanges();
      if (count == 0)
        throw new UpdateFailureException("Project");
      item.LastChanged = data.LastChanged;
    }

    public void Delete(int id)
    {
      var data = (from r in objectContext.Projects
                  where r.Id == id
                  select r).FirstOrDefault();
      if (data != null)
      {
        objectContext.Projects.DeleteObject(data);
        objectContext.SaveChanges();
      }
    }
  }
}
