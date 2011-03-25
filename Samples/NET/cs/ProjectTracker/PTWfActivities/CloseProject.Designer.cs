using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace PTWorkflow
{
	public partial class CloseProject
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
      this.doClose = new System.Workflow.Activities.CodeActivity();
      // 
      // doClose
      // 
      this.doClose.Name = "doClose";
      this.doClose.ExecuteCode += new System.EventHandler(this.doClose_ExecuteCode);
      // 
      // CloseProject
      // 
      this.Activities.Add(this.doClose);
      this.Name = "CloseProject";
      this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity doClose;
	}
}
