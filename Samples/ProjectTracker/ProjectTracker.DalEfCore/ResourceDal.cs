using System.Collections.Generic;
using System.Linq;
using ProjectTracker.Dal;

namespace ProjectTracker.DalEfCore
{
  public class ResourceDal : IResourceDal
  {
    private readonly PTrackerContext db;

    public ResourceDal(PTrackerContext context)
    {
      db = context;
    }

    public List<ResourceDto> Fetch()
    {
      var result = from r in db.Resources
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
      var result = (from r in db.Resources
                    where r.Id == id
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
      var result = (from r in db.Resources
                    where r.Id == id
                    select r.Id).Count() > 0;
      return result;
    }

    public void Insert(ResourceDto item)
    {
      var newItem = new Resource
      {
        FirstName = item.FirstName,
        LastName = item.LastName
      };
      db.Resources.Add(newItem);
      db.SaveChanges();
      item.Id = newItem.Id;
      item.LastChanged = newItem.LastChanged;
    }

    public void Update(ResourceDto item)
    {
      var data = (from r in db.Resources
                  where r.Id == item.Id
                  select r).FirstOrDefault();
      if (data == null)
        throw new DataNotFoundException("Resource");
      if (!data.LastChanged.Matches(item.LastChanged))
        throw new ConcurrencyException("Resource");

      data.FirstName = item.FirstName;
      data.LastName = item.LastName;
      var count = db.SaveChanges();
      //if (count == 0)
      //  throw new UpdateFailureException("Resource");
      item.LastChanged = data.LastChanged;
    }

    public void Delete(int id)
    {
      var data = (from r in db.Resources
                  where r.Id == id
                  select r).FirstOrDefault();
      if (data != null)
      {
        db.Resources.Remove(data);
        db.SaveChanges();
      }
    }
  }
}
