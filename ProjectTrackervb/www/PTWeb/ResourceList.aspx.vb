Option Strict On

Imports ProjectTracker.Library

Partial Class ResourceList
  Inherits System.Web.UI.Page

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    If Not IsPostBack Then
      Session("currentObject") = Nothing
      ApplyAuthorizationRules()

    Else
      Me.ErrorLabel.Text = ""
    End If

  End Sub

  Private Sub ApplyAuthorizationRules()

    Me.GridView1.Columns( _
      Me.GridView1.Columns.Count - 1).Visible = _
      Resource.CanDeleteObject
    Me.NewResourceButton.Visible = _
      ProjectTracker.Library.Resource.CanAddObject

  End Sub

#Region " GridView1 "

  Protected Sub NewResourceButton_Click( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles NewResourceButton.Click

    Response.Redirect("ResourceEdit.aspx")

  End Sub

  Protected Sub GridView1_RowDeleted( _
    ByVal sender As Object, _
    ByVal e As System.Web.UI.WebControls.GridViewDeletedEventArgs) _
    Handles GridView1.RowDeleted

    Session("currentObject") = Nothing
    GridView1.DataBind()

  End Sub

#End Region

#Region " ResourceListDataSource "

  Protected Sub ResourceListDataSource_DeleteObject( _
    ByVal sender As Object, ByVal e As Csla.Web.DeleteObjectArgs) _
    Handles ResourceListDataSource.DeleteObject

    ProjectTracker.Library.Resource.DeleteResource(CInt(e.Keys("Id")))

  End Sub

  Protected Sub ResourceListDataSource_SelectObject( _
    ByVal sender As Object, ByVal e As Csla.Web.SelectObjectArgs) _
    Handles ResourceListDataSource.SelectObject

    e.BusinessObject = GetResourceList()

  End Sub

#End Region

  Private Function GetResourceList() As ProjectTracker.Library.ResourceList

    Dim businessObject As Object = Session("currentObject")
    If businessObject Is Nothing OrElse Not TypeOf businessObject Is ResourceList Then
      businessObject = ProjectTracker.Library.ResourceList.GetResourceList
      Session("currentObject") = businessObject
    End If
    Return CType(businessObject, ProjectTracker.Library.ResourceList)

  End Function

End Class
