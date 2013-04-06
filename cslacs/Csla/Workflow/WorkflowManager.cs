#if !CLIENTONLY
using System;
using System.Collections.Generic;
using System.Threading;
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
    /// Creates an instance of the object.
    /// </summary>
    public WorkflowManager()
    {
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="workflowRuntime">
    /// A workflow runtime instance to use
    /// for all operations.
    /// </param>
    public WorkflowManager(WorkflowRuntime workflowRuntime)
    {
      _workflowRuntime = workflowRuntime;
    }

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
    public WorkflowRuntime RuntimeInstance
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
      Type workflowType = Type.GetType(typeName);
      ExecuteWorkflow(workflowType);
    }

    /// <summary>
    /// Synchronously executes a workflow.
    /// </summary>
    /// <param name="workflowType">
    /// Type object referencing the workflow.
    /// </param>
    public void ExecuteWorkflow(Type workflowType)
    {
      ExecuteWorkflow(workflowType, null, true);
    }

    /// <summary>
    /// Synchronously executes a workflow.
    /// </summary>
    /// <param name="typeName">
    /// Assembly qualified type name of the workflow.
    /// </param>
    /// <param name="disposeRuntime">
    /// Value indicating whether to dispose
    /// the WF runtime when workflow
    /// completes.
    /// </param>
    public void ExecuteWorkflow(string typeName, bool disposeRuntime)
    {
      Type workflowType = Type.GetType(typeName);
      ExecuteWorkflow(workflowType, disposeRuntime);
    }

    /// <summary>
    /// Synchronously executes a workflow.
    /// </summary>
    /// <param name="workflowType">
    /// Type object referencing the workflow.
    /// </param>
    /// <param name="disposeRuntime">
    /// Value indicating whether to dispose
    /// the WF runtime when workflow
    /// completes.
    /// </param>
    public void ExecuteWorkflow(Type workflowType, bool disposeRuntime)
    {
      ExecuteWorkflow(workflowType, null, disposeRuntime);
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
      Type workflowType = Type.GetType(typeName);
      ExecuteWorkflow(workflowType, parameters, true);
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
    /// <param name="disposeRuntime">
    /// Value indicating whether to dispose
    /// the WF runtime when workflow
    /// completes.
    /// </param>
    public void ExecuteWorkflow(string typeName, Dictionary<string, object> parameters, bool disposeRuntime)
    {
      Type workflowType = Type.GetType(typeName);
      ExecuteWorkflow(workflowType, parameters, disposeRuntime);
    }

    /// <summary>
    /// Synchronously executes a workflow.
    /// </summary>
    /// <param name="workflowType">
    /// Type object referencing the workflow.
    /// </param>
    /// <param name="parameters">
    /// Name/value list of parameters to be passed
    /// to the workflow instance.
    /// </param>
    public void ExecuteWorkflow(Type workflowType, Dictionary<string, object> parameters)
    {
      ExecuteWorkflow(workflowType, parameters, true);
    }

    /// <summary>
    /// Synchronously executes a workflow.
    /// </summary>
    /// <param name="workflowType">
    /// Type object referencing the workflow.
    /// </param>
    /// <param name="parameters">
    /// Name/value list of parameters to be passed
    /// to the workflow instance.
    /// </param>
    /// <param name="disposeRuntime">
    /// Value indicating whether to dispose
    /// the WF runtime when workflow
    /// completes.
    /// </param>
    public void ExecuteWorkflow(Type workflowType, Dictionary<string, object> parameters, bool disposeRuntime)
    {
      BeginWorkflow(workflowType, parameters);
      WaitForEnd(disposeRuntime);
    }

    /// <summary>
    /// Resumes synchronous execution of
    /// the workflow.
    /// </summary>
    public void ResumeWorkflow()
    {
      ResumeWorkflow(true);
    }

    /// <summary>
    /// Resumes synchronous execution of
    /// the workflow.
    /// </summary>
    /// <param name="disposeRuntime">
    /// Value indicating whether to dispose
    /// the WF runtime when workflow
    /// completes.
    /// </param>
    public void ResumeWorkflow(bool disposeRuntime)
    {
      BeginResumeWorkflow();
      WaitForEnd(disposeRuntime);
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
      ResumeWorkflow(instanceId, true);
    }

    /// <summary>
    /// Resumes synchronous execution of
    /// a workflow.
    /// </summary>
    /// <param name="instanceId">
    /// Id of the workflow instance to resume.
    /// </param>
    /// <param name="disposeRuntime">
    /// Value indicating whether to dispose
    /// the WF runtime when workflow
    /// completes.
    /// </param>
    public void ResumeWorkflow(Guid instanceId, bool disposeRuntime)
    {
      BeginResumeWorkflow(instanceId);
      WaitForEnd(disposeRuntime);
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
      Type workflowType = Type.GetType(typeName);
      BeginWorkflow(workflowType, null);
    }

    /// <summary>
    /// Asynchronously starts executing workflow.
    /// </summary>
    /// <param name="workflowType">
    /// Type object referencing the workflow.
    /// </param>
    public void BeginWorkflow(Type workflowType)
    {
      BeginWorkflow(workflowType, null);
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
      Type workflowType = Type.GetType(typeName);
      BeginWorkflow(workflowType, parameters);
    }

    /// <summary>
    /// Asynchronously starts executing workflow.
    /// </summary>
    /// <param name="workflowType">
    /// Type object referencing the workflow.
    /// </param>
    /// <param name="parameters">
    /// Name/value list of parameters to be passed
    /// to the workflow instance.
    /// </param>
    public void BeginWorkflow(Type workflowType, Dictionary<string, object> parameters)
    {
      InitializeRuntime();

      if (!_workflowRuntime.IsStarted)
        _workflowRuntime.StartRuntime();

      // create workflow instance
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
    /// Loads a workflow from persisted storage
    /// and resumes asynchronous execution of
    /// that workflow.
    /// </summary>
    /// <param name="instanceId">
    /// Id of the workflow instance to load and resume.
    /// </param>
    public void BeginResumeWorkflow(Guid instanceId)
    {
      InitializeRuntime();

      if (!_workflowRuntime.IsStarted)
        _workflowRuntime.StartRuntime();

      // get workflow instance
      _instance = _workflowRuntime.GetWorkflow(instanceId);

      BeginResumeWorkflow();
    }

    /// <summary>
    /// Resumes asynchronous execution of
    /// the current workflow.
    /// </summary>
    public void BeginResumeWorkflow()
    {
      InitializeRuntime();

      if (!_workflowRuntime.IsStarted)
        _workflowRuntime.StartRuntime();

      // execute workflow
      _instance.Resume();
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
      if (_workflowRuntime != null) 
        _workflowRuntime.Dispose();
      _workflowRuntime = null;
      if (_waitHandle != null)
        _waitHandle.Close();
      _waitHandle = null;
      _instance = null;
    }
  }
}
#endif