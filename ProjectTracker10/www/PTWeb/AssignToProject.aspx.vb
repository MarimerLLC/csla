Public Class AssignToProject
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents lstRoles As System.Web.UI.WebControls.ListBox
  Protected WithEvents dgProjects As System.Web.UI.WebControls.DataGrid
  Protected WithEvents btnCancel As System.Web.UI.WebControls.Button

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
    If Not User.IsInRole("Supervisor") AndAlso _
       Not User.IsInRole("ProjectManager") Then
      ' they should not be here
      Response.Redirect("ResourceEdit.aspx")
    End If

    If Not Page.IsPostBack Then
      Dim role As String
      For Each role In Assignment.Roles
        lstRoles.Items.Add(Assignment.Roles.Item(role))
      Next

      ' set the default role to the first in the list
      If lstRoles.Items.Count > 0 Then lstRoles.SelectedIndex = 0
    End If

    dgProjects.DataSource = ProjectList.GetProjectList
    dgProjects.DataBind()

  End Sub

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles btnCancel.Click

    Response.Redirect("ResourceEdit.aspx")

  End Sub

  Private Sub dgProjects_SelectedIndexChanged(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles dgProjects.SelectedIndexChanged

    Dim resource As Resource = Session("Resource")
    Dim id As Guid = New Guid(dgProjects.SelectedItem.Cells(0).Text)

    ' TODO: this line only works in 1.1, so is replaced with next line for 1.0
    resource.Assignments.AssignTo(id, lstRoles.SelectedValue)
    'resource.Assignments.AssignTo(id, lstRoles.SelectedItem.Value)

    Response.Redirect("ResourceEdit.aspx")

  End Sub

End Class
