Public Class MainForm

  Private Sub MainForm_Load( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load

    If Csla.ApplicationContext.AuthenticationType = "Windows" Then
      AppDomain.CurrentDomain.SetPrincipalPolicy( _
        System.Security.Principal.PrincipalPolicy.WindowsPrincipal)

    Else
      DoLogin()
    End If
    If DocumentCount = 0 Then
      Me.DocumentsToolStripDropDownButton.Enabled = False
    End If
    ApplyAuthorizationRules()

  End Sub

#Region " Projects "

  Private Sub NewProjectToolStripMenuItem_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles NewProjectToolStripMenuItem.Click

    Using busy As New StatusBusy("Creating project...")
      AddWinPart(New ProjectEdit(Project.NewProject))
    End Using

  End Sub

  Private Sub EditProjectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditProjectToolStripMenuItem.Click

    Dim dlg As New ProjectSelect
    dlg.Text = "Edit Project"
    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
      ShowEditProject(dlg.ProjectId)
    End If

  End Sub

  Public Sub ShowEditProject(ByVal projectId As Guid)

    ' see if this project is already loaded
    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is ProjectEdit Then
        Dim part As ProjectEdit = CType(ctl, ProjectEdit)
        If part.Project.Id.Equals(projectId) Then
          ' project already loaded so just
          ' display the existing winpart
          ShowWinPart(part)
          Exit Sub
        End If
      End If
    Next

    ' the project wasn't already loaded
    ' so load it and display the new winpart
    Using busy As New StatusBusy("Loading project...")
      Try
        AddWinPart(New ProjectEdit(Project.GetProject(projectId)))

      Catch ex As Csla.DataPortalException
        MessageBox.Show(ex.BusinessException.ToString, _
          "Error loading", MessageBoxButtons.OK, _
          MessageBoxIcon.Exclamation)

      Catch ex As Exception
        MessageBox.Show(ex.ToString, _
          "Error loading", MessageBoxButtons.OK, _
          MessageBoxIcon.Exclamation)
      End Try
    End Using

  End Sub

  Private Sub DeleteProjectToolStripMenuItem_Click( _
    ByVal sender As System.Object, _
    ByVal e As System.EventArgs) _
    Handles DeleteProjectToolStripMenuItem.Click

    Dim dlg As New ProjectSelect
    dlg.Text = "Delete Project"
    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
      ' get the project id
      Dim projectId As Guid = dlg.ProjectId

      If MessageBox.Show("Are you sure?", "Delete project", _
        MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
        MessageBoxDefaultButton.Button2) = _
        Windows.Forms.DialogResult.Yes Then

        Using busy As New StatusBusy("Deleting project...")
          Try
            Project.DeleteProject(projectId)

          Catch ex As Csla.DataPortalException
            MessageBox.Show(ex.BusinessException.ToString, _
              "Error deleting", MessageBoxButtons.OK, _
              MessageBoxIcon.Exclamation)

          Catch ex As Exception
            MessageBox.Show(ex.ToString, _
              "Error deleting", MessageBoxButtons.OK, _
              MessageBoxIcon.Exclamation)
          End Try
        End Using
      End If
    End If

  End Sub

#End Region

#Region " Resources "

  Private Sub NewResourceToolStripMenuItem_Click( _
    ByVal sender As System.Object, _
    ByVal e As System.EventArgs) _
    Handles NewResourceToolStripMenuItem.Click

    Using busy As New StatusBusy("Creating resource...")
      AddWinPart(New ResourceEdit(Resource.NewResource))
    End Using

  End Sub

  Private Sub EditResourceToolStripMenuItem_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles EditResourceToolStripMenuItem.Click

    Dim dlg As New ResourceSelect
    dlg.Text = "Edit Resource"
    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
      ' get the project id
      ShowEditResource(dlg.ResourceId)
    End If

  End Sub

  Public Sub ShowEditResource(ByVal resourceId As Integer)

    ' see if this project is already loaded
    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is ResourceEdit Then
        Dim part As ResourceEdit = CType(ctl, ResourceEdit)
        If part.Resource.Id.Equals(resourceId) Then
          ' project already loaded so just
          ' display the existing winpart
          ShowWinPart(part)
          Exit Sub
        End If
      End If
    Next

    ' the resource wasn't already loaded
    ' so load it and display the new winpart
    Using busy As New StatusBusy("Loading resource...")
      Try
        AddWinPart(New ResourceEdit(Resource.GetResource(resourceId)))

      Catch ex As Csla.DataPortalException
        MessageBox.Show(ex.BusinessException.ToString, _
          "Error loading", MessageBoxButtons.OK, _
          MessageBoxIcon.Exclamation)

      Catch ex As Exception
        MessageBox.Show(ex.ToString, _
          "Error loading", MessageBoxButtons.OK, _
          MessageBoxIcon.Exclamation)
      End Try
    End Using

  End Sub

  Private Sub DeleteResourceToolStripMenuItem_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DeleteResourceToolStripMenuItem.Click

    Dim dlg As New ResourceSelect
    dlg.Text = "Delete Resource"
    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
      ' get the resource id
      Dim resourceId As Integer = dlg.ResourceId

      If MessageBox.Show("Are you sure?", "Delete resource", _
        MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
        MessageBoxDefaultButton.Button2) = _
        Windows.Forms.DialogResult.Yes Then

        Using busy As New StatusBusy("Deleting resource...")
          Try
            Resource.DeleteResource(resourceId)

          Catch ex As Csla.DataPortalException
            MessageBox.Show(ex.BusinessException.ToString, _
              "Error deleting", MessageBoxButtons.OK, _
              MessageBoxIcon.Exclamation)

          Catch ex As Exception
            MessageBox.Show(ex.ToString, _
              "Error deleting", MessageBoxButtons.OK, _
              MessageBoxIcon.Exclamation)
          End Try
        End Using
      End If
    End If

  End Sub

#End Region

#Region " Admin "

  Private Sub EditRolesToolStripMenuItem_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles EditRolesToolStripMenuItem.Click

    ' see if this form is already loaded
    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is RolesEdit Then
        ShowWinPart(CType(ctl, WinPart))
        Exit Sub
      End If
    Next

    ' it wasn't already loaded, so show it
    AddWinPart(New RolesEdit)

  End Sub

#End Region

#Region " ApplyAuthorizationRules "

  Private Sub ApplyAuthorizationRules()

    ' Project menu
    Me.NewProjectToolStripMenuItem.Enabled = Project.CanAddObject
    Me.EditProjectToolStripMenuItem.Enabled = Project.CanGetObject
    If Project.CanEditObject Then
      Me.EditProjectToolStripMenuItem.Text = "Edit project"
    Else
      Me.EditProjectToolStripMenuItem.Text = "View project"
    End If
    Me.DeleteProjectToolStripMenuItem.Enabled = Project.CanDeleteObject

    ' Resource menu
    Me.NewResourceToolStripMenuItem.Enabled = Resource.CanAddObject
    Me.EditResourceToolStripMenuItem.Enabled = Resource.CanGetObject
    If Resource.CanEditObject Then
      Me.EditResourceToolStripMenuItem.Text = "Edit resource"
    Else
      Me.EditResourceToolStripMenuItem.Text = "View resource"
    End If
    Me.DeleteResourceToolStripMenuItem.Enabled = Resource.CanDeleteObject

    ' Admin menu
    Me.EditRolesToolStripMenuItem.Enabled = Admin.Roles.CanEditObject

  End Sub

#End Region

#Region " Login "

  Private Sub LoginToolStripButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles LoginToolStripButton.Click

    DoLogin()

  End Sub

  Private Sub DoLogin()

    ProjectTracker.Library.Security.PTPrincipal.Logout()

    If Me.LoginToolStripButton.Text = "Login" Then
      LoginForm.ShowDialog(Me)
    End If

    Dim user As System.Security.Principal.IPrincipal = _
      Csla.ApplicationContext.User

    If user.Identity.IsAuthenticated Then
      Me.LoginToolStripLabel.Text = "Logged in as " & user.Identity.Name
      Me.LoginToolStripButton.Text = "Logout"

    Else
      Me.LoginToolStripLabel.Text = "Not logged in"
      Me.LoginToolStripButton.Text = "Login"
    End If

    ' reset menus, etc.
    ApplyAuthorizationRules()

    ' notify all documents
    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is WinPart Then
        CType(ctl, WinPart).OnCurrentPrincipalChanged(Me, EventArgs.Empty)
      End If
    Next

  End Sub

#End Region

#Region " WinPart handling "

  Private Sub AddWinPart(ByVal part As WinPart)

    AddHandler part.CloseWinPart, AddressOf CloseWinPart
    part.BackColor = ToolStrip1.BackColor
    Panel1.Controls.Add(part)
    Me.DocumentsToolStripDropDownButton.Enabled = True
    ShowWinPart(part)

  End Sub

  Private Sub ShowWinPart(ByVal part As WinPart)

    part.Dock = DockStyle.Fill
    part.Visible = True
    part.BringToFront()
    Me.Text = "Project Tracker - " & part.ToString

  End Sub

  Private Sub DocumentsToolStripDropDownButton_DropDownOpening( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles DocumentsToolStripDropDownButton.DropDownOpening

    Dim items As ToolStripItemCollection = _
      DocumentsToolStripDropDownButton.DropDownItems
    For Each item As ToolStripItem In items
      RemoveHandler item.Click, AddressOf DocumentClick
    Next
    items.Clear()
    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is WinPart Then
        Dim item As New ToolStripMenuItem()
        item.Text = CType(ctl, WinPart).ToString
        item.Tag = ctl
        AddHandler item.Click, AddressOf DocumentClick
        items.Add(item)
      End If
    Next

  End Sub

  Private Sub DocumentClick(ByVal sender As Object, ByVal e As EventArgs)

    Dim ctl As WinPart = CType(CType(sender, ToolStripItem).Tag, WinPart)
    ShowWinPart(ctl)

  End Sub

  Public ReadOnly Property DocumentCount() As Integer
    Get
      Dim count As Integer
      For Each ctl As Control In Panel1.Controls
        If TypeOf ctl Is WinPart Then
          count += 1
        End If
      Next
      Return count
    End Get
  End Property

  Private Sub CloseWinPart(ByVal sender As Object, ByVal e As EventArgs)

    Dim part As WinPart = CType(sender, WinPart)
    RemoveHandler part.CloseWinPart, AddressOf CloseWinPart
    part.Visible = False
    Panel1.Controls.Remove(part)
    part.Dispose()
    If DocumentCount = 0 Then
      Me.DocumentsToolStripDropDownButton.Enabled = False
      Me.Text = "Project Tracker"

    Else
      ' Find the first WinPart control and set
      ' the main form's Text property accordingly.
      ' This works because the first WinPart 
      ' is the active one.
      For Each ctl As Control In Panel1.Controls
        If TypeOf ctl Is WinPart Then
          Me.Text = "Project Tracker - " + CType(ctl, WinPart).ToString
          Exit For
        End If
      Next
    End If

  End Sub

  Private Sub Panel1_Resize( _
    ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel1.Resize

    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is WinPart Then
        ctl.Size = Panel1.ClientSize
      End If
    Next

  End Sub

#End Region

End Class