using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using ProjectTracker.Library;

namespace PTWorkflow
{
	public sealed partial class ProjectWorkflow: SequentialWorkflowActivity
	{
		public ProjectWorkflow()
		{
			InitializeComponent();
    }

    #region Dependency Properties

    private static DependencyProperty ProjectIdProperty =
      DependencyProperty.Register("ProjectId", typeof(Guid), typeof(ProjectWorkflow), null);

    public Guid ProjectId
    {
      get { return (Guid)base.GetValue(ProjectIdProperty); }
      set { base.SetValue(ProjectIdProperty, value); }
    }
	
    #endregion

    #region Code Activities

    private ProjectTracker.Library.Project _project;

    private void closeProject_ExecuteCode(object sender, EventArgs e)
    {
      _project = Project.GetProject(ProjectId);
      _project.Ended = DateTime.Today.ToString();
      _project = _project.Save();
    }

    private void notifyResources_ExecuteCode(object sender, EventArgs e)
    {
      foreach (ProjectResource resource in _project.Resources)
      {
        // notify each resource
      }
    }

    private void notifySponsor_ExecuteCode(object sender, EventArgs e)
    {
      // notify project sponsor
    }

    #endregion

  }

}
