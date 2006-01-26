Imports ProjectTracker.Library

Public Class ResourceEdit
  Inherits WinPart

  Private WithEvents mResource As Resource

  Public ReadOnly Property Resource() As Resource
    Get
      Return mResource
    End Get
  End Property

  Public Sub New(ByVal resource As Resource)

    InitializeComponent()

    mResource = resource
    mResource.BeginEdit()

    Me.RoleListBindingSource.DataSource = RoleList.GetList
    Me.ResourceBindingSource.DataSource = mResource

    ApplyAuthorizationRules()

  End Sub

#Region " WinPart Code "

  Protected Overrides Function GetIdValue() As Object

    Return mResource

  End Function

  Public Overrides Function ToString() As String

    Return mResource.FullName

  End Function

  Private Sub ResourceEdit_CurrentPrincipalChanged( _
  ByVal sender As Object, _
  ByVal e As System.EventArgs) _
  Handles Me.CurrentPrincipalChanged

    ApplyAuthorizationRules()

  End Sub

#End Region

  Private Sub ApplyAuthorizationRules()

    ' have the controls enable/disable/etc
    Me.ReadWriteAuthorization1.ResetControlAuthorization()

    Dim canEdit As Boolean = _
      ProjectTracker.Library.Project.CanEditObject

    ' enable/disable appropriate buttons
    Me.OKButton.Enabled = canEdit
    Me.ApplyButton.Enabled = canEdit
    Me.Cancel_Button.Enabled = canEdit

    ' enable/disable role column in grid
    Me.AssignmentsDataGridView.Columns(3).ReadOnly = Not canEdit

  End Sub

  Private Sub SaveResource(ByVal rebind As Boolean)

    Using busy As New StatusBusy("Saving...")
      ' stop the flow of events
      Me.ResourceBindingSource.RaiseListChangedEvents = False
      Me.AssignmentsBindingSource.RaiseListChangedEvents = False

      ' do the save
      Dim temp As Resource = mResource.Clone
      temp.ApplyEdit()
      Try
        mResource = temp.Save
        mResource.BeginEdit()
        If rebind Then
          ' rebind the UI
          Me.ResourceBindingSource.DataSource = Nothing
          Me.AssignmentsBindingSource.DataSource = Nothing
          Me.ResourceBindingSource.DataSource = mResource
          ApplyAuthorizationRules()
        End If

      Catch ex As Csla.DataPortalException
        MessageBox.Show(ex.BusinessException.ToString, _
          "Error saving", MessageBoxButtons.OK, _
          MessageBoxIcon.Exclamation)

      Catch ex As Exception
        MessageBox.Show(ex.ToString, _
          "Error saving", MessageBoxButtons.OK, _
          MessageBoxIcon.Exclamation)

      Finally
        Me.ResourceBindingSource.RaiseListChangedEvents = True
        Me.AssignmentsBindingSource.RaiseListChangedEvents = True
      End Try

    End Using

  End Sub

  Private Sub OKButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles OKButton.Click

    SaveResource(False)
    Me.Close()

  End Sub

  Private Sub ApplyButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles ApplyButton.Click

    SaveResource(True)

  End Sub

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles Cancel_Button.Click

    mResource.CancelEdit()

  End Sub

  Private Sub CloseButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles CloseButton.Click

    mResource.CancelEdit()
    Me.Close()

  End Sub

  Private Sub AssignButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles AssignButton.Click

    Dim dlg As New ProjectSelect
    If dlg.ShowDialog = DialogResult.OK Then
      Try
        mResource.Assignments.AssignTo(dlg.ProjectId)

      Catch ex As InvalidOperationException
        MessageBox.Show(ex.ToString, _
          "Error assigning", MessageBoxButtons.OK, _
          MessageBoxIcon.Information)

      Catch ex As Exception
        MessageBox.Show(ex.ToString, _
          "Error assigning", MessageBoxButtons.OK, _
          MessageBoxIcon.Exclamation)
      End Try
    End If

  End Sub

  Private Sub UnassignButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles UnassignButton.Click

    If Me.AssignmentsDataGridView.SelectedRows.Count > 0 Then
      Dim projectId As Guid = _
        CType(Me.AssignmentsDataGridView.SelectedRows(0).Cells(0).Value, Guid)
      mResource.Assignments.Remove(projectId)
    End If

  End Sub

  Private Sub AssignmentsDataGridView_CellContentClick(ByVal sender As System.Object, _
    ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
    Handles AssignmentsDataGridView.CellContentClick

    If e.ColumnIndex = 1 And e.RowIndex > -1 Then
      Dim projectId As Guid = _
        CType(Me.AssignmentsDataGridView.Rows(e.RowIndex).Cells(0).Value, Guid)
      MainForm.ShowEditProject(projectId)
    End If

  End Sub

End Class
