<Serializable()> _
Public Class ClientProjectUpdater
  Inherits CommandBase

  Private _projectList As List(Of ClientProjectEdit)

#Region " Factory Methods "

  Public Shared Sub UpdateProjects(ByVal projectList As ClientProjectEdit())

    Dim cmd As New ClientProjectUpdater(New List(Of ClientProjectEdit)(projectList))
    cmd = DataPortal.Execute(cmd)

  End Sub

  Private Sub New()

  End Sub

  Private Sub New(ByVal projectList As List(Of ClientProjectEdit))

    _projectList = projectList

  End Sub

#End Region

#Region " Server-side Code "

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Execute()

    For Each item As ClientProjectEdit In _projectList
      item.Save()
    Next
    ' clear list
    _projectList = Nothing

  End Sub

#End Region

End Class
