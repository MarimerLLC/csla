Public Class ChooseRole
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents lblLabel As System.Web.UI.WebControls.Label
  Protected WithEvents lblValue As System.Web.UI.WebControls.Label
  Protected WithEvents lstRoles As System.Web.UI.WebControls.ListBox
  Protected WithEvents btnUpdate As System.Web.UI.WebControls.Button
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

    If Not Page.IsPostBack Then
      Dim role As String
      For Each role In Assignment.Roles
        lstRoles.Items.Add(Assignment.Roles.Item(role))
      Next

      If Session("Source") = "ProjectEdit.aspx" Then
        ' we are dealing with a ProjectResource

        ' check security
        If Not User.IsInRole("ProjectManager") Then
          ' they should not be here
          SendUserBack()
        End If

        Dim obj As ProjectResource = Session("ProjectResource")
        lblLabel.Text = "Resource"
        lblValue.Text = obj.FirstName & " " & obj.LastName

        ' TODO: this line only works in 1.1, so is replaced with next line for 1.0
        'lstRoles.SelectedValue = obj.Role
        SelectItem(lstRoles, obj.Role)

      Else
        ' we are dealing with a ResourceAssignment

        ' check security
        If Not User.IsInRole("Supervisor") AndAlso _
           Not User.IsInRole("ProjectManager") Then
          ' they should not be here
          SendUserBack()
        End If

        Dim obj As ResourceAssignment = Session("ResourceAssignment")
        lblLabel.Text = "Project"
        lblValue.Text = obj.ProjectName

        ' TODO: this line only works in 1.1, so is replaced with next line for 1.0
        'lstRoles.SelectedValue = obj.Role
        SelectItem(lstRoles, obj.Role)

      End If
    End If

  End Sub

  Private Sub btnUpdate_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnUpdate.Click

    If Session("Source") = "ProjectEdit.aspx" Then
      ' we are dealing with a ProjectResource
      Dim obj As ProjectResource = Session("ProjectResource")

      ' TODO: this line only works in 1.1, so is replaced with next line for 1.0
      'obj.Role = lstRoles.SelectedValue
      obj.Role = lstRoles.SelectedItem.Value

    Else
      ' we are dealing with a ResourceAssignment
      Dim obj As ResourceAssignment = Session("ResourceAssignment")

      ' TODO: this line only works in 1.1, so is replaced with next line for 1.0
      'obj.Role = lstRoles.SelectedValue
      obj.Role = lstRoles.SelectedItem.Value

    End If
    SendUserBack()

  End Sub

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnCancel.Click

    SendUserBack()

  End Sub

  Private Sub SendUserBack()

    Dim src As String = Session("Source")
    Session.Remove("Source")
    Response.Redirect(src)

  End Sub

  Private Sub SelectItem(ByVal lst As System.Web.UI.WebControls.ListBox, _
    ByVal item As String)

    Dim index As Integer = 0
    Dim entry As ListItem

    For Each entry In lst.Items
      If entry.Value = item Then
        lst.SelectedIndex = index
        Return
      End If
      index += 1
    Next

  End Sub

End Class
