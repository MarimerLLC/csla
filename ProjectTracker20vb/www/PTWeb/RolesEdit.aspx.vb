Option Strict On

Imports ProjectTracker.Library.Admin

Partial Class RolesEdit
  Inherits System.Web.UI.Page

  Protected Sub AddRoleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddRoleButton.Click

    Me.DetailsView1.DefaultMode = DetailsViewMode.Insert
    MultiView1.ActiveViewIndex = 1

  End Sub

  Protected Sub DetailsView1_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles DetailsView1.ItemInserted

    MultiView1.ActiveViewIndex = 0

  End Sub

  Protected Sub DetailsView1_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DetailsView1.ModeChanged

    MultiView1.ActiveViewIndex = 0

  End Sub

End Class
