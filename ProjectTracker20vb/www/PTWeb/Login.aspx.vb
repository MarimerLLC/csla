
Partial Class Login
    Inherits System.Web.UI.Page

  Protected Sub LoginButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LoginButton.Click

    ProjectTracker.Library.Security.PTPrincipal.Login(UsernameTextBox.Text, PasswordTextBox.Text)
    HttpContext.Current.User = System.Threading.Thread.CurrentPrincipal
    If My.User.IsAuthenticated Then
      Session("CslaPrincipal") = System.Threading.Thread.CurrentPrincipal
      System.Web.Security.FormsAuthentication.RedirectFromLoginPage(My.User.Name, False)
    End If

  End Sub

End Class
