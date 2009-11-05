using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using ProjectTracker.Library;

namespace PTWfActivities
{
	public partial class GetProject: Activity
	{
		public GetProject()
		{
			InitializeComponent();
		}

    #region Dependency Properties

    private static DependencyProperty ProjectIdProperty = DependencyProperty.Register(
      "ProjectId", typeof(Guid), typeof(GetProject), null);

    public Guid ProjectId
    {
      get { return (Guid)base.GetValue(ProjectIdProperty); }
      set { base.SetValue(ProjectIdProperty, value); }
    }

    private static DependencyProperty ProjectProperty = DependencyProperty.Register(
      "Project", typeof(Project), typeof(GetProject), null);

    public Project Project
    {
      get { return (Project)base.GetValue(ProjectProperty); }
      set { base.SetValue(ProjectProperty, value); }
    }

    #endregion

    protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
    {
      this.Project = Project.GetProject(this.ProjectId);
      return base.Execute(executionContext);
    }
  }
}
