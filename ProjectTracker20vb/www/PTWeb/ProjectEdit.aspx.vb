Option Strict On

Imports ProjectTracker.Library

Partial Class ProjectEdit
  Inherits System.Web.UI.Page

  Private Enum Views
    MainView = 0
    AssignView = 1
  End Enum

  Protected Sub Page_Load( _
    ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    If Not IsPostBack Then
      ApplyAuthorizationRules()

    Else
      Me.ErrorLabel.Text = ""
    End If

  End Sub

  Private Sub ApplyAuthorizationRules()

    ' project display
    If Project.CanEditObject Then
      Dim obj As Project = GetProject()
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
    Me.DetailsView1.Rows(Me.DetailsView1.Rows.Count - 1).Visible = _
      Project.CanEditObject

    ' resources display
    Me.GridView1.Columns(Me.GridView1.Columns.Count - 1).Visible = _
      Project.CanEditObject

  End Sub

#Region " Project DetailsView "

  Protected Sub DetailsView1_ItemInserted( _
    ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) _
    Handles DetailsView1.ItemInserted

    Dim project As Project = GetProject()
    Response.Redirect("ProjectEdit.aspx?id=" & project.Id.ToString)

  End Sub

  Protected Sub DetailsView1_ItemUpdated( _
    ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) _
    Handles DetailsView1.ItemUpdated

    ApplyAuthorizationRules()

  End Sub

  Protected Sub DetailsView1_ItemDeleted( _
    ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) _
    Handles DetailsView1.ItemDeleted

    Response.Redirect("ProjectList.aspx")

  End Sub

#End Region

#Region " Resource Grid "

  Protected Sub AddResourceButton_Click( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles AddResourceButton.Click

    Me.MultiView1.ActiveViewIndex = Views.AssignView

  End Sub

  Protected Sub GridView2_SelectedIndexChanged( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles GridView2.SelectedIndexChanged

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

  Protected Sub CancelAssignButton_Click( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles CancelAssignButton.Click

    Me.MultiView1.ActiveViewIndex = Views.MainView

  End Sub

#End Region

#Region " ProjectDataSource "

  Protected Sub ProjectDataSource_DeleteObject( _
    ByVal sender As Object, ByVal e As Csla.Web.DeleteObjectArgs) _
    Handles ProjectDataSource.DeleteObject

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

  Protected Sub ProjectDataSource_InsertObject( _
    ByVal sender As Object, ByVal e As Csla.Web.InsertObjectArgs) _
    Handles ProjectDataSource.InsertObject

    Dim obj As Project = GetProject()
    Csla.Data.DataMapper.Map(e.Values, obj, "Id")
    e.RowsAffected = SaveProject(obj)

  End Sub

  Protected Sub ProjectDataSource_SelectObject( _
    ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) _
    Handles ProjectDataSource.SelectObject

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

  Protected Sub ResourcesDataSource_DeleteObject( _
    ByVal sender As Object, ByVal e As Csla.Web.DeleteObjectArgs) _
    Handles ResourcesDataSource.DeleteObject

    Dim obj As Project = GetProject()
    Dim rid As Integer = CInt(e.Keys("ResourceId"))
    obj.Resources.Remove(rid)
    e.RowsAffected = SaveProject(obj)

  End Sub

  Protected Sub ResourcesDataSource_SelectObject( _
    ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) _
    Handles ResourcesDataSource.SelectObject

    Dim obj As Project = GetProject()
    e.BusinessObject = obj.Resources

  End Sub

  Protected Sub ResourcesDataSource_UpdateObject( _
    ByVal sender As Object, ByVal e As Csla.Web.UpdateObjectArgs) _
    Handles ResourcesDataSource.UpdateObject

    Dim obj As Project = GetProject()
    Dim rid As Integer = CInt(e.Keys("ResourceId"))
    Dim res As ProjectResource = obj.Resources.GetItem(rid)
    Csla.Data.DataMapper.Map(e.Values, res)
    e.RowsAffected = SaveProject(obj)

  End Sub

#End Region

#Region " ResourceListDataSource "

  Protected Sub ResourceListDataSource_SelectObject( _
    ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) _
    Handles ResourceListDataSource.SelectObject

    e.BusinessObject = ProjectTracker.Library.ResourceList.GetResourceList

  End Sub

#End Region

#Region " RoleListDataSource "

  Protected Sub RoleListDataSource_SelectObject( _
    ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) _
    Handles RoleListDataSource.SelectObject

    e.BusinessObject = RoleList.GetList

  End Sub

#End Region

  Private Function GetProject() As Project

    Dim businessObject As Object = Session("currentObject")
    If businessObject Is Nothing OrElse _
        Not TypeOf businessObject Is Project Then
      Try
        Dim idString As String = Request.QueryString("id")
        If Not String.IsNullOrEmpty(idString) Then
          Dim id As New Guid(idString)
          businessObject = Project.GetProject(id)

        Else
          businessObject = Project.NewProject
        End If
        Session("currentObject") = businessObject

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

    Catch ex As Csla.Validation.ValidationException
      Dim message As New System.Text.StringBuilder
      message.AppendFormat("{0}<br/>", ex.Message)
      If project.BrokenRulesCollection.Count = 1 Then
        message.AppendFormat("* {0}: {1}", _
          project.BrokenRulesCollection(0).Property, _
          project.BrokenRulesCollection(0).Description)

      Else
        For Each rule As Csla.Validation.BrokenRule In _
            project.BrokenRulesCollection
          message.AppendFormat( _
            "* {0}: {1}<br/>", rule.Property, rule.Description)
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
