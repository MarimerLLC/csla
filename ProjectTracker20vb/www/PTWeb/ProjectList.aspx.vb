Option Strict On

Imports ProjectTracker.Library

Partial Class ProjectList
  Inherits System.Web.UI.Page

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
      Me.GridView1.Columns.Count - 1).Visible = _
      Project.CanDeleteObject
    NewProjectButton.Visible = _
      ProjectTracker.Library.Project.CanAddObject

  End Sub

#Region " GridView1 "

  Protected Sub GridView1_RowDeleted(ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.GridViewDeletedEventArgs) _
    Handles GridView1.RowDeleted

    Session("currentObject") = Nothing
    GridView1.DataBind()

  End Sub

  Protected Sub NewProjectButton_Click( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles NewProjectButton.Click

    'allow user to add a new project
    Response.Redirect("ProjectEdit.aspx")

  End Sub

#End Region

#Region " ProjectListDataSource "

  Protected Sub ProjectListDataSource_DeleteObject( _
    ByVal sender As Object, ByVal e As Csla.Web.DeleteObjectArgs) _
    Handles ProjectListDataSource.DeleteObject

    Try
      ProjectTracker.Library.Project.DeleteProject( _
        New Guid(e.Keys("Id").ToString))
      e.RowsAffected = 1

    Catch ex As Csla.DataPortalException
      Me.ErrorLabel.Text = ex.BusinessException.Message
      e.RowsAffected = 0

    Catch ex As Exception
      Me.ErrorLabel.Text = ex.Message
      e.RowsAffected = 0
    End Try

  End Sub

  Protected Sub ProjectListDataSource_SelectObject( _
    ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) _
    Handles ProjectListDataSource.SelectObject

    e.BusinessObject = GetProjectList()

  End Sub

#End Region

  Private Function GetProjectList() As ProjectTracker.Library.ProjectList

    Dim businessObject As Object = Session("currentObject")
    If businessObject Is Nothing OrElse _
        Not TypeOf businessObject Is ProjectList Then
      businessObject = ProjectTracker.Library.ProjectList.GetProjectList
      Session("currentObject") = businessObject
    End If
    Return CType(businessObject, ProjectTracker.Library.ProjectList)

  End Function

End Class
