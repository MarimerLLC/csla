
Partial Class ResourceList
    Inherits System.Web.UI.Page

  Protected Sub NewResourceButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewResourceButton.Click

    Response.Redirect("ResourceEdit.aspx")

  End Sub

  Protected Sub ResourceListDataSource_DeleteObject(ByVal sender As Object, ByVal e As Csla.Web.DeleteObjectArgs) Handles ResourceListDataSource.DeleteObject

    ProjectTracker.Library.Resource.DeleteResource(e.Keys("Id"))

  End Sub

  Protected Sub ResourceListDataSource_SelectObject(ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) Handles ResourceListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.ResourceList.GetResourceList

  End Sub

  Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged

    Dim idString As String = GridView1.SelectedDataKey.Value.ToString
    Response.Redirect("ResourceEdit.aspx?id=" & idString)

  End Sub

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    Me.GridView1.DataKeyNames = New String() {"Id"}
    NewResourceButton.Visible = ProjectTracker.Library.Resource.CanAddObject

  End Sub

End Class
