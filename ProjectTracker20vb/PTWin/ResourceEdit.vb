Imports ProjectTracker.Library

Public Class ResourceEdit
  Inherits WinPart

  Private WithEvents mResource As Resource

  Public ReadOnly Property Resource() As Resource
    Get
      Return mResource
    End Get
  End Property

#Region " WinPart Code "

  Public Overrides Property Title() As String
    Get
      Return mResource.LastName
    End Get
    Set(ByVal value As String)
      mResource.LastName = value
    End Set
  End Property

  Public Overrides Function Equals(ByVal obj As Object) As Boolean

    If TypeOf obj Is ResourceEdit Then
      Return CType(obj, ResourceEdit).Resource.Equals(mResource)

    Else
      Return False
    End If

  End Function

  Private Sub ProjectEdit_CurrentPrincipalChanged( _
  ByVal sender As Object, _
  ByVal e As System.EventArgs) _
  Handles Me.CurrentPrincipalChanged

    ApplyAuthorizationRules()

  End Sub

#End Region

  Private Sub ApplyAuthorizationRules()

    ' have the controls enable/disable/etc
    Me.ReadWriteAuthorization1.ResetControlAuthorization()

    Dim canSave As Boolean = _
      ProjectTracker.Library.Project.CanSaveObject

    ' enable/disable appropriate buttons
    Me.OKButton.Enabled = canSave
    Me.ApplyButton.Enabled = canSave
    Me.Cancel_Button.Enabled = canSave

  End Sub

  Private Sub SaveResource()

    Using busy As New StatusBusy("Saving...")
      ' stop the flow of events
      Me.ResourceBindingSource.RaiseListChangedEvents = False
      Me.AssignmentsBindingSource.RaiseListChangedEvents = False

      ' do the save
      Dim old As Resource = CType(mResource.Clone, Resource)
      mResource.ApplyEdit()
      Try
        mResource = mResource.Save
        mResource.BeginEdit()

      Catch ex As Exception
        mResource = old
        MessageBox.Show(ex.ToString, "Save error", _
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End Try

      ' rebind the UI
      Me.ResourceBindingSource.DataSource = mResource
      Me.AssignmentsBindingSource.DataSource = mResource.Assignments
      Me.ResourceBindingSource.RaiseListChangedEvents = True
      Me.AssignmentsBindingSource.RaiseListChangedEvents = True
      ApplyAuthorizationRules()
    End Using

  End Sub

  Public Sub New(ByVal resource As Resource)

    InitializeComponent()

    mResource = resource
    mResource.BeginEdit()

    Me.RoleListBindingSource.DataSource = RoleList.GetList
    Me.ResourceBindingSource.DataSource = mResource
    Me.AssignmentsBindingSource.DataSource = mResource.Assignments

    ApplyAuthorizationRules()

  End Sub

  Private Sub OKButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles OKButton.Click

    SaveResource()
    Me.Close()

  End Sub

  Private Sub ApplyButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles ApplyButton.Click

    SaveResource()

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
      mResource.Assignments.AssignTo(dlg.ProjectId)
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

  Private Sub mProject_PropertyChanged(ByVal sender As Object, _
    ByVal e As System.ComponentModel.PropertyChangedEventArgs) _
    Handles mResource.PropertyChanged

    If e.PropertyName = "IsDirty" Then
      Me.ResourceBindingSource.ResetBindings(False)
      Me.AssignmentsBindingSource.ResetBindings(False)
    End If

  End Sub

  Private Sub AssignmentsDataGridView_CellContentClick(ByVal sender As System.Object, _
    ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
    Handles AssignmentsDataGridView.CellContentClick

    If e.ColumnIndex = 1 Then
      Dim projectId As Guid = _
        CType(Me.AssignmentsDataGridView.Rows(e.RowIndex).Cells(0).Value, Guid)
      MainForm.ShowEditProject(projectId)
    End If

  End Sub

End Class
