Imports ProjectTracker.Library

Public Class ProjectWorkflow
  Inherits SequentialWorkflowActivity

#Region " Dependency Properties "

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

  Private Shared ReadOnly ProjectProperty As DependencyProperty = _
    DependencyProperty.Register("Project", GetType(Project), GetType(ProjectWorkflow), Nothing)

  Public Property Project() As Project
    Get
      Return CType(MyBase.GetValue(ProjectProperty), Project)
    End Get
    Set(ByVal value As Project)
      MyBase.SetValue(ProjectProperty, value)
    End Set
  End Property

#End Region

#Region " Code Activities "

  'Private _project As ProjectTracker.Library.Project

  Private Sub getProject_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    Me.Project = Project.GetProject(ProjectId)

  End Sub

  Private Sub closeProject_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    Me.Project.Ended = Today.ToString
    Me.Project = Me.Project.Save()

  End Sub

  Private Sub notifyResources_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    For Each resource As ProjectResource In Me.getProject1.Project.Resources
      ' notify each resource  
    Next

  End Sub

  Private Sub notifySponsor_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' notify project sponsor

  End Sub

#End Region

End Class
