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
      this.saveProject = new System.Workflow.Activities.CodeActivity();
      this.closeProject = new System.Workflow.Activities.CodeActivity();
      this.getProject = new System.Workflow.Activities.CodeActivity();
      this.login = new System.Workflow.Activities.CodeActivity();
      // 
      // saveProject
      // 
      this.saveProject.Name = "saveProject";
      this.saveProject.ExecuteCode += new System.EventHandler(this.saveProject_ExecuteCode);
      // 
      // closeProject
      // 
      this.closeProject.Name = "closeProject";
      this.closeProject.ExecuteCode += new System.EventHandler(this.closeProject_ExecuteCode);
      // 
      // getProject
      // 
      this.getProject.Name = "getProject";
      this.getProject.ExecuteCode += new System.EventHandler(this.getProject_ExecuteCode);
      // 
      // login
      // 
      this.login.Name = "login";
      this.login.ExecuteCode += new System.EventHandler(this.login_ExecuteCode);
      // 
      // ProjectWorkflow
      // 
      this.Activities.Add(this.login);
      this.Activities.Add(this.getProject);
      this.Activities.Add(this.closeProject);
      this.Activities.Add(this.saveProject);
      this.Name = "ProjectWorkflow";
      this.CanModifyActivities = false;

		}

		#endregion

    private CodeActivity saveProject;
    private CodeActivity closeProject;
    private CodeActivity getProject;
    private CodeActivity login;



  }
}
