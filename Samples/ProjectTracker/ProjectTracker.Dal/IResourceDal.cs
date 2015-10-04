using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IResourceDal
  {
    List<ResourceDto> Fetch();
    ResourceDto Fetch(int id);
    bool Exists(int id);
    void Insert(ResourceDto item);
    void Update(ResourceDto item);
    void Delete(int id);
  }
}
