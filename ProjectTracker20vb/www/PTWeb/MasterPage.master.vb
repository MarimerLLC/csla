Option Strict On

Partial Class MasterPage
    Inherits System.Web.UI.MasterPage

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    PageTitle.Text = Page.Title

    'SetDefaultSecurity()

  End Sub

  Private Sub SetDefaultSecurity()

    Try
      If TypeOf System.Threading.Thread.CurrentPrincipal Is System.Security.Principal.GenericPrincipal _
          AndAlso Csla.ApplicationContext.AuthenticationType <> "Windows" Then

        Dim x As ProjectTracker.Library.Project = Nothing

        ProjectTracker.Library.Security.PTPrincipal.Logout()
        Session("CslaPrincipal") = System.Threading.Thread.CurrentPrincipal
      End If

    Catch
      ' do nothing
    End Try

  End Sub

  Protected Sub LoginStatus1_LoggingOut(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.LoginCancelEventArgs) Handles LoginStatus1.LoggingOut

    ProjectTracker.Library.Security.PTPrincipal.Logout()
    Session("CslaPrincipal") = System.Threading.Thread.CurrentPrincipal
    System.Web.Security.FormsAuthentication.SignOut()

  End Sub

End Class

