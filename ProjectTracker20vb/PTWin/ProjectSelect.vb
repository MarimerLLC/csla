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
    Me.Close()

  End Sub

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

    Me.Close()

  End Sub

  Private Sub ProjectSelect_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Dim sorted As New Csla.SortedBindingList(Of ProjectInfo)(ProjectList.GetProjectList)
    sorted.ApplySort("Name", System.ComponentModel.ListSortDirection.Ascending)
    Me.ProjectListBindingSource.DataSource = sorted

  End Sub

  Private Sub GetListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetListButton.Click

    Me.ProjectListBindingSource.DataSource = ProjectList.GetProjectList(NameTextBox.Text)

  End Sub

End Class
