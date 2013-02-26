using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IRoleDal
  {
    List<RoleDto> Fetch();
    RoleDto Fetch(int id);
    void Insert(RoleDto item);
    void Update(RoleDto item);
    void Delete(int id);
  }
}
