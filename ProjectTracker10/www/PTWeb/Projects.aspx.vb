Public Class Projects
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents dgProjects As System.Web.UI.WebControls.DataGrid
  Protected WithEvents HyperLink2 As System.Web.UI.WebControls.HyperLink
  Protected WithEvents btnNewProject As System.Web.UI.WebControls.LinkButton

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

    dgProjects.DataSource = ProjectList.GetProjectList()
    DataBind()

    ' set security
    btnNewProject.Visible = _
      User.IsInRole("ProjectManager")
    dgProjects.Columns(2).Visible = _
      user.IsInRole("ProjectManager") OrElse _
      user.IsInRole("Administrator")

  End Sub

  Private Sub dgProjects_SelectedIndexChanged(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles dgProjects.SelectedIndexChanged

    Dim id As Guid = New Guid(dgProjects.SelectedItem.Cells(0).Text)
    Session("Project") = Project.GetProject(id)
    Response.Redirect("ProjectEdit.aspx")

  End Sub

  Private Sub dgProjects_DeleteCommand(ByVal source As Object, _
      ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
      Handles dgProjects.DeleteCommand

    Dim id As New Guid(e.Item.Cells(0).Text)
    Project.DeleteProject(id)

    dgProjects.DataSource = ProjectList.GetProjectList()
    DataBind()

  End Sub

  Private Sub btnNewProject_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnNewProject.Click

    Session("Project") = Project.NewProject
    Response.Redirect("ProjectEdit.aspx")

  End Sub

End Class
