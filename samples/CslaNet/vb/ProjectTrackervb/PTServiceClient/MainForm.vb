Public Class MainForm

  Private Sub MainForm_Load( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load

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
          Dim request As New PTService.ProjectRequest
          request.Id = New Guid(projectId)
          Me.ProjectDetailBindingSource.DataSource = svc.GetProject(request)
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

  Private Sub AssignToProjectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AssignToProjectButton.Click

    If Len(Me.ResourceIdLabel.Text.Trim) = 0 Then
      MessageBox.Show("You must select a resource first", "Assign resource", _
        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End If
    If Len(Me.ProjectIdLabel.Text.Trim) = 0 OrElse Guid.Empty.Equals(New Guid(Me.ProjectIdLabel.Text)) Then
      MessageBox.Show("You must select a project first", "Assign resource", _
        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End If
    Using svc As New PTService.PTService
      SetCredentials(svc)
      Try
        ' do the assignment
        svc.AssignResource( _
          CInt(Me.ResourceIdLabel.Text), New Guid(Me.ProjectIdLabel.Text))
        ' refresh the detail view
        Dim request As New PTService.ProjectRequest
        request.Id = New Guid(Me.ProjectIdLabel.Text)
        Me.ProjectDetailBindingSource.DataSource = svc.GetProject(request)

      Catch ex As Exception
        MessageBox.Show(ex.Message, "Assign resource", _
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End Try
    End Using

  End Sub

  Private Sub ResourceInfoDataGridView_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ResourceInfoDataGridView.CellDoubleClick

    Dim dlg As New ResourceName(Me.ResourceInfoDataGridView.SelectedRows(0).Cells(0).Value.ToString, _
      Me.ResourceInfoDataGridView.SelectedRows(0).Cells(1).Value.ToString)
    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
      Using svc As New PTService.PTService
        SetCredentials(svc)
        ' save the changes
        Dim resourceId As Integer = CInt(dlg.IdLabel1.Text)
        Dim firstName As String = dlg.FirstNameTextBox.Text
        Dim lastName As String = dlg.LastNameTextBox.Text
        svc.ChangeResourceName(resourceId, firstName, lastName)
        ' refresh the resource list
        Me.ResourceInfoBindingSource.DataSource = svc.GetResourceList
      End Using
    End If

  End Sub

  Private Sub SetCredentials(ByVal svc As PTService.PTService)

    Dim credentials As New PTService.CslaCredentials
    credentials.Username = UsernameTextBox.Text
    credentials.Password = PasswordTextBox.Text
    svc.CslaCredentialsValue = credentials

  End Sub

End Class
