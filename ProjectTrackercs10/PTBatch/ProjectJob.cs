using System;
using CSLA.BatchQueue;
using ProjectTracker.Library;

namespace PTBatch
{
  public class ProjectJob : IBatchEntry
  {
    void IBatchEntry.Execute(object state)
    {
      ProjectList projects = ProjectList.GetProjectList();
      Project project;

      foreach(ProjectList.ProjectInfo info in projects)
      {
        project = Project.GetProject(info.ID);
        project.Name = project.Name + " (batch)";
        project.Save();
      }
    }
  }
}
