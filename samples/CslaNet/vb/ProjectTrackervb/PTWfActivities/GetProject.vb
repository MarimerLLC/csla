Imports ProjectTracker.Library

Public Class GetProject
  Inherits Activity

#Region " Dependency Properties "

  Private Shared ProjectIdProperty As DependencyProperty = _
    DependencyProperty.Register( _
    "ProjectId", GetType(Guid), GetType(GetProject), Nothing)

  Public Property ProjectId() As Guid
    Get
      Return CType(MyBase.GetValue(ProjectIdProperty), Guid)
    End Get
    Set(ByVal value As Guid)
      MyBase.SetValue(ProjectIdProperty, value)
    End Set
  End Property

  Private Shared ProjectProperty As DependencyProperty = _
    DependencyProperty.Register( _
    "Project", GetType(Project), GetType(GetProject), Nothing)

  Public Property Project() As Project
    Get
      Return CType(MyBase.GetValue(ProjectProperty), Project)
    End Get
    Set(ByVal value As Project)
      MyBase.SetValue(ProjectProperty, value)
    End Set
  End Property

#End Region

  Protected Overrides Function Execute( _
    ByVal executionContext As ActivityExecutionContext) As ActivityExecutionStatus

    Me.Project = Project.GetProject(Me.ProjectId)
    Return MyBase.Execute(executionContext)

  End Function

End Class
