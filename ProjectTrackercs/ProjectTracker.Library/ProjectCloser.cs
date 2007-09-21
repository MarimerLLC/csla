using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Workflow;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectCloser : CommandBase
  {
    public static void CloseProject(Guid id)
    {
      ProjectCloser cmd = new ProjectCloser(id);
      cmd = DataPortal.Execute<ProjectCloser>(cmd);
    }

    private Guid _projectId;

    private ProjectCloser()
    { /* require use of factory methods */ }

    private ProjectCloser(Guid projectId)
    {
      _projectId = projectId;
    }

    protected override void DataPortal_Execute()
    {
      Dictionary<string, object> parameters = 
        new Dictionary<string, object>();
      parameters.Add("ProjectId", _projectId);

      WorkflowManager mgr = new WorkflowManager();
      mgr.ExecuteWorkflow(
        "PTWorkflow.ProjectWorkflow, PTWorkflow", 
        parameters);
      
      if (mgr.Status == WorkflowStatus.Terminated)
        throw mgr.Error;
    }
  }
}
