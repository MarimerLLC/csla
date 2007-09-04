Imports ProjectTracker.Library

Public Class ProjectEdit
  Inherits WinPart

  Private WithEvents mProject As Project

  Public ReadOnly Property Project() As Project
    Get
      Return mProject
    End Get
  End Property

  Public Sub New(ByVal project As Project)

    InitializeComponent()

    mProject = project

    Me.RoleListBindingSource.DataSource = RoleList.GetList

    BindUI()

    ApplyAuthorizationRules()

  End Sub

#Region " WinPart Code "

  Protected Overrides Function GetIdValue() As Object

    Return mProject

  End Function

  Public Overrides Function ToString() As String

    Return mProject.Name

  End Function

  Private Sub ProjectEdit_CurrentPrincipalChanged( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
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
    Me.AssignButton.Enabled = canEdit
    Me.UnassignButton.Enabled = canEdit

    ' enable/disable role column in grid
    Me.ResourcesDataGridView.Columns(2).ReadOnly = Not canEdit

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

    mProject.BeginEdit()
    Me.ProjectBindingSource.DataSource = mProject

  End Sub

  Private Sub RebindUI(ByVal saveObject As Boolean, ByVal rebind As Boolean)

    ' disable events
    Me.ProjectBindingSource.RaiseListChangedEvents = False
    Me.ResourcesBindingSource.RaiseListChangedEvents = False
    Try
      ' unbind the UI
      UnbindBindingSource(Me.ResourcesBindingSource, saveObject, False)
      UnbindBindingSource(Me.ProjectBindingSource, saveObject, True)
      Me.ResourcesBindingSource.DataSource = Me.ProjectBindingSource

      ' save or cancel changes
      If saveObject Then
        mProject.ApplyEdit()
        Try
          Dim temp As Project = mProject.Clone()
          mProject = temp.Save()

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
        mProject.CancelEdit()
      End If

      ' rebind UI if requested
      If rebind Then
        BindUI()
      End If

    Finally
      ' restore events
      Me.ProjectBindingSource.RaiseListChangedEvents = True
      Me.ResourcesBindingSource.RaiseListChangedEvents = True

      If rebind Then
        ' refresh the UI if rebinding
        Me.ProjectBindingSource.ResetBindings(False)
        Me.ResourcesBindingSource.ResetBindings(False)
      End If
    End Try

  End Sub

  Private Sub AssignButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles AssignButton.Click

    Dim dlg As New ResourceSelect
    If dlg.ShowDialog = DialogResult.OK Then
      Try
        mProject.Resources.Assign(dlg.ResourceId)

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

    If Me.ResourcesDataGridView.SelectedRows.Count > 0 Then
      Dim resourceId As Integer = _
        CInt(Me.ResourcesDataGridView.SelectedRows(0).Cells(0).Value)
      mProject.Resources.Remove(resourceId)
    End If

  End Sub

  Private Sub ResourcesDataGridView_CellContentClick( _
    ByVal sender As System.Object, _
    ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
    Handles ResourcesDataGridView.CellContentClick

    If e.ColumnIndex = 1 And e.RowIndex > -1 Then
      Dim resourceId As Integer = _
        CInt(Me.ResourcesDataGridView.Rows(e.RowIndex).Cells(0).Value)
      MainForm.ShowEditResource(resourceId)
    End If

  End Sub

  Private Sub ProjectBindingSource_CurrentItemChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProjectBindingSource.CurrentItemChanged

    Me.OKButton.Enabled = mProject.IsSavable

  End Sub

  Private Sub ResourcesBindingSource_CurrentItemChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResourcesBindingSource.CurrentItemChanged

    Me.OKButton.Enabled = mProject.IsSavable

  End Sub

End Class
