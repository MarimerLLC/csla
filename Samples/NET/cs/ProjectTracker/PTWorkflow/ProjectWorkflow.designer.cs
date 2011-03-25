using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace PTWorkflow
{
	partial class ProjectWorkflow
	{
		#region Designer generated code
		
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
		private void InitializeComponent()
		{
      this.CanModifyActivities = true;
      System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
      System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
      this.notifySponsor = new System.Workflow.Activities.CodeActivity();
      this.notifyResources = new System.Workflow.Activities.CodeActivity();
      this.sequenceActivity2 = new System.Workflow.Activities.SequenceActivity();
      this.sequenceActivity1 = new System.Workflow.Activities.SequenceActivity();
      this.parallelActivity1 = new System.Workflow.Activities.ParallelActivity();
      this.closeProject1 = new PTWorkflow.CloseProject();
      this.getProject1 = new PTWfActivities.GetProject();
      // 
      // notifySponsor
      // 
      this.notifySponsor.Name = "notifySponsor";
      this.notifySponsor.ExecuteCode += new System.EventHandler(this.notifySponsor_ExecuteCode);
      // 
      // notifyResources
      // 
      this.notifyResources.Name = "notifyResources";
      this.notifyResources.ExecuteCode += new System.EventHandler(this.notifyResources_ExecuteCode);
      // 
      // sequenceActivity2
      // 
      this.sequenceActivity2.Activities.Add(this.notifySponsor);
      this.sequenceActivity2.Name = "sequenceActivity2";
      // 
      // sequenceActivity1
      // 
      this.sequenceActivity1.Activities.Add(this.notifyResources);
      this.sequenceActivity1.Name = "sequenceActivity1";
      // 
      // parallelActivity1
      // 
      this.parallelActivity1.Activities.Add(this.sequenceActivity1);
      this.parallelActivity1.Activities.Add(this.sequenceActivity2);
      this.parallelActivity1.Name = "parallelActivity1";
      // 
      // closeProject1
      // 
      this.closeProject1.Name = "closeProject1";
      activitybind1.Name = "getProject1";
      activitybind1.Path = "Project";
      this.closeProject1.SetBinding(System.Workflow.ComponentModel.DependencyProperty.FromName("Project", typeof(PTWorkflow.CloseProject)), ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
      // 
      // getProject1
      // 
      this.getProject1.Name = "getProject1";
      this.getProject1.Project = null;
      activitybind2.Name = "ProjectWorkflow";
      activitybind2.Path = "ProjectId";
      this.getProject1.SetBinding(System.Workflow.ComponentModel.DependencyProperty.FromName("ProjectId", typeof(PTWfActivities.GetProject)), ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
      // 
      // ProjectWorkflow
      // 
      this.Activities.Add(this.getProject1);
      this.Activities.Add(this.closeProject1);
      this.Activities.Add(this.parallelActivity1);
      this.Name = "ProjectWorkflow";
      this.CanModifyActivities = false;

		}

		#endregion

        private PTWfActivities.GetProject getProject1;
        private CloseProject closeProject1;
        private CodeActivity notifySponsor;
        private CodeActivity notifyResources;
        private SequenceActivity sequenceActivity2;
        private SequenceActivity sequenceActivity1;
        private ParallelActivity parallelActivity1;














  }
}
