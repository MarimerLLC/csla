using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.Dal;
using ProjectTracker.DalMock.MockDbTypes;

namespace ProjectTracker.DalMock
{
  public class RoleDal : IRoleDal
  {
    public List<RoleDto> Fetch()
    {
      var result = from r in MockDb.Roles
                   select new RoleDto { Id = r.Id, Name = r.Name, LastChanged = r.LastChanged };
      return result.ToList();
    }

    public RoleDto Fetch(int id)
    {
      var result = (from r in MockDb.Roles
                    where r.Id == id
                    select new RoleDto { Id = r.Id, Name = r.Name, LastChanged = r.LastChanged }).FirstOrDefault();
      if (result == null)
        throw new DataNotFoundException("Role");
      return result;
    }

    public void Insert(RoleDto item)
    {
      item.Id = MockDb.Roles.Max(c => c.Id) + 1;
      var newItem = new RoleData { Id = item.Id, Name = item.Name, LastChanged = MockDb.GetTimeStamp() };
      MockDb.Roles.Add(newItem);
    }

    public void Update(RoleDto item)
    {
      var data = (from r in MockDb.Roles
                 where r.Id == item.Id
                 select r).FirstOrDefault();
      if (data == null)
        throw new DataNotFoundException("Role");
      data.Name = item.Name;
      data.LastChanged = MockDb.GetTimeStamp();
    }

    public void Delete(int id)
    {
      var data = (from r in MockDb.Roles
                  where r.Id == id
                  select r).FirstOrDefault();
      if (data != null)
        MockDb.Roles.Remove(data);
    }
  }
}
