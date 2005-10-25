Imports ProjectTracker.Library

Public Class MainForm

  Private mProject As Project

  Private Sub ProjectListDataGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProjectListDataGridView.SelectionChanged

    If Me.ProjectListDataGridView.SelectedRows.Count > 0 Then
      ' save last project
      SaveProject(mProject)

      ' get new project
      Dim id As Guid
      id = New Guid(Me.ProjectListDataGridView.SelectedRows(0).Cells(0).Value.ToString)
      mProject = ProjectTracker.Library.Project.GetProject(id)
      Me.ProjectBindingSource.DataSource = mProject
      Me.ResourcesBindingSource.DataSource = mProject.Resources
    End If

  End Sub

  Private Sub MainForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

    SaveProject(mProject)

  End Sub

  Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    ProjectTracker.Library.Security.PTPrincipal.Login("rocky", "lhotka")

    Me.RoleListBindingSource.DataSource = RoleList.GetList
    Me.ProjectListBindingSource.DataSource = ProjectList.GetProjectList

  End Sub

  Private Sub SaveProject(ByVal project As Project)

    If project IsNot Nothing Then
      project.ApplyEdit()
      project.Save()
    End If

  End Sub

End Class