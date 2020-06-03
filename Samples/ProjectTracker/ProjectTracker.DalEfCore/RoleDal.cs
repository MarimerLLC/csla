using System.Collections.Generic;
using System.Linq;
using ProjectTracker.Dal;

namespace ProjectTracker.DalEfCore
{
  public class RoleDal : IRoleDal
  {
    private readonly PTrackerContext db;

    public RoleDal(PTrackerContext context)
    {
      db = context;
    }

    public List<RoleDto> Fetch()
    {
      var result = from r in db.Roles
                   select new RoleDto { Id = r.Id, Name = r.Name, LastChanged = r.LastChanged };
      return result.ToList();
    }

    public RoleDto Fetch(int id)
    {
      var result = (from r in db.Roles
                    where r.Id == id
                    select new RoleDto { Id = r.Id, Name = r.Name, LastChanged = r.LastChanged }).FirstOrDefault();
      if (result == null)
        throw new DataNotFoundException("Role");
      return result;
    }

    public void Insert(RoleDto item)
    {
      var newItem = new Role
      {
        Name = item.Name
      };
      db.Roles.Add(newItem);
      db.SaveChanges();
      item.Id = newItem.Id;
      item.LastChanged = newItem.LastChanged;
    }

    public void Update(RoleDto item)
    {
      var data = (from r in db.Roles
                  where r.Id == item.Id
                  select r).FirstOrDefault();
      if (data == null)
        throw new DataNotFoundException("Role");
      if (!data.LastChanged.Matches(item.LastChanged))
        throw new ConcurrencyException("Role");

      data.Name = item.Name;
      var count = db.SaveChanges();
      if (count == 0)
        throw new UpdateFailureException("Role");
      item.LastChanged = data.LastChanged;
    }

    public void Delete(int id)
    {
      var data = (from r in db.Roles
                  where r.Id == id
                  select r).FirstOrDefault();
      if (data != null)
      {
        db.Roles.Remove(data);
        db.SaveChanges();
      }
    }
  }
}
