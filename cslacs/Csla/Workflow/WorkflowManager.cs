using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Workflow.Runtime;

namespace Csla.Workflow
{
  /// <summary>
  /// Manages execution of a WF workflow.
  /// </summary>
  public class WorkflowManager
  {
    ManualResetEvent _waitHandle = null;
    private Exception _workflowError = null;
    private WorkflowRuntime _workflowRuntime = null;
    private WorkflowInstance _instance = null;
    private WorkflowStatus _status = WorkflowStatus.Initializing;

    /// <summary>
    /// Gets the error exception returned
    /// from the workflow if the
    /// workflow terminated abnormally.
    /// </summary>
    public Exception Error
    {
      get
      {
        return _workflowError;
      }
    }

    /// <summary>
    /// Gets the current status of the workflow.
    /// </summary>
    public WorkflowStatus Status
    {
      get
      {
        return _status;
      }
    }

    /// <summary>
    /// Gets the workflow instance being
    /// executed.
    /// </summary>
    public WorkflowInstance WorkflowInstance
    {
      get
      {
        return _instance;
      }
    }

    /// <summary>
    /// Gets the workflow runtime instance
    /// that is executing the workflow.
    /// </summary>
    private WorkflowRuntime RuntimeInstance
    {
      get
      {
        return _workflowRuntime;
      }
    }

    /// <summary>
    /// Synchronously executes a workflow.
    /// </summary>
    /// <param name="typeName">
    /// Assembly qualified type name of the workflow.
    /// </param>
    public void ExecuteWorkflow(string typeName)
    {
      ExecuteWorkflow(typeName, null);
    }

    /// <summary>
    /// Synchronously executes a workflow.
    /// </summary>
    /// <param name="typeName">
    /// Assembly qualified type name of the workflow.
    /// </param>
    /// <param name="parameters">
    /// Name/value list of parameters to be passed
    /// to the workflow instance.
    /// </param>
    public void ExecuteWorkflow(string typeName, Dictionary<string, object> parameters)
    {
      BeginWorkflow(typeName, parameters);
      WaitForEnd(true);
    }

    /// <summary>
    /// Resumes synchronous execution of
    /// a workflow.
    /// </summary>
    /// <param name="instanceId">
    /// Id of the workflow instance to resume.
    /// </param>
    public void ResumeWorkflow(Guid instanceId)
    {
      BeginResumeWorkflow(instanceId);
      WaitForEnd(true);
    }

    /// <summary>
    /// Initializes the workflow runtime.
    /// </summary>
    public void InitializeRuntime()
    {
      if (_workflowRuntime != null)
        return;

      _waitHandle = new ManualResetEvent(false);

      // initialize workflow runtime
      _workflowRuntime = new WorkflowRuntime();
      _workflowRuntime.WorkflowCompleted +=
        delegate(object sender, WorkflowCompletedEventArgs e)
        {
          _status = WorkflowStatus.Completed;
          _waitHandle.Set();
        };
      _workflowRuntime.WorkflowTerminated +=
        delegate(object sender, WorkflowTerminatedEventArgs e)
        {
          _status = WorkflowStatus.Terminated;
          _workflowError = e.Exception;
          _waitHandle.Set();
        };
      _workflowRuntime.WorkflowSuspended +=
        delegate(object sender, WorkflowSuspendedEventArgs e)
        {
          _status = WorkflowStatus.Suspended;
          _waitHandle.Set();
        };
      _workflowRuntime.WorkflowAborted +=
        delegate(object sender, WorkflowEventArgs e)
        {
          _status = WorkflowStatus.Aborted;
          _waitHandle.Set();
        };
      _workflowRuntime.WorkflowIdled +=
        delegate(object sender, WorkflowEventArgs e)
        {
          _status = WorkflowStatus.Idled;
          _waitHandle.Set();
        };
    }

    /// <summary>
    /// Asynchronously starts executing workflow.
    /// </summary>
    /// <param name="typeName">
    /// Assembly qualified type name of the workflow.
    /// </param>
    public void BeginWorkflow(string typeName)
    {
      BeginWorkflow(typeName, null);
    }

    /// <summary>
    /// Asynchronously starts executing workflow.
    /// </summary>
    /// <param name="typeName">
    /// Assembly qualified type name of the workflow.
    /// </param>
    /// <param name="parameters">
    /// Name/value list of parameters to be passed
    /// to the workflow instance.
    /// </param>
    public void BeginWorkflow(string typeName, Dictionary<string, object> parameters)
    {
      InitializeRuntime();

      // create workflow instance
      Type workflowType = Type.GetType(typeName);
      if (parameters != null)
        _instance = _workflowRuntime.CreateWorkflow(
          workflowType,
          parameters);
      else
        _instance = _workflowRuntime.CreateWorkflow(
          workflowType);

      // execute workflow
      _instance.Start();
      _status = WorkflowStatus.Executing;
    }

    /// <summary>
    /// Resumes asynchronous execution of
    /// a workflow.
    /// </summary>
    /// <param name="instanceId">
    /// Id of the workflow instance to resume.
    /// </param>
    public void BeginResumeWorkflow(Guid instanceId)
    {
      InitializeRuntime();

      // get workflow instance
      _instance = _workflowRuntime.GetWorkflow(instanceId);

      // execute workflow
      _instance.Start();
      _status = WorkflowStatus.Executing;
    }

    /// <summary>
    /// Waits for the workflow to complete
    /// or terminate.
    /// </summary>
    public void WaitForEnd()
    {
      // wait for workflow to complete
      _waitHandle.WaitOne();
    }

    /// <summary>
    /// Waits for the workflow to complete
    /// or terminate.
    /// </summary>
    /// <param name="disposeRuntime">
    /// Value indicating whether to dispose
    /// the WF runtime when workflow
    /// completes.
    /// </param>
    public void WaitForEnd(bool disposeRuntime)
    {
      WaitForEnd();

      if (disposeRuntime)
        DisposeRuntime();
    }

    /// <summary>
    /// Diposes the workflow runtime.
    /// </summary>
    public void DisposeRuntime()
    {
      // dispose runtime
      _workflowRuntime.Dispose();
      _workflowRuntime = null;
    }
  }
}
