using System.Collections.Generic;
using System.Linq;
using ProjectTracker.Dal;

namespace ProjectTracker.DalEfCore
{
  public class ProjectDal : IProjectDal
  {
    private readonly PTrackerContext db;

    public ProjectDal(PTrackerContext context)
    {
      db = context;
    }

    public List<ProjectDto> Fetch()
    {
      var result = from r in db.Projects
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
      var result = from r in db.Projects
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
      var result = (from r in db.Projects
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
      var result = (from r in db.Projects
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
      db.Projects.Add(newItem);
      db.SaveChanges();
      item.Id = newItem.Id;
      item.LastChanged = newItem.LastChanged;
    }

    public void Update(ProjectDto item)
    {
      var data = (from r in db.Projects
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
      var count = db.SaveChanges();
      item.LastChanged = data.LastChanged;
    }

    public void Delete(int id)
    {
      var data = (from r in db.Projects
                  where r.Id == id
                  select r).FirstOrDefault();
      if (data != null)
      {
        db.Projects.Remove(data);
        db.SaveChanges();
      }
    }
  }
}
