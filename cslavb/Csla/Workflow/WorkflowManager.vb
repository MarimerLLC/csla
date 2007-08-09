#If (Not NET20) Then

Imports System.Threading
Imports System.Workflow.Runtime

Namespace Workflow

  ''' <summary>
  ''' Manages execution of a WF workflow.
  ''' </summary>
  Public Class WorkflowManager

    Private mWaitHandle As ManualResetEvent = Nothing
    Private mWorkflowError As Exception = Nothing
    Private mWorkflowRuntime As WorkflowRuntime = Nothing
    Private mInstance As WorkflowInstance = Nothing
    Private mStatus As WorkflowStatus = WorkflowStatus.Initializing

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="workflowRuntime">
    ''' A workflow runtime instance to use
    ''' for all operations.
    ''' </param>
    Public Sub New(ByVal workflowRuntime As WorkflowRuntime)
      mWorkflowRuntime = workflowRuntime
    End Sub

    ''' <summary>
    ''' Gets the error exception returned
    ''' from the workflow if the
    ''' workflow terminated abnormally.
    ''' </summary>
    Public ReadOnly Property [Error]() As Exception
      Get
        Return mWorkflowError
      End Get
    End Property

    ''' <summary>
    ''' Gets the current status of the workflow.
    ''' </summary>
    Public ReadOnly Property Status() As WorkflowStatus
      Get
        Return mStatus
      End Get
    End Property

    ''' <summary>
    ''' Gets the workflow instance being
    ''' executed.
    ''' </summary>
    Public ReadOnly Property WorkflowInstance() As WorkflowInstance
      Get
        Return mInstance
      End Get
    End Property

    ''' <summary>
    ''' Gets the workflow runtime instance
    ''' that is executing the workflow.
    ''' </summary>
    Public ReadOnly Property RuntimeInstance() As WorkflowRuntime
      Get
        Return mWorkflowRuntime
      End Get
    End Property

    ''' <summary>
    ''' Synchronously executes a workflow.
    ''' </summary>
    ''' <param name="typeName">
    ''' Assembly qualified type name of the workflow.
    ''' </param>
    Public Sub ExecuteWorkflow(ByVal typeName As String)
      Dim workflowType As Type = Type.GetType(typeName)
      ExecuteWorkflow(workflowType)
    End Sub

    ''' <summary>
    ''' Synchronously executes a workflow.
    ''' </summary>
    ''' <param name="workflowType">
    ''' Type object referencing the workflow.
    ''' </param>
    Public Sub ExecuteWorkflow(ByVal workflowType As Type)
      ExecuteWorkflow(workflowType, Nothing, True)
    End Sub

    ''' <summary>
    ''' Synchronously executes a workflow.
    ''' </summary>
    ''' <param name="typeName">
    ''' Assembly qualified type name of the workflow.
    ''' </param>
    ''' <param name="disposeRuntime">
    ''' Value indicating whether to dispose
    ''' the WF runtime when workflow
    ''' completes.
    ''' </param>
    Public Sub ExecuteWorkflow(ByVal typeName As String, ByVal disposeRuntime As Boolean)
      Dim workflowType As Type = Type.GetType(typeName)
      ExecuteWorkflow(workflowType, disposeRuntime)
    End Sub

    ''' <summary>
    ''' Synchronously executes a workflow.
    ''' </summary>
    ''' <param name="workflowType">
    ''' Type object referencing the workflow.
    ''' </param>
    ''' <param name="disposeRuntime">
    ''' Value indicating whether to dispose
    ''' the WF runtime when workflow
    ''' completes.
    ''' </param>
    Public Sub ExecuteWorkflow(ByVal workflowType As Type, ByVal disposeRuntime As Boolean)
      ExecuteWorkflow(workflowType, Nothing, disposeRuntime)
    End Sub

    ''' <summary>
    ''' Synchronously executes a workflow.
    ''' </summary>
    ''' <param name="typeName">
    ''' Assembly qualified type name of the workflow.
    ''' </param>
    ''' <param name="parameters">
    ''' Name/value list of parameters to be passed
    ''' to the workflow instance.
    ''' </param>
    Public Sub ExecuteWorkflow(ByVal typeName As String, ByVal parameters As Dictionary(Of String, Object))
      Dim workflowType As Type = Type.GetType(typeName)
      ExecuteWorkflow(workflowType, parameters, True)
    End Sub

    ''' <summary>
    ''' Synchronously executes a workflow.
    ''' </summary>
    ''' <param name="typeName">
    ''' Assembly qualified type name of the workflow.
    ''' </param>
    ''' <param name="parameters">
    ''' Name/value list of parameters to be passed
    ''' to the workflow instance.
    ''' </param>
    ''' <param name="disposeRuntime">
    ''' Value indicating whether to dispose
    ''' the WF runtime when workflow
    ''' completes.
    ''' </param>
    Public Sub ExecuteWorkflow(ByVal typeName As String, ByVal parameters As Dictionary(Of String, Object), ByVal disposeRuntime As Boolean)
      Dim workflowType As Type = Type.GetType(typeName)
      ExecuteWorkflow(workflowType, parameters, disposeRuntime)
    End Sub

    ''' <summary>
    ''' Synchronously executes a workflow.
    ''' </summary>
    ''' <param name="workflowType">
    ''' Type object referencing the workflow.
    ''' </param>
    ''' <param name="parameters">
    ''' Name/value list of parameters to be passed
    ''' to the workflow instance.
    ''' </param>
    Public Sub ExecuteWorkflow(ByVal workflowType As Type, ByVal parameters As Dictionary(Of String, Object))
      ExecuteWorkflow(workflowType, parameters, True)
    End Sub

    ''' <summary>
    ''' Synchronously executes a workflow.
    ''' </summary>
    ''' <param name="workflowType">
    ''' Type object referencing the workflow.
    ''' </param>
    ''' <param name="parameters">
    ''' Name/value list of parameters to be passed
    ''' to the workflow instance.
    ''' </param>
    ''' <param name="disposeRuntime">
    ''' Value indicating whether to dispose
    ''' the WF runtime when workflow
    ''' completes.
    ''' </param>
    Public Sub ExecuteWorkflow(ByVal workflowType As Type, ByVal parameters As Dictionary(Of String, Object), ByVal disposeRuntime As Boolean)
      BeginWorkflow(workflowType, parameters)
      WaitForEnd(disposeRuntime)
    End Sub

    ''' <summary>
    ''' Resumes synchronous execution of
    ''' the workflow.
    ''' </summary>
    Public Sub ResumeWorkflow()
      ResumeWorkflow(True)
    End Sub

    ''' <summary>
    ''' Resumes synchronous execution of
    ''' the workflow.
    ''' </summary>
    ''' <param name="disposeRuntime">
    ''' Value indicating whether to dispose
    ''' the WF runtime when workflow
    ''' completes.
    ''' </param>
    Public Sub ResumeWorkflow(ByVal disposeRuntime As Boolean)
      BeginResumeWorkflow()
      WaitForEnd(disposeRuntime)
    End Sub

    ''' <summary>
    ''' Resumes synchronous execution of
    ''' a workflow.
    ''' </summary>
    ''' <param name="instanceId">
    ''' Id of the workflow instance to resume.
    ''' </param>
    Public Sub ResumeWorkflow(ByVal instanceId As Guid)
      ResumeWorkflow(instanceId, True)
    End Sub

    ''' <summary>
    ''' Resumes synchronous execution of
    ''' a workflow.
    ''' </summary>
    ''' <param name="instanceId">
    ''' Id of the workflow instance to resume.
    ''' </param>
    ''' <param name="disposeRuntime">
    ''' Value indicating whether to dispose
    ''' the WF runtime when workflow
    ''' completes.
    ''' </param>
    Public Sub ResumeWorkflow(ByVal instanceId As Guid, ByVal disposeRuntime As Boolean)
      BeginResumeWorkflow(instanceId)
      WaitForEnd(disposeRuntime)
    End Sub

    ''' <summary>
    ''' Initializes the workflow runtime.
    ''' </summary>
    Public Sub InitializeRuntime()
      If Not mWorkflowRuntime Is Nothing Then
        Return
      End If

      mWaitHandle = New ManualResetEvent(False)

      ' initialize workflow runtime
      mWorkflowRuntime = New WorkflowRuntime()
      AddHandler mWorkflowRuntime.WorkflowCompleted, AddressOf OnWorkflowCompleted
      AddHandler mWorkflowRuntime.WorkflowTerminated, AddressOf OnWorkflowTerminated
      AddHandler mWorkflowRuntime.WorkflowSuspended, AddressOf OnWorkflowSuspended
      AddHandler mWorkflowRuntime.WorkflowAborted, AddressOf OnWorkflowAborted
      AddHandler mWorkflowRuntime.WorkflowIdled, AddressOf OnWorkflowIdled
    End Sub

    Private Sub OnWorkflowCompleted(ByVal sender As Object, ByVal e As WorkflowCompletedEventArgs)
      mStatus = WorkflowStatus.Completed
      mWaitHandle.Set()
    End Sub

    Private Sub OnWorkflowTerminated(ByVal sender As Object, ByVal e As WorkflowTerminatedEventArgs)
      mStatus = WorkflowStatus.Terminated
      mWorkflowError = e.Exception
      mWaitHandle.Set()
    End Sub

    Private Sub OnWorkflowSuspended(ByVal sender As Object, ByVal e As WorkflowSuspendedEventArgs)
      mStatus = WorkflowStatus.Suspended
      mWaitHandle.Set()
    End Sub

    Private Sub OnWorkflowAborted(ByVal sender As Object, ByVal e As WorkflowEventArgs)
      mStatus = WorkflowStatus.Aborted
      mWaitHandle.Set()
    End Sub

    Private Sub OnWorkflowIdled(ByVal sender As Object, ByVal e As WorkflowEventArgs)
      mStatus = WorkflowStatus.Idled
      mWaitHandle.Set()
    End Sub

    ''' <summary>
    ''' Asynchronously starts executing workflow.
    ''' </summary>
    ''' <param name="typeName">
    ''' Assembly qualified type name of the workflow.
    ''' </param>
    Public Sub BeginWorkflow(ByVal typeName As String)
      Dim workflowType As Type = Type.GetType(typeName)
      BeginWorkflow(workflowType, Nothing)
    End Sub

    ''' <summary>
    ''' Asynchronously starts executing workflow.
    ''' </summary>
    ''' <param name="workflowType">
    ''' Type object referencing the workflow.
    ''' </param>
    Public Sub BeginWorkflow(ByVal workflowType As Type)
      BeginWorkflow(workflowType, Nothing)
    End Sub

    ''' <summary>
    ''' Asynchronously starts executing workflow.
    ''' </summary>
    ''' <param name="typeName">
    ''' Assembly qualified type name of the workflow.
    ''' </param>
    ''' <param name="parameters">
    ''' Name/value list of parameters to be passed
    ''' to the workflow instance.
    ''' </param>
    Public Sub BeginWorkflow(ByVal typeName As String, ByVal parameters As Dictionary(Of String, Object))
      Dim workflowType As Type = Type.GetType(typeName)
      BeginWorkflow(workflowType, parameters)
    End Sub

    ''' <summary>
    ''' Asynchronously starts executing workflow.
    ''' </summary>
    ''' <param name="workflowType">
    ''' Type object referencing the workflow.
    ''' </param>
    ''' <param name="parameters">
    ''' Name/value list of parameters to be passed
    ''' to the workflow instance.
    ''' </param>
    Public Sub BeginWorkflow(ByVal workflowType As Type, ByVal parameters As Dictionary(Of String, Object))
      InitializeRuntime()

      If (Not mWorkflowRuntime.IsStarted) Then
        mWorkflowRuntime.StartRuntime()
      End If

      ' create workflow instance
      If Not parameters Is Nothing Then
        mInstance = mWorkflowRuntime.CreateWorkflow(workflowType, parameters)
      Else
        mInstance = mWorkflowRuntime.CreateWorkflow(workflowType)
      End If

      ' execute workflow
      mInstance.Start()
      mStatus = WorkflowStatus.Executing
    End Sub

    ''' <summary>
    ''' Loads a workflow from persisted storage
    ''' and resumes asynchronous execution of
    ''' that workflow.
    ''' </summary>
    ''' <param name="instanceId">
    ''' Id of the workflow instance to load and resume.
    ''' </param>
    Public Sub BeginResumeWorkflow(ByVal instanceId As Guid)
      InitializeRuntime()

      If (Not mWorkflowRuntime.IsStarted) Then
        mWorkflowRuntime.StartRuntime()
      End If

      ' get workflow instance
      mInstance = mWorkflowRuntime.GetWorkflow(instanceId)

      BeginResumeWorkflow()
    End Sub

    ''' <summary>
    ''' Resumes asynchronous execution of
    ''' the current workflow.
    ''' </summary>
    Public Sub BeginResumeWorkflow()
      InitializeRuntime()

      If (Not mWorkflowRuntime.IsStarted) Then
        mWorkflowRuntime.StartRuntime()
      End If

      ' execute workflow
      mInstance.Resume()
      mStatus = WorkflowStatus.Executing
    End Sub

    ''' <summary>
    ''' Waits for the workflow to complete
    ''' or terminate.
    ''' </summary>
    Public Sub WaitForEnd()
      ' wait for workflow to complete
      mWaitHandle.WaitOne()
    End Sub

    ''' <summary>
    ''' Waits for the workflow to complete
    ''' or terminate.
    ''' </summary>
    ''' <param name="disposeRuntime">
    ''' Value indicating whether to dispose
    ''' the WF runtime when workflow
    ''' completes.
    ''' </param>
    Public Sub WaitForEnd(ByVal disposeRuntime As Boolean)
      WaitForEnd()

      If disposeRuntime Then
        Me.DisposeRuntime()
      End If
    End Sub

    ''' <summary>
    ''' Diposes the workflow runtime.
    ''' </summary>
    Public Sub DisposeRuntime()
      ' dispose runtime
      If Not mWorkflowRuntime Is Nothing Then
        mWorkflowRuntime.Dispose()
      End If
      mWorkflowRuntime = Nothing
      If Not mWaitHandle Is Nothing Then
        mWaitHandle.Close()
      End If
      mWaitHandle = Nothing
      mInstance = Nothing
    End Sub

  End Class

End Namespace

#End If