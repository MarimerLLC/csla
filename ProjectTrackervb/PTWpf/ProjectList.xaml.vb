''' <summary>
''' Interaction logic for ProjectList.xaml
''' </summary>

Partial Public Class ProjectList
  Inherits EditForm

  Private Sub ShowProject(ByVal sender As Object, ByVal e As EventArgs)
    Dim item As ProjectTracker.Library.ProjectInfo = CType(Me.listBox1.SelectedItem, ProjectTracker.Library.ProjectInfo)

    Dim frm As ProjectEdit = New ProjectEdit(item.Id)
    MainForm.ShowControl(frm)
  End Sub

End Class
