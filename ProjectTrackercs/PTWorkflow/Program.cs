#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;

#endregion

namespace PTWorkflow
{
  class Program
  {
    static void Main(string[] args)
    {
      using (WorkflowRuntime workflowRuntime = new WorkflowRuntime())
      {
        AutoResetEvent waitHandle = new AutoResetEvent(false);
        workflowRuntime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) { waitHandle.Set(); };
        workflowRuntime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e)
        {
          Console.WriteLine(e.Exception.Message);
          waitHandle.Set();
        };

        Dictionary<string,object> parameters = new Dictionary<string,object>();
        parameters.Add("ProjectId", new Guid("750a346c-6c08-48c8-9329-e306ce8c7299"));
        WorkflowInstance instance = 
          workflowRuntime.CreateWorkflow(typeof(PTWorkflow.ProjectWorkflow), parameters);
        instance.Start();

        waitHandle.WaitOne();
      }
    }
  }
}
