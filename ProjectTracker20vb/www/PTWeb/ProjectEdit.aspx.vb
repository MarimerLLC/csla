
Partial Class ProjectEdit
  Inherits System.Web.UI.Page

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    If Not IsPostBack Then
      Dim idString As String = Request.QueryString("id")
      Dim obj As ProjectTracker.Library.Project
      If Len(idString) > 0 Then
        Dim id As Guid
        id = New Guid(idString)
        obj = ProjectTracker.Library.Project.GetProject(id)
        If ProjectTracker.Library.Project.CanSaveObject Then
          Me.DetailsView1.DefaultMode = DetailsViewMode.Edit

        Else
          Me.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly
        End If

      Else
        obj = ProjectTracker.Library.Project.NewProject
        Me.DetailsView1.DefaultMode = DetailsViewMode.Insert
      End If
        Session("currentObject") = obj
      End If

  End Sub

  Protected Sub DetailsView1_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles DetailsView1.ItemDeleted

    Response.Redirect("ProjectList.aspx")

  End Sub

  Protected Sub RoleListDataSource_SelectObject(ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) Handles RoleListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.RoleList.GetList

  End Sub

  Protected Sub ProjectListButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ProjectListButton.Click

    Response.Redirect("ProjectList.aspx")

  End Sub

#Region " Project "

  Protected Sub ProjectDataSource_DeleteObject(ByVal sender As Object, ByVal e As Csla.Web.DeleteObjectArgs) Handles ProjectDataSource.DeleteObject

    ProjectTracker.Library.Project.DeleteProject(New Guid(e.Keys("Id").ToString))
    Session("currentObject") = Nothing

  End Sub

  Protected Sub ProjectDataSource_InsertObject(ByVal sender As Object, ByVal e As Csla.Web.InsertObjectArgs) Handles ProjectDataSource.InsertObject

    Dim obj As ProjectTracker.Library.Project = Session("currentObject")

    With obj
      .Name = e.Values.Item("Name")
      .Started = e.Values.Item("Started")
      .Ended = e.Values.Item("Ended")
      .Description = e.Values.Item("Description")
    End With
    obj.Save()

  End Sub

  Protected Sub ProjectDataSource_SelectObject(ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) Handles ProjectDataSource.SelectObject

    e.BusinessObject = Session("currentObject")

  End Sub

  Protected Sub ProjectDataSource_UpdateObject(ByVal sender As Object, ByVal e As Csla.Web.UpdateObjectArgs) Handles ProjectDataSource.UpdateObject

    Dim obj As ProjectTracker.Library.Project = Session("currentObject")

    With obj
      .Name = e.Values.Item("Name")
      .Started = e.Values.Item("Started")
      .Ended = e.Values.Item("Ended")
      .Description = e.Values.Item("Description")
    End With
    Session("currentObject") = obj.Save()

  End Sub

#End Region

#Region " Resources "

  Protected Sub ResourcesDataSource_DeleteObject(ByVal sender As Object, ByVal e As Csla.Web.DeleteObjectArgs) Handles ResourcesDataSource.DeleteObject

    Dim obj As ProjectTracker.Library.Project = Session("currentObject")
    Dim res As ProjectTracker.Library.ProjectResource
    Dim rid As Object = e.Keys("ResourceId")
    res = obj.Resources(rid)
    obj.Resources.Remove(res.ResourceId)
    Session("currentObject") = obj.Save()

  End Sub

  Protected Sub ResourcesDataSource_SelectObject(ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) Handles ResourcesDataSource.SelectObject

    Dim obj As ProjectTracker.Library.Project = Session("currentObject")
    e.BusinessObject = obj.Resources

  End Sub

  Protected Sub ResourcesDataSource_UpdateObject(ByVal sender As Object, ByVal e As Csla.Web.UpdateObjectArgs) Handles ResourcesDataSource.UpdateObject

    Dim obj As ProjectTracker.Library.Project = Session("currentObject")
    Dim res As ProjectTracker.Library.ProjectResource
    Dim rid As Object = e.OldValues("ResourceId")
    res = obj.Resources(rid)
    Dim roleNum As Object = e.Values("Role")
    res.Role = roleNum
    Session("currentObject") = obj.Save()

  End Sub

#End Region

End Class
