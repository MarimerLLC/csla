Option Strict On

Partial Class MasterPage
    Inherits System.Web.UI.MasterPage

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    PageTitle.Text = Page.Title

  End Sub

End Class

