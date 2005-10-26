Public Class MainForm

  Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    StatusChanged()
    DoLogin()
    If DocumentCount = 0 Then
      Me.DocumentsToolStripDropDownButton.Enabled = False
    End If
    ApplyAuthorizationRules()

  End Sub

#Region " Projects "

  Private Sub NewProjectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewProjectToolStripMenuItem.Click

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
      AddWinPart(New ProjectEdit(Project.GetProject(projectId)))
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
          Catch ex As Exception
            MessageBox.Show(ex.ToString, "Error deleting", _
              MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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

    Dim resourceId As String
    resourceId = InputBox("New resource id", "New resource")
    If Len(resourceId) > 0 Then
      Using busy As New StatusBusy("Creating resource...")
        AddWinPart(New ResourceEdit(Resource.NewResource(resourceId)))
      End Using
    End If

  End Sub

  Private Sub EditResourceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditResourceToolStripMenuItem.Click

    Dim dlg As New ResourceSelect
    dlg.Text = "Edit Resource"
    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
      ' get the project id
      ShowEditResource(dlg.ResourceId)
    End If

  End Sub

  Public Sub ShowEditResource(ByVal resourceId As String)

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

    ' the project wasn't already loaded
    ' so load it and display the new winpart
    Using busy As New StatusBusy("Loading resource...")
      AddWinPart(New ResourceEdit(Resource.GetResource(resourceId)))
    End Using

  End Sub

  Private Sub DeleteResourceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteResourceToolStripMenuItem.Click

    Dim dlg As New ResourceSelect
    dlg.Text = "Delete Resource"
    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
      ' get the resource id
      Dim resourceId As String = dlg.ResourceId

      If MessageBox.Show("Are you sure?", "Delete resource", _
        MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
        MessageBoxDefaultButton.Button2) = _
        Windows.Forms.DialogResult.Yes Then

        Using busy As New StatusBusy("Deleting resource...")
          Try
            Resource.DeleteResource(resourceId)
          Catch ex As Exception
            MessageBox.Show(ex.ToString, "Error deleting", _
              MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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
    If Project.CanSaveObject Then
      Me.EditProjectToolStripMenuItem.Text = "Edit project"
    Else
      Me.EditProjectToolStripMenuItem.Text = "View project"
    End If
    Me.DeleteProjectToolStripMenuItem.Enabled = Project.CanDeleteObject

    ' Resource menu
    Me.NewResourceToolStripMenuItem.Enabled = Resource.CanAddObject
    Me.EditResourceToolStripMenuItem.Enabled = Resource.CanGetObject
    If Resource.CanSaveObject Then
      Me.EditResourceToolStripMenuItem.Text = "Edit resource"
    Else
      Me.EditResourceToolStripMenuItem.Text = "View resource"
    End If
    Me.DeleteResourceToolStripMenuItem.Enabled = Resource.CanDeleteObject

  End Sub

#End Region

#Region " Login "

  Private Sub LoginToolStripButton_Click(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles LoginToolStripButton.Click

    DoLogin()

  End Sub

  Private Sub DoLogin()

    If Me.LoginToolStripButton.Text = "Login" Then
      LoginForm.ShowDialog(Me)

    Else
      ProjectTracker.Library.Security.PTPrincipal.Logout()
    End If

    If My.User.IsAuthenticated Then
      Me.LoginToolStripLabel.Text = "Logged in as " & My.User.Name
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
        CType(ctl, WinPart).PrincipalChanged(Me, EventArgs.Empty)
      End If
    Next

  End Sub

#End Region

#Region " WinPart handling "

  Private Sub AddWinPart(ByVal part As WinPart)

    AddHandler part.CloseWinPart, AddressOf CloseWinPart
    AddHandler part.StatusChanged, AddressOf StatusChanged
    part.BackColor = ToolStrip1.BackColor
    Panel1.Controls.Add(part)
    ShowWinPart(part)

  End Sub

  Private Shared TopLeft As New Point(0, 0)

  Private Sub ShowWinPart(ByVal part As WinPart)

    part.Location = TopLeft
    part.Size = Panel1.ClientSize
    part.Visible = True
    part.BringToFront()
    Me.DocumentsToolStripDropDownButton.Enabled = True

  End Sub

  Private Sub Panel1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel1.Resize

    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is WinPart Then
        ctl.Size = Panel1.ClientSize
      End If
    Next

  End Sub

  Private Sub DocumentsToolStripDropDownButton_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles DocumentsToolStripDropDownButton.DropDownOpening

    Dim items As ToolStripItemCollection = DocumentsToolStripDropDownButton.DropDownItems
    items.Clear()
    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is WinPart Then
        items.Add(CType(ctl, WinPart).Title, Nothing, AddressOf DocumentClick)
      End If
    Next
  End Sub

  Private Sub DocumentClick(ByVal sender As Object, ByVal e As EventArgs)

    For Each ctl As Control In Panel1.Controls
      If TypeOf ctl Is WinPart AndAlso CType(ctl, WinPart).Title = CType(sender, ToolStripItem).Text Then
        ctl.Visible = True
        ctl.BringToFront()
      End If
    Next

  End Sub

  Private Sub CloseWinPart(ByVal sender As Object, ByVal e As EventArgs)

    Dim part As WinPart = CType(sender, WinPart)
    RemoveHandler part.CloseWinPart, AddressOf CloseWinPart
    part.Visible = False
    Panel1.Controls.Remove(part)
    part.Dispose()
    If DocumentCount = 0 Then
      Me.DocumentsToolStripDropDownButton.Enabled = False
    End If

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

#End Region

#Region " Status "

  Public Sub StatusChanged()

    StatusChanged("", False)

  End Sub

  Public Sub StatusChanged(ByVal statusText As String)

    StatusChanged(statusText, Len(statusText) > 0)

  End Sub

  Public Sub StatusChanged(ByVal statusText As String, ByVal busy As Boolean)

    StatusLabel.Text = statusText
    If busy Then
      Me.Cursor = Cursors.WaitCursor

    Else
      Me.Cursor = Cursors.Default
    End If

  End Sub

  Private Sub StatusChanged(ByVal sender As Object, ByVal e As StatusChangedEventArgs)

    StatusChanged(e.Status, e.Busy)

  End Sub

#End Region

End Class