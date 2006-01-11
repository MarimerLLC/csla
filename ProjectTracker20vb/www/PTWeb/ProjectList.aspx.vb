Option Strict On

Imports ProjectTracker.Library

Partial Class ProjectList
  Inherits System.Web.UI.Page

  Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged

    Dim idString As String = GridView1.SelectedDataKey.Value.ToString
    Response.Redirect("ProjectEdit.aspx?id=" & idString)

  End Sub

  Protected Sub NewProjectButton_Click(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles NewProjectButton.Click

    Response.Redirect("ProjectEdit.aspx")

  End Sub

  Protected Sub Page_Load(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles Me.Load

    Me.GridView1.Columns(Me.GridView1.Columns.Count - 1).Visible = Project.CanDeleteObject
    NewProjectButton.Visible = Project.CanAddObject

  End Sub

  Protected Sub ProjectListDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles ProjectListDataSource.DeleteObject

    ProjectTracker.Library.Project.DeleteProject(New Guid(e.Keys("Id").ToString))

  End Sub

  Protected Sub ProjectListDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles ProjectListDataSource.SelectObject

    e.BusinessObject = GetProjectList()

  End Sub

  Private Function GetProjectList() As ProjectTracker.Library.ProjectList

    Dim businessObject As Object = Session("currentObject")
    If businessObject Is Nothing OrElse Not TypeOf businessObject Is ProjectList Then
      businessObject = ProjectTracker.Library.ProjectList.GetProjectList
      Session("currentObject") = businessObject
    End If
    Return CType(businessObject, ProjectTracker.Library.ProjectList)

  End Function

End Class
