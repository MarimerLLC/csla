Public Class ProjectEdit
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents btnSave As System.Web.UI.WebControls.Button
  Protected WithEvents HyperLink1 As System.Web.UI.WebControls.HyperLink
  Protected WithEvents txtID As System.Web.UI.WebControls.TextBox
  Protected WithEvents txtName As System.Web.UI.WebControls.TextBox
  Protected WithEvents txtStarted As System.Web.UI.WebControls.TextBox
  Protected WithEvents txtEnded As System.Web.UI.WebControls.TextBox
  Protected WithEvents txtDescription As System.Web.UI.WebControls.TextBox
  Protected WithEvents RequiredFieldValidator1 As System.Web.UI.WebControls.RequiredFieldValidator
  Protected WithEvents CompareValidator1 As System.Web.UI.WebControls.CompareValidator
  Protected WithEvents CompareValidator2 As System.Web.UI.WebControls.CompareValidator
  Protected WithEvents HyperLink3 As System.Web.UI.WebControls.HyperLink
  Protected WithEvents dgResources As System.Web.UI.WebControls.DataGrid
  Protected WithEvents btnCancel As System.Web.UI.WebControls.Button
  Protected WithEvents btnAssignResource As System.Web.UI.WebControls.LinkButton
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

  ' we make this a page-level variable and then
  ' always load it in Page_Load. That way the object
  ' is available to all our code at all times
  Protected mProject As Project

  Private Sub Page_Load(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load

    ' get the existing Project (if any)
    mProject = Session("Project")

    If mProject Is Nothing Then
      ' got here via either direct nav or back button
      If Len(txtID.Text) = 0 Then
        ' got here via direct nav
        ' we are creating a new project
        mProject = Project.NewProject

      Else
        ' we've returned here via the Browser's back button
        ' load object based on ID value
        mProject = Project.GetProject(New Guid(txtID.Text))
      End If
      Session("Project") = mProject
    End If

    ' bind the grid every time (we're not using
    ' viewstate to keep it populated)
    dgResources.DataSource = mProject.Resources

    If IsPostBack Then
      dgResources.DataBind()

    Else
      DataBind()
    End If

    ' set security
    If Not User.IsInRole("ProjectManager") Then
      btnNewProject.Visible = False
      btnSave.Visible = False
      btnAssignResource.Visible = False
      dgResources.Columns(5).Visible = False
    End If

  End Sub

  Private Sub dgResources_ItemCommand(ByVal source As Object, _
      ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
      Handles dgResources.ItemCommand

    If e.CommandName = "SelectRole" Then
      Dim id As String = e.Item.Cells(0).Text
      Session("ProjectResource") = mProject.Resources(id)
      Session("Source") = "ProjectEdit.aspx"
      Response.Redirect("ChooseRole.aspx")
    End If

  End Sub

  Private Sub dgResources_DeleteCommand(ByVal source As Object, _
      ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
      Handles dgResources.DeleteCommand

    Dim id As String = e.Item.Cells(0).Text
    mProject.Resources.Remove(id)

    ' rebind grid to update display
    dgResources.DataSource = mProject.Resources
    dgResources.DataBind()

  End Sub

  Private Sub btnSave_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnSave.Click

    SaveFormToObject()
    mProject = mProject.Save
    Session.Remove("Project")
    Response.Redirect("Projects.aspx")

  End Sub

  Private Sub dgResources_SelectedIndexChanged(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles dgResources.SelectedIndexChanged

    ' check security
    If User.IsInRole("ProjectManager") Then
      ' only do save if user is in a valid role
      SaveFormToObject()
      mProject = mProject.Save
    End If

    Session.Remove("Project")

    Dim id As String = dgResources.SelectedItem.Cells(0).Text
    Session("Resource") = Resource.GetResource(id)
    Response.Redirect("ResourceEdit.aspx")

  End Sub

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnCancel.Click

    Session.Remove("Project")
    Response.Redirect("Projects.aspx")

  End Sub

  Private Sub btnAssignResource_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnAssignResource.Click

    SaveFormToObject()
    Response.Redirect("AssignResource.aspx")

  End Sub

  Private Sub SaveFormToObject()

    With mProject
      .Name = txtName.Text
      .Started = txtStarted.Text
      .Ended = txtEnded.Text
      .Description = txtDescription.Text
    End With

  End Sub

  Private Sub btnNewProject_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnNewProject.Click

    Session("Project") = Project.NewProject
    Response.Redirect("ProjectEdit.aspx")

  End Sub

End Class
