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

#End Region

#Region " Code Activities "

  Private mProject As ProjectTracker.Library.Project

  Private Sub closeProject_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    mProject = Project.GetProject(ProjectId)
    mProject.Ended = Today.ToString
    mProject = mProject.Save()

  End Sub

  Private Sub notifyResources_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    For Each resource As ProjectResource In mProject.Resources
      ' notify each resource  
    Next

  End Sub

  Private Sub notifySponsor_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ' notify project sponsor

  End Sub

#End Region

End Class
