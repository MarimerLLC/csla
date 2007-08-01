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

    private static DependencyProperty ProjectIdProperty = DependencyProperty.Register(
      "ProjectId", typeof(Guid), typeof(ProjectWorkflow), null);

    public Guid ProjectId
    {
      get { return (Guid)base.GetValue(ProjectIdProperty); }
      set { base.SetValue(ProjectIdProperty, value); }
    }

    //private static DependencyProperty ProjectProperty =
    //  DependencyProperty.Register("Project", typeof(Project), typeof(ProjectWorkflow), null);

    //public Project Project
    //{
    //  get { return (Project)base.GetValue(ProjectProperty); }
    //  set { base.SetValue(ProjectProperty, value); }
    //}
	
    #endregion

    #region Code Activities
    [System.Runtime.Serialization.OptionalField(VersionAdded=2)]
    private void getProject_ExecuteCode(object sender, EventArgs e)
    {
      //this.Project = Project.GetProject(ProjectId);
    }

    private void closeProject_ExecuteCode(object sender, EventArgs e)
    {
      //this.Project.Ended = DateTime.Today.ToString();
      //this.Project = this.Project.Save();
    }

    private void notifyResources_ExecuteCode(object sender, EventArgs e)
    {
      foreach (ProjectResource resource in this.getProject1.Project.Resources)
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
