Option Strict On

Imports ProjectTracker.Library.Admin

Partial Class RolesEdit
  Inherits System.Web.UI.Page

  Protected Sub GridView1_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles GridView1.RowEditing

    Dim pcnt As Integer = Me.RoleDataSource.SelectParameters.Count
    '= GridView1.Rows(e.NewEditIndex).Cells(0).Text
    Me.MultiView3.ActiveViewIndex = 1

  End Sub

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    If IsPostBack Then

      Me.MultiView3.ActiveViewIndex = 0

    End If

  End Sub

End Class
