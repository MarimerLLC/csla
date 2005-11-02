
Partial Class ProjectList
  Inherits System.Web.UI.Page

  Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged

    Dim idString As String = GridView1.SelectedDataKey.Value.ToString
    Response.Redirect("ProjectEdit.aspx?id=" & idString)

  End Sub

  Protected Sub NewProjectButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewProjectButton.Click

    Response.Redirect("ProjectEdit.aspx")

  End Sub

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    NewProjectButton.Visible = ProjectTracker.Library.Project.CanAddObject

  End Sub

  Protected Sub ProjectListDataSource_DeleteObject(ByVal sender As Object, ByVal e As DataControls.DeleteObjectArgs) Handles ProjectListDataSource.DeleteObject

    ProjectTracker.Library.Project.DeleteProject(New Guid(e.Keys("Id").ToString))

  End Sub

  Protected Sub ProjectListDataSource_SelectObject(ByVal sender As Object, ByVal e As DataControls.SelectObjectArgs) Handles ProjectListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.ProjectList.GetProjectList

  End Sub

End Class
