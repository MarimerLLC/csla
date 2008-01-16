using System;
using System.Collections.Generic;
using Csla;
using Csla.Security;
using Csla.Workflow;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectCloser : CommandBase
  {
    private Guid mProjectId;

    #region Authorization

    protected static void AddObjectAuthorizationRules()
    {
      // add object-level authorization rules here
      AuthorizationRules.AllowEdit(typeof(Project), "ProjectManager");
    }

    #endregion

    #region Factory Methods

    public static void CloseProject(Guid id)
    {
      ProjectCloser cmd = new ProjectCloser(id);
      cmd = DataPortal.Execute<ProjectCloser>(cmd);
    }

    private ProjectCloser()
    { /* require use of factory methods */ }

    private ProjectCloser(Guid id)
    {
      mProjectId = id;
    }

    #endregion

    #region Data Access

    protected override void DataPortal_Execute()
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("ProjectId", mProjectId);

      WorkflowManager mgr = new WorkflowManager();
      mgr.ExecuteWorkflow("PTWorkflow.ProjectWorkflow, PTWorkflow", parameters);

      if (mgr.Status == WorkflowStatus.Terminated)
        throw mgr.Error;
    }

    #endregion
  }
}