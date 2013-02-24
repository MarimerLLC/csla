using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using Csla.Data;
using System.Data;

namespace ProjectTracker.DalEf
{
  public class ProjectDal : IProjectDal
  {
    public List<ProjectDto> Fetch()
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = from r in ctx.ObjectContext.Projects
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
    }

    public List<ProjectDto> Fetch(string nameFilter)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = from r in ctx.ObjectContext.Projects
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
    }

    public ProjectDto Fetch(int id)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = (from r in ctx.ObjectContext.Projects
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
    }

    public bool Exists(int id)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = (from r in ctx.ObjectContext.Projects
                      where r.Id == id
                      select r.Id).Count() > 0;
        return result;
      }
    }

    public void Insert(ProjectDto item)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var newItem = new Project
        {
          Name = item.Name,
          Description = item.Description,
          Started = item.Started,
          Ended = item.Ended
        };
        ctx.ObjectContext.AddToProjects(newItem);
        ctx.ObjectContext.SaveChanges();
        item.Id = newItem.Id;
        item.LastChanged = newItem.LastChanged;
      }
    }

    public void Update(ProjectDto item)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var data = (from r in ctx.ObjectContext.Projects
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
        var count = ctx.ObjectContext.SaveChanges();
        if (count == 0)
          throw new UpdateFailureException("Project");
        item.LastChanged = data.LastChanged;
      }
    }

    public void Delete(int id)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var data = (from r in ctx.ObjectContext.Projects
                    where r.Id == id
                    select r).FirstOrDefault();
        if (data != null)
        {
          ctx.ObjectContext.Projects.DeleteObject(data);
          ctx.ObjectContext.SaveChanges();
        }
      }
    }
  }
}
