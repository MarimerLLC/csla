Imports ProjectTracker.Library

Public Class ProjectEdit
  Inherits WinPart

  Private WithEvents mProject As Project

  Public ReadOnly Property Project() As Project
    Get
      Return mProject
    End Get
  End Property

#Region " WinPart Code "

  Public Overrides Property Title() As String
    Get
      Return mProject.Name
    End Get
    Set(ByVal value As String)
      mProject.Name = value
    End Set
  End Property

  Public Overrides Function Equals(ByVal obj As Object) As Boolean

    If TypeOf obj Is ProjectEdit Then
      Return CType(obj, ProjectEdit).Project.Equals(mProject)

    Else
      Return False
    End If

  End Function

  Private Sub ProjectEdit_CurrentPrincipalChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.CurrentPrincipalChanged

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

  Private Sub SaveProject()

    Using busy As New StatusBusy("Saving...")
      ' stop the flow of events
      Me.ProjectBindingSource.RaiseListChangedEvents = False
      Me.ResourcesBindingSource.RaiseListChangedEvents = False

      ' do the save
      Dim old As Project = mProject.Clone
      mProject.ApplyEdit()
      Try
        mProject = mProject.Save
        mProject.BeginEdit()

      Catch ex As Exception
        mProject = old
        MessageBox.Show(ex.ToString, "Save error", _
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End Try

      ' rebind the UI
      Me.ProjectBindingSource.DataSource = mProject
      Me.ResourcesBindingSource.DataSource = mProject.Resources
      Me.ProjectBindingSource.RaiseListChangedEvents = True
      Me.ResourcesBindingSource.RaiseListChangedEvents = True
      ApplyAuthorizationRules()
    End Using

  End Sub

  Public Sub New(ByVal project As Project)

    InitializeComponent()

    mProject = project
    mProject.BeginEdit()

    Me.RoleListBindingSource.DataSource = RoleList.GetList
    Me.ProjectBindingSource.DataSource = mProject
    Me.ResourcesBindingSource.DataSource = mProject.Resources

    ApplyAuthorizationRules()

  End Sub

  Private Sub OKButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles OKButton.Click

    SaveProject()
    Me.Close()

  End Sub

  Private Sub ApplyButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles ApplyButton.Click

    SaveProject()

  End Sub

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles Cancel_Button.Click

    mProject.CancelEdit()

  End Sub

  Private Sub CloseButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles CloseButton.Click

    mProject.CancelEdit()
    Me.Close()

  End Sub

  Private Sub AssignButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles AssignButton.Click

    Dim dlg As New ResourceSelect
    If dlg.ShowDialog = DialogResult.OK Then
      mProject.Resources.Assign(dlg.ResourceId)
    End If

  End Sub

  Private Sub UnassignButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles UnassignButton.Click

    If Me.ResourcesDataGridView.SelectedRows.Count > 0 Then
      Dim resourceId As String = _
        CStr(Me.ResourcesDataGridView.SelectedRows(0).Cells(0).Value)
      mProject.Resources.Remove(resourceId)
    End If

  End Sub

  Private Sub mProject_PropertyChanged(ByVal sender As Object, _
    ByVal e As System.ComponentModel.PropertyChangedEventArgs) _
    Handles mProject.PropertyChanged

    If e.PropertyName = "IsDirty" Then
      Me.ProjectBindingSource.ResetBindings(False)
      Me.ResourcesBindingSource.ResetBindings(False)
    End If

  End Sub

  Private Sub ResourcesDataGridView_CellContentClick(ByVal sender As System.Object, _
    ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ResourcesDataGridView.CellContentClick

    If e.ColumnIndex = 1 Then
      Dim resourceId As String = _
        CStr(Me.ResourcesDataGridView.Rows(e.RowIndex).Cells(0).Value)
      MainForm.ShowEditResource(resourceId)
    End If

  End Sub

  Private Sub ProjectEdit_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  End Sub
End Class
