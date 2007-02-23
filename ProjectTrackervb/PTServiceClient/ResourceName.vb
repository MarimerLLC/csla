Imports System.Windows.Forms

Public Class ResourceName

  Public Sub New(ByVal id As String, ByVal name As String)

    InitializeComponent()
    Me.IdLabel1.Text = id
    Me.NameLabel1.Text = name

  End Sub

  Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
    Me.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.Close()
  End Sub

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
    Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Close()
  End Sub

End Class
