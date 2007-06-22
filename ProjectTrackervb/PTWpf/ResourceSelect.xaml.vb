Imports ProjectTracker.Library

''' <summary>
''' Interaction logic for ResourceSelect.xaml
''' </summary>
Partial Public Class ResourceSelect
  Inherits Window

  Private mResourceId As Integer

  Public ReadOnly Property ResourceId() As Integer
    Get
      Return mResourceId
    End Get
  End Property


  Private Sub OkButton(ByVal sender As Object, ByVal e As EventArgs)
    Dim item As ResourceInfo = CType(ResourceListBox.SelectedItem, ResourceInfo)
    If Not item Is Nothing Then
      DialogResult = True
      mResourceId = (item).Id
    Else
      DialogResult = False
    End If
    Hide()
  End Sub

  Private Sub CancelButton(ByVal sender As Object, ByVal e As EventArgs)
    DialogResult = False
    Hide()
  End Sub

  Private Sub ResourceSelected(ByVal sender As Object, ByVal e As EventArgs)
    OkButton(sender, e)
  End Sub

End Class
