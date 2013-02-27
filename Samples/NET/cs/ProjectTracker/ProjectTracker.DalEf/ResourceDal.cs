using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using Csla.Data;
using System.Data;

namespace ProjectTracker.DalEf
{
  public class ResourceDal : IResourceDal
  {
    public List<ResourceDto> Fetch()
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = from r in ctx.ObjectContext.Resources
                     select new ResourceDto
                     {
                       Id = r.Id,
                       FirstName = r.FirstName,
                       LastName = r.LastName,
                       LastChanged = r.LastChanged
                     };
        return result.ToList();
      }
    }

    public ResourceDto Fetch(int id)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = (from r in ctx.ObjectContext.Resources
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
    }

    public bool Exists(int id)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = (from r in ctx.ObjectContext.Resources
                      where r.Id == id
                      select r.Id).Count() > 0;
        return result;
      }
    }

    public void Insert(ResourceDto item)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var newItem = new Resource
        {
          FirstName = item.FirstName,
          LastName = item.LastName
        };
        ctx.ObjectContext.AddToResources(newItem);
        ctx.ObjectContext.SaveChanges();
        item.Id = newItem.Id;
        item.LastChanged = newItem.LastChanged;
      }
    }

    public void Update(ResourceDto item)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var data = (from r in ctx.ObjectContext.Resources
                    where r.Id == item.Id
                    select r).FirstOrDefault();
        if (data == null)
          throw new DataNotFoundException("Resource");
        if (!data.LastChanged.Matches(item.LastChanged))
          throw new ConcurrencyException("Resource");

        data.FirstName = item.FirstName;
        data.LastName = item.LastName;
        var count = ctx.ObjectContext.SaveChanges();
        if (count == 0)
          throw new UpdateFailureException("Resource");
        item.LastChanged = data.LastChanged;
      }
    }

    public void Delete(int id)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var data = (from r in ctx.ObjectContext.Resources
                    where r.Id == id
                    select r).FirstOrDefault();
        if (data != null)
        {
          ctx.ObjectContext.Resources.DeleteObject(data);
          ctx.ObjectContext.SaveChanges();
        }
      }
    }
  }
}
