Public Class Resources
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents HyperLink1 As System.Web.UI.WebControls.HyperLink
  Protected WithEvents dgResources As System.Web.UI.WebControls.DataGrid
  Protected WithEvents btnNewResource As System.Web.UI.WebControls.LinkButton

  'NOTE: The following placeholder declaration is required by the Web Form Designer.
  'Do not delete or move it.
  Private designerPlaceholderDeclaration As System.Object

  Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
    'CODEGEN: This method call is required by the Web Form Designer
    'Do not modify it using the code editor.
    InitializeComponent()
  End Sub

#End Region

  Private Sub Page_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

    dgResources.DataSource = ResourceList.GetResourceList
    DataBind()

    ' set security
    btnNewResource.Visible = _
      User.IsInRole("Supervisor") OrElse _
      User.IsInRole("ProjectManager")
    dgResources.Columns(2).Visible = _
      User.IsInRole("Supervisor") OrElse _
      User.IsInRole("ProjectManager") OrElse _
      User.IsInRole("Administrator")

  End Sub

  Private Sub dgResources_SelectedIndexChanged(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles dgResources.SelectedIndexChanged

    Dim id As String = dgResources.SelectedItem.Cells(0).Text
    Session("Resource") = Resource.GetResource(id)
    Response.Redirect("ResourceEdit.aspx")

  End Sub

  Private Sub dgResources_DeleteCommand(ByVal source As Object, _
      ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
      Handles dgResources.DeleteCommand

    Dim id As String = e.Item.Cells(0).Text
    Resource.DeleteResource(id)

    dgResources.DataSource = ResourceList.GetResourceList
    DataBind()

  End Sub

  Private Sub btnNewResource_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnNewResource.Click

    ' make sure there's no active resource so ResourceEdit knows
    ' to add a new object
    Session.Remove("Resource")
    Response.Redirect("ResourceEdit.aspx")

  End Sub

End Class
