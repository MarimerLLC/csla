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
      Session("currentObject") = Nothing
      ApplyAuthorizationRules()

    Else
      Me.ErrorLabel.Text = ""
    End If

  End Sub

  Private Sub ApplyAuthorizationRules()

    Dim obj As Resource = GetResource()
    ' Resource display
    If Resource.CanEditObject Then
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
    Me.DetailsView1.Rows(Me.DetailsView1.Rows.Count - 1).Visible = Resource.CanEditObject

    ' resources display
    Me.GridView1.Columns(Me.GridView1.Columns.Count - 1).Visible = Resource.CanEditObject

  End Sub

#Region " Resource DetailsView "

  Protected Sub DetailsView1_ItemInserted(ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) _
    Handles DetailsView1.ItemInserted

    Dim resource As Resource = GetResource()
    Response.Redirect("resourceEdit.aspx?id=" & resource.Id.ToString)

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

    Dim obj As Resource = GetResource()
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

    e.BusinessObject = GetResource()

  End Sub

  Protected Sub ResourceDataSource_UpdateObject(ByVal sender As Object, _
    ByVal e As Csla.Web.UpdateObjectArgs) Handles ResourceDataSource.UpdateObject

    Dim obj As ProjectTracker.Library.Resource = GetResource()
    Csla.Data.DataMapper.Map(e.Values, obj)
    e.RowsAffected = SaveResource(obj)

  End Sub

#End Region

#Region " AssignmentsDataSource "

  Protected Sub AssignmentsDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles AssignmentsDataSource.DeleteObject

    Dim obj As ProjectTracker.Library.Resource = GetResource()
    Dim rid As New Guid(e.Keys("ProjectId").ToString)
    obj.Assignments.Remove(rid)
    e.RowsAffected = SaveResource(obj)

  End Sub

  Protected Sub AssignmentsDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles AssignmentsDataSource.SelectObject

    Dim obj As ProjectTracker.Library.Resource = GetResource()
    e.BusinessObject = obj.Assignments

  End Sub

  Protected Sub AssignmentsDataSource_UpdateObject(ByVal sender As Object, _
    ByVal e As Csla.Web.UpdateObjectArgs) Handles AssignmentsDataSource.UpdateObject

    Dim obj As ProjectTracker.Library.Resource = GetResource()
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

  Private Function GetResource() As Resource

    Dim businessObject As Object = Session("currentObject")
    If businessObject Is Nothing OrElse Not TypeOf businessObject Is Resource Then
      Try
        Dim idString As String = Request.QueryString("id")
        If Not String.IsNullOrEmpty(idString) Then
          Dim id As Integer = CInt(idString)
          businessObject = Resource.GetResource(id)
        Else
          businessObject = Resource.NewResource
          Session("currentObject") = businessObject
        End If
      Catch ex As System.Security.SecurityException
        Response.Redirect("ResourceList.aspx")
      End Try
    End If
    Return CType(businessObject, Resource)

  End Function

  Private Function SaveResource(ByVal resource As Resource) As Integer

    Dim rowsAffected As Integer
    Try
      Session("currentObject") = resource.Save()
      rowsAffected = 1

    Catch ex As Csla.Validation.ValidationException
      Dim message As New System.Text.StringBuilder
      message.AppendFormat("{0}<br/>", ex.Message)
      If resource.BrokenRulesCollection.Count = 1 Then
        message.AppendFormat("* {0}: {1}", _
          resource.BrokenRulesCollection(0).Property, _
          resource.BrokenRulesCollection(0).Description)

      Else
        For Each rule As Csla.Validation.BrokenRule In resource.BrokenRulesCollection
          message.AppendFormat("* {0}: {1}<br/>", rule.Property, rule.Description)
        Next
      End If
      Me.ErrorLabel.Text = message.ToString
      rowsAffected = 0

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
