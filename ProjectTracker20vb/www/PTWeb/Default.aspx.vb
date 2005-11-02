Option Strict On

Partial Class _Default
  Inherits System.Web.UI.Page

  Protected Sub ProjectsButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ProjectsButton.Click

    Response.Redirect("ProjectList.aspx")

  End Sub

  Protected Sub ResourcesButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ResourcesButton.Click

    Response.Redirect("ResourceList.aspx")

  End Sub

  Protected Sub EditRolesButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EditRolesButton.Click

    Response.Redirect("RolesEdit.aspx")

  End Sub

  Protected Sub LoginStatus1_LoggingOut(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.LoginCancelEventArgs) Handles LoginStatus1.LoggingOut

    ProjectTracker.Library.Security.PTPrincipal.Logout()
    Session.Remove("CslaPrincipal")
    System.Web.Security.FormsAuthentication.SignOut()

  End Sub

End Class
