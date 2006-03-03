Option Strict On

Partial Class MasterPage
    Inherits System.Web.UI.MasterPage

  Protected Sub Page_Load( _
    ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    PageTitle.Text = Page.Title

  End Sub

  Protected Sub LoginStatus1_LoggingOut( _
    ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.LoginCancelEventArgs) _
    Handles LoginStatus1.LoggingOut

    ProjectTracker.Library.Security.PTPrincipal.Logout()
    Session("CslaPrincipal") = Csla.ApplicationContext.User
    System.Web.Security.FormsAuthentication.SignOut()

  End Sub

End Class

