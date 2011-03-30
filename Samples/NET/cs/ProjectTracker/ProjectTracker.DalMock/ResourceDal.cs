using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using ProjectTracker.DalMock.MockDbTypes;

namespace ProjectTracker.DalMock
{
  public class ResourceDal : IResourceDal
  {
    public List<ResourceDto> Fetch()
    {
      var result = from r in MockDb.Resources
                   select new ResourceDto
                   {
                     Id = r.Id,
                     FirstName = r.FirstName,
                     LastName = r.LastName,
                     LastChanged = r.LastChanged
                   };
      return result.ToList();
    }

    public ResourceDto Fetch(int id)
    {
      var result = (from r in MockDb.Resources
                    select new ResourceDto
                    {
                      Id = r.Id,
                      FirstName = r.FirstName,
                      LastName = r.LastName,
                      LastChanged = r.LastChanged
                    }).FirstOrDefault();
      if (result == null)
        throw new DataNotFoundException("Resource");
      return result;
    }

    public bool Exists(int id)
    {
      var result = (from r in MockDb.Resources
                    where r.Id == id
                    select r.Id).Count() > 0;
      return result;
    }

    public void Insert(ResourceDto item)
    {
      item.Id = MockDb.Resources.Max(c => c.Id) + 1;
      var newItem = new ResourceData
      {
        Id = item.Id,
        FirstName = item.FirstName,
        LastName = item.LastName,
        LastChanged = MockDb.GetTimeStamp()
      };
      MockDb.Resources.Add(newItem);
    }

    public void Update(ResourceDto item)
    {
      var data = (from r in MockDb.Resources
                  where r.Id == item.Id
                  select r).FirstOrDefault();
      if (data == null)
        throw new DataNotFoundException("Resource");
      if (!MockDb.TimeStampEquals(data.LastChanged, item.LastChanged))
        throw new ConcurrencyException("Project");
      data.FirstName = item.FirstName;
      data.LastName = item.LastName;
      data.LastChanged = MockDb.GetTimeStamp();
    }

    public void Delete(int id)
    {
      var data = (from r in MockDb.Resources
                  where r.Id == id
                  select r).FirstOrDefault();
      if (data != null)
        MockDb.Resources.Remove(data);
    }
  }
}
