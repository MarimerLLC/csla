Imports System.Windows.Forms

Public Class ResourceSelect

  Private mResourceId As Integer
  Public ReadOnly Property ResourceId() As Integer
    Get
      Return mResourceId
    End Get
  End Property

  Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

    mResourceId = CType(Me.ResourceListListBox.SelectedValue, ResourceInfo).Id
    Me.Close()

  End Sub

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

    Me.Close()

  End Sub

  Private Sub ResourceSelect_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Me.ResourceListBindingSource.DataSource = ResourceList.GetResourceList

  End Sub

End Class
