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

namespace PTWorkflow
{
  public partial class CloseProject : SequenceActivity
  {
    public CloseProject()
    {
      InitializeComponent();
    }

    private static DependencyProperty ProjectProperty = DependencyProperty.Register(
      "Project", typeof(Project), typeof(CloseProject), null);

    public Project Project
    {
      get { return (Project)base.GetValue(ProjectProperty); }
      set { base.SetValue(ProjectProperty, value); }
    }

    private void doClose_ExecuteCode(object sender, EventArgs e)
    {
      this.Project.Ended = DateTime.Today.ToString();
      this.Project = this.Project.Save();
    }
  }
}
