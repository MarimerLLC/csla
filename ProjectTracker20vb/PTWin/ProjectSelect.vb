Imports System.Windows.Forms

Public Class ProjectSelect

  Private mProjectId As Guid

  Public ReadOnly Property ProjectId() As Guid
    Get
      Return mProjectId
    End Get
  End Property

  Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

    mProjectId = CType(Me.ProjectListListBox.SelectedValue, Guid)
    Me.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.Close()

  End Sub

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

    Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Close()

  End Sub

  Private Sub ProjectSelect_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Me.ProjectListBindingSource.DataSource = ProjectList.GetProjectList

  End Sub

  Private Sub GetListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetListButton.Click

    Me.ProjectListBindingSource.DataSource = ProjectList.GetProjectList(NameTextBox.Text)

  End Sub

End Class
