Public Class ProjectWorkflow
  Inherits SequentialWorkflowActivity

  Private _project As ProjectTracker.Library.Project

  Private Shared ReadOnly ProjectIdProperty As DependencyProperty = _
    DependencyProperty.Register("ProjectId", GetType(Guid), GetType(ProjectWorkflow), Nothing)

  Public Property ProjectId() As Guid
    Get
      Return CType(MyBase.GetValue(ProjectIdProperty), Guid)
    End Get
    Set(ByVal value As Guid)
      MyBase.SetValue(ProjectIdProperty, value)
    End Set
  End Property

  Private Sub login_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ProjectTracker.Library.Security.PTPrincipal.Login("pm", "pm")

  End Sub

  Private Sub getProject_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    'Dim list As ProjectTracker.Library.ProjectList = ProjectTracker.Library.ProjectList.GetProjectList
    _project = ProjectTracker.Library.Project.GetProject(ProjectId) 'list(0).Id)

  End Sub


  Private Sub closeProject_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    _project.Ended = Today.ToString

  End Sub


  Private Sub saveProject_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    _project.Save()

  End Sub

End Class
