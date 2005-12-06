Option Strict On

Imports ProjectTracker.Library

Partial Class ResourceEdit
  Inherits System.Web.UI.Page

  Private Enum Views
    MainView = 0
    AssignView = 1
  End Enum

  Protected Sub Page_Load(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles Me.Load

    If Not IsPostBack Then
      Try
        Dim idString As String = Request.QueryString("id")
        Dim obj As Resource
        If Len(idString) > 0 Then
          Dim id As Integer
          id = CInt(idString)
          obj = Resource.GetResource(id)

        Else
          obj = Resource.NewResource
        End If
        Session("currentObject") = obj
        Me.MultiView1.ActiveViewIndex = Views.MainView
        ApplyAuthorizationRules()

      Catch ex As System.Security.SecurityException
        Response.Redirect("ResourceList.aspx")
      End Try

    Else
      Me.ErrorLabel.Text = ""
    End If

  End Sub

  Private Sub ApplyAuthorizationRules()

    Dim obj As Resource = CType(Session("currentObject"), Resource)
    ' Resource display
    If Resource.CanSaveObject Then
      If obj.IsNew Then
        Me.DetailsView1.DefaultMode = DetailsViewMode.Insert

      Else
        Me.DetailsView1.DefaultMode = DetailsViewMode.Edit
      End If
      Me.AssignProjectButton.Visible = Not obj.IsNew

    Else
      Me.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly
      Me.AssignProjectButton.Visible = False
    End If
    Me.DetailsView1.Rows(Me.DetailsView1.Rows.Count - 1).Visible = Resource.CanSaveObject

    ' resources display
    Me.GridView1.Columns(Me.GridView1.Columns.Count - 1).Visible = Resource.CanSaveObject

  End Sub

#Region " Resource DetailsView "

  Protected Sub DetailsView1_ItemInserted(ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) _
    Handles DetailsView1.ItemInserted

    ApplyAuthorizationRules()

  End Sub

  Protected Sub DetailsView1_ItemUpdated(ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) _
    Handles DetailsView1.ItemUpdated

    ApplyAuthorizationRules()

  End Sub

  Protected Sub DetailsView1_ItemDeleted(ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) _
    Handles DetailsView1.ItemDeleted

    Response.Redirect("ResourceList.aspx")

  End Sub

#End Region

#Region " Project Grid "

  Protected Sub AssignProjectButton_Click(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles AssignProjectButton.Click

    Me.MultiView1.ActiveViewIndex = Views.AssignView

  End Sub

  Protected Sub GridView2_SelectedIndexChanged(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles GridView2.SelectedIndexChanged

    Dim obj As Resource = CType(Session("currentObject"), Resource)
    Try
      obj.Assignments.AssignTo(New Guid(Me.GridView2.SelectedDataKey.Value.ToString))
      If SaveResource(obj) > 0 Then
        Me.GridView1.DataBind()
        Me.MultiView1.ActiveViewIndex = Views.MainView
      End If

    Catch ex As InvalidOperationException
      ErrorLabel.Text = ex.Message
    End Try

  End Sub

  Protected Sub CancelAssignButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelAssignButton.Click

    Me.MultiView1.ActiveViewIndex = Views.MainView

  End Sub

#End Region

#Region " RoleListDataSource "

  Protected Sub RoleListDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles RoleListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.RoleList.GetList

  End Sub

#End Region

#Region " ResourceDataSource "

  Protected Sub ResourceDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles ResourceDataSource.DeleteObject

    ProjectTracker.Library.Resource.DeleteResource(CInt(e.Keys("Id")))
    Session.Remove("currentObject")
    e.RowsAffected = 1

  End Sub

  Protected Sub ResourceDataSource_InsertObject(ByVal sender As Object, _
    ByVal e As Csla.Web.InsertObjectArgs) Handles ResourceDataSource.InsertObject

    Dim obj As ProjectTracker.Library.Resource = _
      ProjectTracker.Library.Resource.NewResource
    Csla.Data.DataMapper.Map(e.Values, obj, "Id")
    e.RowsAffected = SaveResource(obj)

  End Sub

  Protected Sub ResourceDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles ResourceDataSource.SelectObject

    e.BusinessObject = Session("currentObject")

  End Sub

  Protected Sub ResourceDataSource_UpdateObject(ByVal sender As Object, _
    ByVal e As Csla.Web.UpdateObjectArgs) Handles ResourceDataSource.UpdateObject

    Dim obj As ProjectTracker.Library.Resource = _
      CType(Session("currentObject"), ProjectTracker.Library.Resource)
    Csla.Data.DataMapper.Map(e.Values, obj)
    e.RowsAffected = SaveResource(obj)

  End Sub

#End Region

#Region " AssignmentsDataSource "

  Protected Sub AssignmentsDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles AssignmentsDataSource.DeleteObject

    Dim obj As ProjectTracker.Library.Resource = _
      CType(Session("currentObject"), ProjectTracker.Library.Resource)
    Dim res As ProjectTracker.Library.ResourceAssignment
    Dim rid As New Guid(e.Keys("ProjectId").ToString)
    res = obj.Assignments(rid)
    obj.Assignments.Remove(res.ProjectId)
    e.RowsAffected = SaveResource(obj)

  End Sub

  Protected Sub AssignmentsDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles AssignmentsDataSource.SelectObject

    Dim obj As ProjectTracker.Library.Resource = _
      CType(Session("currentObject"), ProjectTracker.Library.Resource)
    e.BusinessObject = obj.Assignments

  End Sub

  Protected Sub AssignmentsDataSource_UpdateObject(ByVal sender As Object, _
    ByVal e As Csla.Web.UpdateObjectArgs) Handles AssignmentsDataSource.UpdateObject

    Dim obj As ProjectTracker.Library.Resource = _
      CType(Session("currentObject"), ProjectTracker.Library.Resource)
    Dim res As ProjectTracker.Library.ResourceAssignment
    Dim rid As New Guid(e.Keys("ProjectId").ToString)
    res = obj.Assignments(rid)
    Csla.Data.DataMapper.Map(e.Values, res)
    e.RowsAffected = SaveResource(obj)

  End Sub

#End Region

#Region " ProjectListDataSource "

  Protected Sub ProjectListDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles ProjectListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.ProjectList.GetProjectList

  End Sub

#End Region

  Private Function SaveResource(ByVal resource As Resource) As Integer

    Dim rowsAffected As Integer
    Try
      Session("currentObject") = resource.Save()
      rowsAffected = 1

    Catch ex As Csla.DataPortalException
      Me.ErrorLabel.Text = ex.BusinessException.Message
      rowsAffected = 0

    Catch ex As Exception
      Me.ErrorLabel.Text = ex.Message
      rowsAffected = 0
    End Try
    Return rowsAffected

  End Function

End Class
