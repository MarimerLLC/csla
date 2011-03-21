using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IProjectDal
  {
    List<ProjectDto> Fetch();
    ProjectDto Fetch(int id);
    void Insert(ProjectDto item);
    void Update(ProjectDto item);
    void Delete(int id);
  }
}
