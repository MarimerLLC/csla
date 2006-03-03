Public Class LoginForm

  Private Sub OK_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles OK.Click

    Using busy As New StatusBusy("Verifying credentials...")
      ProjectTracker.Library.Security.PTPrincipal.Login( _
          Me.UsernameTextBox.Text, Me.PasswordTextBox.Text)
    End Using
    Me.Close()

  End Sub

  Private Sub Cancel_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles Cancel.Click

    Me.Close()

  End Sub

  Private Sub LoginForm_Load( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load

    Me.UsernameTextBox.Focus()

  End Sub

End Class
