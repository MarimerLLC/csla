Imports ProjectTracker.Library

Public Class CloseProject
  Inherits SequenceActivity

  Private Shared ProjectProperty As DependencyProperty = _
    DependencyProperty.Register( _
    "Project", GetType(Project), GetType(CloseProject), Nothing)

  Public Property Project() As Project
    Get
      Return CType(MyBase.GetValue(ProjectProperty), Project)
    End Get
    Set(ByVal value As Project)
      MyBase.SetValue(ProjectProperty, value)
    End Set
  End Property

  Private Sub doClose_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    Me.Project.Ended = Today.ToString
    Me.Project = Me.Project.Save

  End Sub

End Class
