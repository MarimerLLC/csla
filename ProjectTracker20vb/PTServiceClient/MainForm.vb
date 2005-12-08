Public Class MainForm

  Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Using svc As New PTService.PTService
      Me.ProjectInfoBindingSource.DataSource = svc.GetProjectList
      Me.ResourceInfoBindingSource.DataSource = svc.GetResourceList
      Me.RoleInfoBindingSource.DataSource = svc.GetRoles
    End Using

  End Sub

  Private Sub ProjectInfoDataGridView_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ProjectInfoDataGridView.SelectionChanged

    If Me.ProjectInfoDataGridView.SelectedRows.Count > 0 Then
      Dim projectId As String = Me.ProjectInfoDataGridView.SelectedRows(0).Cells(0).Value.ToString
      If projectId <> Me.IdLabel1.Text Then
        Using svc As New PTService.PTService
          Me.ProjectDetailBindingSource.DataSource = svc.GetProject(projectId)
        End Using
      End If
    End If

  End Sub

  Private Sub SaveProjectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveProjectButton.Click

    Using svc As New PTService.PTService
      SetCredentials(svc)
      Dim id As Guid = New Guid(Me.IdLabel1.Text)
      If Guid.Empty.Equals(id) Then
        'adding
        Me.ProjectDetailBindingSource.DataSource = _
          svc.AddProject( _
            Me.NameTextBox.Text, _
            Me.StartedTextBox.Text, _
            Me.EndedTextBox.Text, _
            Me.DescriptionTextBox.Text)

      Else
        ' updating
        Me.ProjectDetailBindingSource.DataSource = _
          svc.EditProject( _
            New Guid(Me.IdLabel1.Text), _
            Me.NameTextBox.Text, _
            Me.StartedTextBox.Text, _
            Me.EndedTextBox.Text, _
            Me.DescriptionTextBox.Text)
      End If
      ' refresh project list
      Me.ProjectInfoBindingSource.DataSource = svc.GetProjectList
    End Using

  End Sub

  Private Sub ClearProjectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearProjectButton.Click

    Me.ProjectDetailBindingSource.Clear()
    Me.ProjectDetailBindingSource.AddNew()

  End Sub

  Private Sub SetCredentials(ByVal svc As PTService.PTService)

    Dim credentials As New PTService.CslaCredentials
    credentials.Username = "rocky"
    credentials.Password = "lhotka"
    svc.CslaCredentialsValue = credentials

  End Sub

End Class
