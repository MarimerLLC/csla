Public Class AssignResource
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents dgResources As System.Web.UI.WebControls.DataGrid
  Protected WithEvents btnCancel As System.Web.UI.WebControls.Button
  Protected WithEvents lstRoles As System.Web.UI.WebControls.ListBox

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

    ' check security
    If Not User.IsInRole("ProjectManager") Then
      ' they should not be here
      Response.Redirect("ProjectEdit.aspx")
    End If

    If Not Page.IsPostBack Then
      Dim role As String
      For Each role In Assignment.Roles
        lstRoles.Items.Add(Assignment.Roles.Item(role))
      Next
      If lstRoles.Items.Count > 0 Then lstRoles.SelectedIndex = 0
    End If

    dgResources.DataSource = ResourceList.GetResourceList
    dgResources.DataBind()

  End Sub

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnCancel.Click

    Response.Redirect("ProjectEdit.aspx")

  End Sub

  Private Sub dgResources_SelectedIndexChanged(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles dgResources.SelectedIndexChanged

    Dim project As Project = Session("Project")
    Dim id As String = dgResources.SelectedItem.Cells(0).Text

    ' TODO: this line only works in 1.1, so is replaced with next line for 1.0
    project.Resources.Assign(id, lstRoles.SelectedValue)
    'project.Resources.Assign(id, lstRoles.SelectedItem.Value)

    Response.Redirect("ProjectEdit.aspx")

  End Sub

End Class
