Public Class ResourceEdit
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents HyperLink1 As System.Web.UI.WebControls.HyperLink
  Protected WithEvents HyperLink2 As System.Web.UI.WebControls.HyperLink
  Protected WithEvents btnSave As System.Web.UI.WebControls.Button
  Protected WithEvents btnCancel As System.Web.UI.WebControls.Button
  Protected WithEvents txtID As System.Web.UI.WebControls.TextBox
  Protected WithEvents txtFirstname As System.Web.UI.WebControls.TextBox
  Protected WithEvents txtLastname As System.Web.UI.WebControls.TextBox
  Protected WithEvents dgProjects As System.Web.UI.WebControls.DataGrid
  Protected WithEvents btnAssign As System.Web.UI.WebControls.LinkButton
  Protected WithEvents RequiredFieldValidator1 As System.Web.UI.WebControls.RequiredFieldValidator
  Protected WithEvents RequiredFieldValidator2 As System.Web.UI.WebControls.RequiredFieldValidator
  Protected WithEvents RequiredFieldValidator3 As System.Web.UI.WebControls.RequiredFieldValidator
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

  ' we make this a page-level variable and then
  ' always load it in Page_Load. That way the object
  ' is available to all our code at all times
  Protected mResource As Resource

  Private Sub Page_Load(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load

    ' get the existing Resource (if any)
    mResource = Session("Resource")

    If mResource Is Nothing Then
      ' either we're adding a new object or the user hit the back button
      If Len(txtID.Text) = 0 Then
        ' we are adding a new resource
        txtID.ReadOnly = False
        btnAssign.Visible = False

      Else
        If txtID.ReadOnly Then
          ' we've returned here via the Browser's back button
          ' load object based on ID value
          mResource = Resource.GetResource(txtID.Text)
          Session("Resource") = mResource

        Else
          ' we are adding a new resource
          mResource = Resource.NewResource(txtID.Text)
          Session("Resource") = mResource
          txtID.ReadOnly = True
          btnAssign.Visible = True
        End If
      End If
    End If

    If Not mResource Is Nothing Then
      ' we have a resource to which we can bind
      dgProjects.DataSource = mResource.Assignments

      If IsPostBack Then
        dgProjects.DataBind()

      Else
        DataBind()
      End If
    End If

    ' set security
    If Not User.IsInRole("Supervisor") AndAlso _
       Not User.IsInRole("ProjectManager") Then
      btnNewResource.Visible = False
      btnSave.Visible = False
      btnAssign.Visible = False
      dgProjects.Columns(4).Visible = False
    End If

  End Sub

  Private Sub dgProjects_DeleteCommand(ByVal source As Object, _
      ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
      Handles dgProjects.DeleteCommand

    Dim id As String = e.Item.Cells(0).Text
    mResource.Assignments.Remove(New Guid(id))

    ' rebind grid to update display
    dgProjects.DataSource = mResource.Assignments
    dgProjects.DataBind()

  End Sub

  Private Sub btnSave_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnSave.Click

    SaveFormToObject()

    If mResource.IsNew Then
      mResource = mResource.Save
      Session("Resource") = mResource
      Response.Redirect("ResourceEdit.aspx")

    Else
      mResource.Save()
      Session.Remove("Resource")
      Response.Redirect("Resources.aspx")
    End If

  End Sub

  Private Sub dgProjects_SelectedIndexChanged(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles dgProjects.SelectedIndexChanged

    ' check security
    If User.IsInRole("Supervisor") OrElse _
       User.IsInRole("ProjectManager") Then
      ' only do save if user is in a valid role
      SaveFormToObject()
      mResource = mResource.Save
    End If

    Session.Remove("Resource")

    Dim id As New Guid(dgProjects.SelectedItem.Cells(0).Text)
    Session("Project") = Project.GetProject(id)
    Response.Redirect("ProjectEdit.aspx")

  End Sub

  Private Sub dgProjects_ItemCommand(ByVal source As Object, _
      ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) _
      Handles dgProjects.ItemCommand

    If e.CommandName = "SelectRole" Then
      Dim id As New Guid(e.Item.Cells(0).Text)
      Session("ResourceAssignment") = mResource.Assignments(id)
      Session("Source") = "ResourceEdit.aspx"
      Response.Redirect("ChooseRole.aspx")
    End If

  End Sub

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnCancel.Click

    Session.Remove("Resource")
    Response.Redirect("Resources.aspx")

  End Sub

  Private Sub btnAssign_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnAssign.Click

    SaveFormToObject()
    Response.Redirect("AssignToProject.aspx")

  End Sub

  Private Sub SaveFormToObject()

    With mResource
      .FirstName = txtFirstname.Text
      .LastName = txtLastname.Text
    End With

  End Sub

  Private Sub btnNewResource_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnNewResource.Click

    Session.Remove("Resource")
    Response.Redirect("ResourceEdit.aspx")

  End Sub

End Class
