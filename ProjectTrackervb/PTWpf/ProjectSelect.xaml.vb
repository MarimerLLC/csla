Imports ProjectTracker.Library

''' <summary>
''' Interaction logic for ProjectSelect.xaml
''' </summary>
Partial Public Class ProjectSelect
  Inherits Window

  Private mProjectId As Guid

  Public ReadOnly Property ProjectId() As Guid
    Get
      Return mProjectId
    End Get
  End Property


  Private Sub OkButton(ByVal sender As Object, ByVal e As EventArgs)

    Dim item As ProjectInfo = CType(ProjectListBox.SelectedItem, ProjectInfo)
    If Not item Is Nothing Then
      DialogResult = True
      mProjectId = (item).Id

    Else
      DialogResult = False
    End If
    Hide()

  End Sub

  Private Sub CancelButton(ByVal sender As Object, ByVal e As EventArgs)
    DialogResult = False
    Hide()
  End Sub

  Private Sub ProjectSelected(ByVal sender As Object, ByVal e As EventArgs)
    OkButton(sender, e)
  End Sub

End Class
