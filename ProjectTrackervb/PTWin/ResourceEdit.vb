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

    Me.RoleListBindingSource.DataSource = RoleList.GetList

    BindUI()

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
      ProjectTracker.Library.Resource.CanEditObject

    ' enable/disable appropriate buttons
    Me.OKButton.Enabled = canEdit
    Me.ApplyButton.Enabled = canEdit
    Me.Cancel_Button.Enabled = canEdit

    ' enable/disable role column in grid
    Me.AssignmentsDataGridView.Columns(3).ReadOnly = Not canEdit

  End Sub

  Private Sub OKButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles OKButton.Click

    Using busy As New StatusBusy("Saving...")
      RebindUI(True, False)
    End Using
    Me.Close()

  End Sub

  Private Sub ApplyButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles ApplyButton.Click

    Using busy As New StatusBusy("Saving...")
      RebindUI(True, True)
    End Using

  End Sub

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles Cancel_Button.Click

    RebindUI(False, True)

  End Sub

  Private Sub CloseButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles CloseButton.Click

    RebindUI(False, False)
    Me.Close()

  End Sub

  Private Sub BindUI()

    mResource.BeginEdit()
    Me.ResourceBindingSource.DataSource = mResource

  End Sub

  Private Sub RebindUI(ByVal saveObject As Boolean, ByVal rebind As Boolean)

    ' disable events
    Me.ResourceBindingSource.RaiseListChangedEvents = False
    Me.AssignmentsBindingSource.RaiseListChangedEvents = False
    Try
      ' unbind the UI
      UnbindBindingSource(Me.AssignmentsBindingSource, saveObject, False)
      UnbindBindingSource(Me.ResourceBindingSource, saveObject, True)
      Me.AssignmentsBindingSource.DataSource = Me.ResourceBindingSource

      ' save or cancel changes
      If saveObject Then
        mResource.ApplyEdit()
        Try
          mResource = mResource.Save()

        Catch ex As Csla.DataPortalException
          MessageBox.Show(ex.BusinessException.ToString(), _
            "Error saving", MessageBoxButtons.OK, _
            MessageBoxIcon.Exclamation)

        Catch ex As Exception
          MessageBox.Show(ex.ToString(), _
            "Error Saving", MessageBoxButtons.OK, _
            MessageBoxIcon.Exclamation)
        End Try

      Else
        mResource.CancelEdit()
      End If

    Finally
      ' rebind UI if requested
      If rebind Then
        BindUI()
      End If

      ' restore events
      Me.ResourceBindingSource.RaiseListChangedEvents = True
      Me.AssignmentsBindingSource.RaiseListChangedEvents = True

      If rebind Then
        ' refresh the UI if rebinding
        Me.ResourceBindingSource.ResetBindings(False)
        Me.AssignmentsBindingSource.ResetBindings(False)
      End If
    End Try

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
