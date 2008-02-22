Imports Csla.Workflow

<Serializable()> _
Public Class ProjectCloser
  Inherits CommandBase

  Public Shared Sub CloseProject(ByVal id As Guid)

    Dim cmd As New ProjectCloser(id)
    cmd = DataPortal.Execute(Of ProjectCloser)(cmd)

  End Sub

  Private _projectId As Guid

  Private Sub New()
    ' require use of factory methods
  End Sub

  Private Sub New(ByVal id As Guid)
    _projectId = id
  End Sub

  Protected Overrides Sub DataPortal_Execute()

    Dim parameters As New Dictionary(Of String, Object)
    parameters.Add("ProjectId", _projectId)

    Dim mgr As New WorkflowManager
    mgr.ExecuteWorkflow( _
        "PTWorkflow.ProjectWorkflow, PTWorkflow", _
        parameters)

    If mgr.Status = WorkflowStatus.Terminated Then
      Throw mgr.Error
    End If

  End Sub

End Class
