Public Class LoginForm

  Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click

    Using busy As New StatusBusy("Verifying credentials...")
      ProjectTracker.Library.Security.PTPrincipal.Login( _
          Me.UsernameTextBox.Text, Me.PasswordTextBox.Text)
    End Using
    Me.Close()

  End Sub

  Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click

    Me.Close()

  End Sub

  Private Sub LoginForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Me.UsernameTextBox.Focus()

  End Sub

End Class
