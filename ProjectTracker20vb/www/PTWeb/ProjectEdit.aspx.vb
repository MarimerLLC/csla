Option Strict On

Imports ProjectTracker.Library

Partial Class ProjectEdit
  Inherits System.Web.UI.Page

  Private Enum Views
    MainView = 0
    AssignView = 1
  End Enum

  Protected Sub Page_Load(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles Me.Load

    If Not IsPostBack Then
      ApplyAuthorizationRules()

    Else
      Me.ErrorLabel.Text = ""
    End If

  End Sub

  Private Sub ApplyAuthorizationRules()

    Dim obj As Project = GetProject()
    ' project display
    If Project.CanEditObject Then
      If obj.IsNew Then
        Me.DetailsView1.DefaultMode = DetailsViewMode.Insert

      Else
        Me.DetailsView1.DefaultMode = DetailsViewMode.Edit
      End If
      Me.AddResourceButton.Visible = Not obj.IsNew

    Else
      Me.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly
      Me.AddResourceButton.Visible = False
    End If
    Me.DetailsView1.Rows(Me.DetailsView1.Rows.Count - 1).Visible = Project.CanEditObject

    ' resources display
    Me.GridView1.Columns(Me.GridView1.Columns.Count - 1).Visible = Project.CanEditObject

  End Sub

#Region " Project DetailsView "

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

    Response.Redirect("ProjectList.aspx")

  End Sub

#End Region

#Region " Resource Grid "

  Protected Sub AddResourceButton_Click(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles AddResourceButton.Click

    Me.MultiView1.ActiveViewIndex = Views.AssignView

  End Sub

  Protected Sub GridView2_SelectedIndexChanged(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles GridView2.SelectedIndexChanged

    Dim obj As Project = GetProject()
    Try
      obj.Resources.Assign(CInt(Me.GridView2.SelectedDataKey.Value))
      If SaveProject(obj) > 0 Then
        Me.GridView1.DataBind()
        Me.MultiView1.ActiveViewIndex = Views.MainView
      End If

    Catch ex As InvalidOperationException
      ErrorLabel.Text = ex.Message
    End Try

  End Sub

  Protected Sub CancelAssignButton_Click(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles CancelAssignButton.Click

    Me.MultiView1.ActiveViewIndex = Views.MainView

  End Sub

#End Region

#Region " ProjectDataSource "

  Protected Sub ProjectDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles ProjectDataSource.DeleteObject

    Try
      Project.DeleteProject(New Guid(e.Keys("Id").ToString))
      Session("currentObject") = Nothing
      e.RowsAffected = 1

    Catch ex As Csla.DataPortalException
      Me.ErrorLabel.Text = ex.BusinessException.Message
      e.RowsAffected = 0

    Catch ex As Exception
      Me.ErrorLabel.Text = ex.Message
      e.RowsAffected = 0
    End Try

  End Sub

  Protected Sub ProjectDataSource_InsertObject(ByVal sender As Object, _
    ByVal e As Csla.Web.InsertObjectArgs) Handles ProjectDataSource.InsertObject

    Dim obj As Project = GetProject()
    Csla.Data.DataMapper.Map(e.Values, obj, "Id")
    e.RowsAffected = SaveProject(obj)

  End Sub

  Protected Sub ProjectDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles ProjectDataSource.SelectObject

    e.BusinessObject = GetProject()

  End Sub

  Protected Sub ProjectDataSource_UpdateObject(ByVal sender As Object, _
    ByVal e As Csla.Web.UpdateObjectArgs) Handles ProjectDataSource.UpdateObject

    Dim obj As Project = GetProject()
    Csla.Data.DataMapper.Map(e.Values, obj)
    e.RowsAffected = SaveProject(obj)

  End Sub

#End Region

#Region " ResourcesDataSource "

  Protected Sub ResourcesDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles ResourcesDataSource.DeleteObject

    Dim obj As Project = GetProject()
    Dim rid As Integer = CInt(e.Keys("ResourceId"))
    obj.Resources.Remove(rid)
    e.RowsAffected = SaveProject(obj)

  End Sub

  Protected Sub ResourcesDataSource_SelectObject(ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) Handles ResourcesDataSource.SelectObject

    Dim obj As Project = GetProject()
    e.BusinessObject = obj.Resources

  End Sub

  Protected Sub ResourcesDataSource_UpdateObject(ByVal sender As Object, ByVal e As Csla.Web.UpdateObjectArgs) Handles ResourcesDataSource.UpdateObject

    Dim obj As Project = GetProject()
    Dim res As ProjectResource
    Dim rid As Integer = CInt(e.Keys("ResourceId"))
    res = obj.Resources.GetItem(rid)
    Csla.Data.DataMapper.Map(e.Values, res)
    e.RowsAffected = SaveProject(obj)

  End Sub

#End Region

#Region " ResourceListDataSource "

  Protected Sub ResourceListDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles ResourceListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.ResourceList.GetResourceList

  End Sub

#End Region

#Region " RoleListDataSource "

  Protected Sub RoleListDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles RoleListDataSource.SelectObject

    e.BusinessObject = RoleList.GetList

  End Sub

#End Region

  Private Function GetProject() As Project

    Dim businessObject As Object = Session("currentObject")
    If businessObject Is Nothing OrElse Not TypeOf businessObject Is Project Then
      Try
        Dim idString As String = Request.QueryString("id")
        If Not String.IsNullOrEmpty(idString) Then
          Dim id As New Guid(idString)
          businessObject = Project.GetProject(id)
        Else
          businessObject = Project.NewProject
          Session("currentObject") = businessObject
        End If
      Catch ex As System.Security.SecurityException
        Response.Redirect("ProjectList.aspx")
      End Try
    End If
    Return CType(businessObject, Project)

  End Function

  Private Function SaveProject(ByVal project As Project) As Integer

    Dim rowsAffected As Integer
    Try
      Session("currentObject") = project.Save()
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
