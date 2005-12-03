
Partial Class ResourceEdit
  Inherits System.Web.UI.Page

  Protected Sub Page_Load(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles Me.Load

    If Not IsPostBack Then
      Dim idString As String = Request.QueryString("id")
      Dim obj As ProjectTracker.Library.Resource
      If Len(idString) > 0 Then
        obj = ProjectTracker.Library.Resource.GetResource(idString)
        If ProjectTracker.Library.Resource.CanSaveObject Then
          Me.DetailsView1.DefaultMode = DetailsViewMode.Edit

        Else
          Me.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly
        End If

      Else
        obj = ProjectTracker.Library.Resource.NewResource("new")
        Me.DetailsView1.DefaultMode = DetailsViewMode.Insert
      End If
      Session("currentObject") = obj
    End If

  End Sub

  Protected Sub ResourceListButton_Click(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles ResourceListButton.Click

    Response.Redirect("ResourceList.aspx")

  End Sub

  Protected Sub DetailsView1_ItemDeleted(ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles DetailsView1.ItemDeleted

    Response.Redirect("ResourceList.aspx")

  End Sub

  Protected Sub RoleListDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles RoleListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.RoleList.GetList

  End Sub

#Region " ResourceDataSource "

  Protected Sub ResourceDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles ResourceDataSource.DeleteObject

    ProjectTracker.Library.Resource.DeleteResource(e.Keys("Id"))
    Session.Remove("currentObject")
    e.RowsAffected = 1

  End Sub

  Protected Sub ResourceDataSource_InsertObject(ByVal sender As Object, _
    ByVal e As Csla.Web.InsertObjectArgs) Handles ResourceDataSource.InsertObject

    Dim obj As ProjectTracker.Library.Resource = _
      ProjectTracker.Library.Resource.NewResource(e.Values("Id"))
    Csla.Data.DataMapper.Map(e.Values, obj, "Id")
    Session("currentObject") = obj.Save
    e.RowsAffected = 1

  End Sub

  Protected Sub ResourceDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles ResourceDataSource.SelectObject

    e.BusinessObject = Session("currentObject")

  End Sub

  Protected Sub ResourceDataSource_UpdateObject(ByVal sender As Object, _
    ByVal e As Csla.Web.UpdateObjectArgs) Handles ResourceDataSource.UpdateObject

    Dim obj As ProjectTracker.Library.Resource = _
      Session("currentObject")
    Csla.Data.DataMapper.Map(e.Values, obj)
    Session("currentObject") = obj.Save
    e.RowsAffected = 1

  End Sub

#End Region

#Region " AssignmentsDataSource "

  Protected Sub AssignmentsDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles AssignmentsDataSource.DeleteObject

    Dim obj As ProjectTracker.Library.Resource = Session("currentObject")
    Dim res As ProjectTracker.Library.ResourceAssignment
    Dim rid As Object = e.Keys("ProjectId")
    res = obj.Assignments(rid)
    obj.Assignments.Remove(res.ProjectID)
    Session("currentObject") = obj.Save()
    e.RowsAffected = 1

  End Sub

  Protected Sub AssignmentsDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles AssignmentsDataSource.SelectObject

    Dim obj As ProjectTracker.Library.Resource = _
      Session("currentObject")
    e.BusinessObject = obj.Assignments

  End Sub

  Protected Sub AssignmentsDataSource_UpdateObject(ByVal sender As Object, _
    ByVal e As Csla.Web.UpdateObjectArgs) Handles AssignmentsDataSource.UpdateObject

    Dim obj As ProjectTracker.Library.Resource = Session("currentObject")
    Dim res As ProjectTracker.Library.ResourceAssignment
    Dim rid As Object = e.OldValues("ProjectId")
    res = obj.Assignments(rid)
    Csla.Data.DataMapper.Map(e.Values, res)
    Session("currentObject") = obj.Save()
    e.RowsAffected = 1

  End Sub

#End Region

End Class
