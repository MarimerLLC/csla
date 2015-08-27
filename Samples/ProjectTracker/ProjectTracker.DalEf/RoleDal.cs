using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using Csla.Data;
using System.Data;

namespace ProjectTracker.DalEf
{
  public class RoleDal : IRoleDal
  {
    public List<RoleDto> Fetch()
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = from r in ctx.ObjectContext.Roles
                     select new RoleDto { Id = r.Id, Name = r.Name, LastChanged = r.LastChanged };
        return result.ToList();
      }
    }

    public RoleDto Fetch(int id)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var result = (from r in ctx.ObjectContext.Roles
                      where r.Id == id
                      select new RoleDto { Id = r.Id, Name = r.Name, LastChanged = r.LastChanged }).FirstOrDefault();
        if (result == null)
          throw new DataNotFoundException("Role");
        return result;
      }
    }

    public void Insert(RoleDto item)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var newItem = new Role 
        { 
          Name = item.Name
        };
        ctx.ObjectContext.AddToRoles(newItem);
        ctx.ObjectContext.SaveChanges();
        item.Id = newItem.Id;
        item.LastChanged = newItem.LastChanged;
      }
    }

    public void Update(RoleDto item)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var data = (from r in ctx.ObjectContext.Roles
                    where r.Id == item.Id
                    select r).FirstOrDefault();
        if (data == null)
          throw new DataNotFoundException("Role");
        if (!data.LastChanged.Matches(item.LastChanged))
          throw new ConcurrencyException("Role");

        data.Name = item.Name;
        var count = ctx.ObjectContext.SaveChanges();
        if (count == 0)
          throw new UpdateFailureException("Role");
        item.LastChanged = data.LastChanged;
      }
    }

    public void Delete(int id)
    {
      using (var ctx = ObjectContextManager<PTrackerEntities>.GetManager("PTrackerEntities"))
      {
        var data = (from r in ctx.ObjectContext.Roles
                    where r.Id == id
                    select r).FirstOrDefault();
        if (data != null)
        {
          ctx.ObjectContext.Roles.DeleteObject(data);
          ctx.ObjectContext.SaveChanges();
        }
      }
    }
  }
}
