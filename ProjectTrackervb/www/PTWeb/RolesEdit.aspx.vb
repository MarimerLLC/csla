Option Strict On

Imports ProjectTracker.Library.Admin

Partial Class RolesEdit
  Inherits System.Web.UI.Page

  Private Enum Views
    MainView = 0
    InsertView = 1
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

    Me.GridView1.Columns( _
      Me.GridView1.Columns.Count - 1).Visible = Roles.CanEditObject
    Me.AddRoleButton.Visible = Roles.CanAddObject

  End Sub

  Protected Sub AddRoleButton_Click( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles AddRoleButton.Click

    Me.DetailsView1.DefaultMode = DetailsViewMode.Insert
    MultiView1.ActiveViewIndex = Views.InsertView

  End Sub

  Protected Sub DetailsView1_ItemInserted( _
    ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) _
    Handles DetailsView1.ItemInserted

    MultiView1.ActiveViewIndex = Views.MainView
    Me.GridView1.DataBind()

  End Sub

  Protected Sub DetailsView1_ModeChanged( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles DetailsView1.ModeChanged

    MultiView1.ActiveViewIndex = Views.MainView

  End Sub

#Region " RolesDataSource "

  Protected Sub RolesDataSource_DeleteObject( _
    ByVal sender As Object, ByVal e As Csla.Web.DeleteObjectArgs) _
    Handles RolesDataSource.DeleteObject

    Try
      Dim obj As Roles = GetRoles()
      Dim id As Integer = CInt(e.Keys.Item("Id"))
      obj.Remove(id)
      Session("currentObject") = obj.Save
      e.RowsAffected = 1

    Catch ex As Csla.DataPortalException
      Me.ErrorLabel.Text = ex.BusinessException.Message
      e.RowsAffected = 0

    Catch ex As Exception
      Me.ErrorLabel.Text = ex.Message
      e.RowsAffected = 0
    End Try

  End Sub

  Protected Sub RolesDataSource_InsertObject( _
    ByVal sender As Object, ByVal e As Csla.Web.InsertObjectArgs) _
    Handles RolesDataSource.InsertObject

    Try
      Dim obj As Roles = GetRoles()
      Dim role As Role = obj.AddNew
      Csla.Data.DataMapper.Map(e.Values, role)
      Session("currentObject") = obj.Save
      e.RowsAffected = 1

    Catch ex As Csla.DataPortalException
      Me.ErrorLabel.Text = ex.BusinessException.Message
      e.RowsAffected = 0

    Catch ex As Exception
      Me.ErrorLabel.Text = ex.Message
      e.RowsAffected = 0
    End Try

  End Sub

  Protected Sub RolesDataSource_SelectObject( _
    ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) _
    Handles RolesDataSource.SelectObject

    Dim obj As ProjectTracker.Library.Admin.Roles = GetRoles()
    e.BusinessObject = obj

  End Sub

  Protected Sub RolesDataSource_UpdateObject( _
    ByVal sender As Object, ByVal e As Csla.Web.UpdateObjectArgs) _
    Handles RolesDataSource.UpdateObject

    Try
      Dim obj As Roles = GetRoles()
      Dim role As Role = obj.GetRoleById(CInt(e.Keys.Item("Id")))
      role.Name = e.Values.Item("Name").ToString
      Session("currentObject") = obj.Save
      e.RowsAffected = 1

    Catch ex As Csla.DataPortalException
      Me.ErrorLabel.Text = ex.BusinessException.Message
      e.RowsAffected = 0

    Catch ex As Exception
      Me.ErrorLabel.Text = ex.Message
      e.RowsAffected = 0
    End Try

  End Sub

#End Region

  Private Function GetRoles() As ProjectTracker.Library.Admin.Roles

    Dim businessObject As Object = Session("currentObject")
    If businessObject Is Nothing OrElse _
      Not TypeOf businessObject Is ProjectTracker.Library.Admin.Roles Then
      businessObject = _
        ProjectTracker.Library.Admin.Roles.GetRoles
      Session("currentObject") = businessObject
    End If
    Return CType(businessObject, ProjectTracker.Library.Admin.Roles)

  End Function

End Class
