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
      Try
        Dim idString As String = Request.QueryString("id")
        Dim obj As Project
        If Len(idString) > 0 Then
          Dim id As Guid
          id = New Guid(idString)
          obj = Project.GetProject(id)

        Else
          obj = Project.NewProject
        End If
        Session("currentObject") = obj
        Me.MultiView1.ActiveViewIndex = Views.MainView
        ApplyAuthorizationRules()

      Catch ex As System.Security.SecurityException
        Response.Redirect("Login.aspx?ReturnUrl=ProjectEdit.aspx")
      End Try
    End If

  End Sub

  Private Sub ApplyAuthorizationRules()

    Dim obj As Project = CType(Session("currentObject"), Project)
    ' project display
    If Project.CanSaveObject Then
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
    Me.DetailsView1.Rows(Me.DetailsView1.Rows.Count - 1).Visible = Project.CanSaveObject

    ' resources display
    Me.GridView1.Columns(Me.GridView1.Columns.Count - 1).Visible = Project.CanSaveObject

  End Sub

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

#Region " Add Resource "

  Protected Sub AddResourceButton_Click(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles AddResourceButton.Click

    Me.MultiView1.ActiveViewIndex = Views.AssignView

  End Sub

  Protected Sub GridView2_SelectedIndexChanged(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles GridView2.SelectedIndexChanged

    Try
      Dim obj As Project = CType(Session("currentObject"), Project)
      obj.Resources.Assign(CInt(Me.GridView2.SelectedDataKey.Value))
      Session("currentObject") = obj.Save()
      Me.GridView1.DataBind()
      Me.MultiView1.ActiveViewIndex = Views.MainView
      ErrorLabel.Text = ""

    Catch ex As InvalidOperationException
      ErrorLabel.Text = ex.Message

    Catch ex As Csla.DataPortalException
      ErrorLabel.Text = ex.InnerException.Message
    End Try


  End Sub

  Protected Sub CancelAssignButton_Click(ByVal sender As Object, _
    ByVal e As System.EventArgs) Handles CancelAssignButton.Click

    Me.MultiView1.ActiveViewIndex = Views.MainView
    ErrorLabel.Text = ""

  End Sub

#End Region

#Region " ProjectDataSource "

  Protected Sub ProjectDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles ProjectDataSource.DeleteObject

    Project.DeleteProject(New Guid(e.Keys("Id").ToString))
    Session("currentObject") = Nothing
    e.RowsAffected = 1

  End Sub

  Protected Sub ProjectDataSource_InsertObject(ByVal sender As Object, _
    ByVal e As Csla.Web.InsertObjectArgs) Handles ProjectDataSource.InsertObject

    Dim obj As Project = CType(Session("currentObject"), Project)
    Csla.Data.DataMapper.Map(e.Values, obj, "Id")
    Session("currentObject") = obj.Save()
    e.RowsAffected = 1

  End Sub

  Protected Sub ProjectDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles ProjectDataSource.SelectObject

    e.BusinessObject = Session("currentObject")

  End Sub

  Protected Sub ProjectDataSource_UpdateObject(ByVal sender As Object, _
    ByVal e As Csla.Web.UpdateObjectArgs) Handles ProjectDataSource.UpdateObject

    Dim obj As Project = CType(Session("currentObject"), Project)
    Csla.Data.DataMapper.Map(e.Values, obj)
    Session("currentObject") = obj.Save()
    e.RowsAffected = 1

  End Sub

#End Region

#Region " ResourcesDataSource "

  Protected Sub ResourcesDataSource_DeleteObject(ByVal sender As Object, _
    ByVal e As Csla.Web.DeleteObjectArgs) Handles ResourcesDataSource.DeleteObject

    Dim obj As Project = CType(Session("currentObject"), Project)
    Dim res As ProjectResource
    Dim rid As Integer = CInt(e.Keys("ResourceId"))
    res = obj.Resources(rid)
    obj.Resources.Remove(res.ResourceId)
    Session("currentObject") = obj.Save()
    e.RowsAffected = 1

  End Sub

  Protected Sub ResourcesDataSource_SelectObject(ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) Handles ResourcesDataSource.SelectObject

    Dim obj As Project = CType(Session("currentObject"), Project)
    e.BusinessObject = obj.Resources

  End Sub

  Protected Sub ResourcesDataSource_UpdateObject(ByVal sender As Object, ByVal e As Csla.Web.UpdateObjectArgs) Handles ResourcesDataSource.UpdateObject

    Dim obj As Project = CType(Session("currentObject"), Project)
    Dim res As ProjectResource
    Dim rid As Integer = CInt(e.Keys("ResourceId"))
    res = obj.Resources(rid)
    Csla.Data.DataMapper.Map(e.Values, res)
    Session("currentObject") = obj.Save()
    e.RowsAffected = 1

  End Sub

#End Region

#Region " ResourceListDataSource "

  Protected Sub ResourceListDataSource_SelectObject(ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) Handles ResourceListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.ResourceList.GetResourceList

  End Sub

#End Region

#Region " RoleListDataSource "

  Protected Sub RoleListDataSource_SelectObject(ByVal sender As Object, _
    ByVal e As Csla.Web.SelectObjectArgs) Handles RoleListDataSource.SelectObject

    e.BusinessObject = RoleList.GetList

  End Sub

#End Region

End Class
