using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IProjectDal
  {
    List<ProjectDto> Fetch();
    List<ProjectDto> Fetch(string nameFilter);
    ProjectDto Fetch(int id);
    bool Exists(int id);
    void Insert(ProjectDto item);
    void Update(ProjectDto item);
    void Delete(int id);
  }
}
