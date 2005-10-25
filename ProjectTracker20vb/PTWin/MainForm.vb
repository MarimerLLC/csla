Imports ProjectTracker.Library

Public Class MainForm

  Private Sub ProjectListDataGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProjectListDataGridView.SelectionChanged

    If Me.ProjectListDataGridView.SelectedRows.Count > 0 Then
      Dim id As Guid
      id = New Guid(Me.ProjectListDataGridView.SelectedRows(0).Cells(0).Value.ToString)
      Dim project As Project = ProjectTracker.Library.Project.GetProject(id)
      Me.ProjectBindingSource.DataSource = project
      Me.ResourcesBindingSource.DataSource = project.Resources
    End If

  End Sub

  Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Me.ProjectListBindingSource.DataSource = ProjectList.GetProjectList

  End Sub

End Class