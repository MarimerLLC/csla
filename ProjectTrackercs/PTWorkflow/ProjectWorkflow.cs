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

    private ProjectTracker.Library.Project _project;

    private void login_ExecuteCode(object sender, EventArgs e)
    {
      ProjectTracker.Library.Security.PTPrincipal.Login("pm", "pm");
    }

    private void getProject_ExecuteCode(object sender, EventArgs e)
    {
      _project = ProjectTracker.Library.Project.GetProject(ProjectId);
    }

    private void closeProject_ExecuteCode(object sender, EventArgs e)
    {
      _project.Ended = DateTime.Today.ToString();
    }

    private void saveProject_ExecuteCode(object sender, EventArgs e)
    {
      _project.Save();
    }
	}

}
