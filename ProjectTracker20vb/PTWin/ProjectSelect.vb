Imports System.Windows.Forms
Imports System.ComponentModel

Public Class ProjectSelect

  Private mProjectId As Guid

  Public ReadOnly Property ProjectId() As Guid
    Get
      Return mProjectId
    End Get
  End Property

  Private Sub OK_Button_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles OK_Button.Click

    mProjectId = CType(Me.ProjectListListBox.SelectedValue, Guid)
    Me.Close()

  End Sub

  Private Sub Cancel_Button_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles Cancel_Button.Click

    Me.Close()

  End Sub

  Private Sub ProjectSelect_Load( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load

    DisplayList(ProjectList.GetProjectList)

  End Sub

  Private Sub DisplayList(ByVal list As ProjectList)

    Dim sortedList As New Csla.SortedBindingList(Of ProjectInfo)(list)
    sortedList.ApplySort("Name", ListSortDirection.Ascending)
    Me.ProjectListBindingSource.DataSource = sortedList

  End Sub

  Private Sub GetListButton_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles GetListButton.Click

    DisplayList(ProjectList.GetProjectList(NameTextBox.Text))

  End Sub

End Class
