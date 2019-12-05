using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using ProjectTracker.DalMock.MockDbTypes;

namespace ProjectTracker.DalMock
{
  public class ProjectDal : IProjectDal
  {
    public List<ProjectDto> Fetch()
    {
      var result = from r in MockDb.Projects
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
      var result = from r in MockDb.Projects
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
      var result = (from r in MockDb.Projects
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
      var result = (from r in MockDb.Projects
                    where r.Id == id
                    select r.Id).Count() > 0;
      return result;
    }

    public void Insert(ProjectDto item)
    {
      item.Id = MockDb.Projects.Max(c => c.Id) + 1;
      item.LastChanged = MockDb.GetTimeStamp();
      var newItem = new ProjectData
      {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        Started = item.Started,
        Ended = item.Ended,
        LastChanged = item.LastChanged
      };
      MockDb.Projects.Add(newItem);
    }

    public void Update(ProjectDto item)
    {
      var data = (from r in MockDb.Projects
                  where r.Id == item.Id
                  select r).FirstOrDefault();
      if (data == null)
        throw new DataNotFoundException("Project");
      if (!data.LastChanged.Matches(item.LastChanged))
        throw new ConcurrencyException("Project");
      
      item.LastChanged = MockDb.GetTimeStamp();

      data.Name = item.Name;
      data.Description = item.Description;
      data.Started = item.Started;
      data.Ended = item.Ended;
      data.LastChanged = item.LastChanged;
    }

    public void Delete(int id)
    {
      var data = (from r in MockDb.Projects
                  where r.Id == id
                  select r).FirstOrDefault();
      if (data != null)
        MockDb.Projects.Remove(data);

      var assignments = (from r in MockDb.Assignments
                         where r.ProjectId == id
                         select r).ToList();
      foreach (var item in assignments)
        MockDb.Assignments.Remove(item);
    }
  }
}
