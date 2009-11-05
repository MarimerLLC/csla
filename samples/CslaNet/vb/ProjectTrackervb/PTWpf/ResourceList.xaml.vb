Imports ProjectTracker.Library

''' <summary>
''' Interaction logic for ResourceList.xaml
''' </summary>

Partial Public Class ResourceList
  Inherits EditForm

  Private Sub ShowResource(ByVal sender As Object, ByVal e As EventArgs)
    Dim item As ResourceInfo = CType(Me.listBox1.SelectedItem, ResourceInfo)

    If item IsNot Nothing Then
      Dim frm As ResourceEdit = New ResourceEdit(item.Id)
      MainForm.ShowControl(frm)
    End If

  End Sub

End Class