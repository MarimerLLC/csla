Namespace Workflow

  ''' <summary>
  ''' Status of the workflow.
  ''' </summary>
  Public Enum WorkflowStatus
    ''' <summary>
    ''' Workflow is being initialized.
    ''' </summary>
    Initializing
    ''' <summary>
    ''' Workflow is currently executing.
    ''' </summary>
    Executing
    ''' <summary>
    ''' Workflow has completed normally.
    ''' </summary>
    Completed
    ''' <summary>
    ''' Workflow has been idled.
    ''' </summary>
    Idled
    ''' <summary>
    ''' Workflow has terminated abnormally.
    ''' </summary>
    Terminated
    ''' <summary>
    ''' Workflow was aborted.
    ''' </summary>
    Aborted
    ''' <summary>
    ''' Workflow has been suspended.
    ''' </summary>
    Suspended
  End Enum

End Namespace
