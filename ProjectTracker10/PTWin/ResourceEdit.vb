Imports System.Threading

Public Class ResourceEdit
  Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

  Public Sub New()
    MyBase.New()

    'This call is required by the Windows Form Designer.
    InitializeComponent()

    'Add any initialization after the InitializeComponent() call

  End Sub

  'Form overrides dispose to clean up the component list.
  Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing Then
      If Not (components Is Nothing) Then
        components.Dispose()
      End If
    End If
    MyBase.Dispose(disposing)
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  Friend WithEvents btnCancel As System.Windows.Forms.Button
  Friend WithEvents btnSave As System.Windows.Forms.Button
  Friend WithEvents txtID As System.Windows.Forms.TextBox
  Friend WithEvents Label5 As System.Windows.Forms.Label
  Friend WithEvents Label2 As System.Windows.Forms.Label
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents txtFirstname As System.Windows.Forms.TextBox
  Friend WithEvents txtLastname As System.Windows.Forms.TextBox
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents btnRemoveProject As System.Windows.Forms.Button
  Friend WithEvents btnAssignProject As System.Windows.Forms.Button
  Friend WithEvents colProjectID As System.Windows.Forms.DataGridTextBoxColumn
  Friend WithEvents colName As System.Windows.Forms.DataGridTextBoxColumn
  Friend WithEvents colAssigned As System.Windows.Forms.DataGridTextBoxColumn
  Friend WithEvents colRole As System.Windows.Forms.DataGridTextBoxColumn
  Friend WithEvents dvProjects As PTWin.DataListView
  Friend WithEvents mnuRoles As System.Windows.Forms.ContextMenu
  Friend WithEvents chkIsDirty As System.Windows.Forms.CheckBox
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.btnCancel = New System.Windows.Forms.Button
    Me.btnSave = New System.Windows.Forms.Button
    Me.txtID = New System.Windows.Forms.TextBox
    Me.Label5 = New System.Windows.Forms.Label
    Me.txtFirstname = New System.Windows.Forms.TextBox
    Me.Label2 = New System.Windows.Forms.Label
    Me.txtLastname = New System.Windows.Forms.TextBox
    Me.Label1 = New System.Windows.Forms.Label
    Me.GroupBox1 = New System.Windows.Forms.GroupBox
    Me.dvProjects = New PTWin.DataListView
    Me.mnuRoles = New System.Windows.Forms.ContextMenu
    Me.btnRemoveProject = New System.Windows.Forms.Button
    Me.btnAssignProject = New System.Windows.Forms.Button
    Me.colProjectID = New System.Windows.Forms.DataGridTextBoxColumn
    Me.colName = New System.Windows.Forms.DataGridTextBoxColumn
    Me.colAssigned = New System.Windows.Forms.DataGridTextBoxColumn
    Me.colRole = New System.Windows.Forms.DataGridTextBoxColumn
    Me.chkIsDirty = New System.Windows.Forms.CheckBox
    Me.GroupBox1.SuspendLayout()
    Me.SuspendLayout()
    '
    'btnCancel
    '
    Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.btnCancel.Location = New System.Drawing.Point(616, 40)
    Me.btnCancel.Name = "btnCancel"
    Me.btnCancel.TabIndex = 8
    Me.btnCancel.Text = "&Cancel"
    '
    'btnSave
    '
    Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnSave.Location = New System.Drawing.Point(616, 8)
    Me.btnSave.Name = "btnSave"
    Me.btnSave.TabIndex = 7
    Me.btnSave.Text = "&Save"
    '
    'txtID
    '
    Me.txtID.Location = New System.Drawing.Point(88, 8)
    Me.txtID.Name = "txtID"
    Me.txtID.ReadOnly = True
    Me.txtID.Size = New System.Drawing.Size(96, 20)
    Me.txtID.TabIndex = 1
    Me.txtID.TabStop = False
    Me.txtID.Text = ""
    '
    'Label5
    '
    Me.Label5.Location = New System.Drawing.Point(8, 8)
    Me.Label5.Name = "Label5"
    Me.Label5.TabIndex = 0
    Me.Label5.Text = "ID"
    '
    'txtFirstname
    '
    Me.txtFirstname.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.txtFirstname.Location = New System.Drawing.Point(88, 40)
    Me.txtFirstname.Name = "txtFirstname"
    Me.txtFirstname.Size = New System.Drawing.Size(512, 20)
    Me.txtFirstname.TabIndex = 3
    Me.txtFirstname.Text = ""
    '
    'Label2
    '
    Me.Label2.Location = New System.Drawing.Point(8, 40)
    Me.Label2.Name = "Label2"
    Me.Label2.TabIndex = 2
    Me.Label2.Text = "First name"
    '
    'txtLastname
    '
    Me.txtLastname.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.txtLastname.Location = New System.Drawing.Point(88, 72)
    Me.txtLastname.Name = "txtLastname"
    Me.txtLastname.Size = New System.Drawing.Size(512, 20)
    Me.txtLastname.TabIndex = 5
    Me.txtLastname.Text = ""
    '
    'Label1
    '
    Me.Label1.Location = New System.Drawing.Point(8, 72)
    Me.Label1.Name = "Label1"
    Me.Label1.TabIndex = 4
    Me.Label1.Text = "Last name"
    '
    'GroupBox1
    '
    Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.GroupBox1.Controls.Add(Me.dvProjects)
    Me.GroupBox1.Controls.Add(Me.btnRemoveProject)
    Me.GroupBox1.Controls.Add(Me.btnAssignProject)
    Me.GroupBox1.Location = New System.Drawing.Point(8, 104)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(688, 192)
    Me.GroupBox1.TabIndex = 6
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "Assigned to"
    '
    'dvProjects
    '
    Me.dvProjects.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.dvProjects.AutoDiscover = False
    Me.dvProjects.ContextMenu = Me.mnuRoles
    Me.dvProjects.DataSource = Nothing
    Me.dvProjects.DisplayMember = ""
    Me.dvProjects.FullRowSelect = True
    Me.dvProjects.Location = New System.Drawing.Point(16, 24)
    Me.dvProjects.MultiSelect = False
    Me.dvProjects.Name = "dvProjects"
    Me.dvProjects.Size = New System.Drawing.Size(576, 160)
    Me.dvProjects.TabIndex = 0
    Me.dvProjects.View = System.Windows.Forms.View.Details
    '
    'btnRemoveProject
    '
    Me.btnRemoveProject.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnRemoveProject.Location = New System.Drawing.Point(600, 56)
    Me.btnRemoveProject.Name = "btnRemoveProject"
    Me.btnRemoveProject.TabIndex = 2
    Me.btnRemoveProject.Text = "&Remove"
    '
    'btnAssignProject
    '
    Me.btnAssignProject.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnAssignProject.Location = New System.Drawing.Point(600, 24)
    Me.btnAssignProject.Name = "btnAssignProject"
    Me.btnAssignProject.TabIndex = 1
    Me.btnAssignProject.Text = "&Assign to"
    '
    'colProjectID
    '
    Me.colProjectID.Format = ""
    Me.colProjectID.FormatInfo = Nothing
    Me.colProjectID.MappingName = "ProjectID"
    Me.colProjectID.ReadOnly = True
    Me.colProjectID.Width = 0
    '
    'colName
    '
    Me.colName.Format = ""
    Me.colName.FormatInfo = Nothing
    Me.colName.HeaderText = "Project"
    Me.colName.MappingName = "ProjectName"
    Me.colName.ReadOnly = True
    Me.colName.Width = 140
    '
    'colAssigned
    '
    Me.colAssigned.Format = ""
    Me.colAssigned.FormatInfo = Nothing
    Me.colAssigned.HeaderText = "Assigned"
    Me.colAssigned.MappingName = "Assigned"
    Me.colAssigned.ReadOnly = True
    Me.colAssigned.Width = 75
    '
    'colRole
    '
    Me.colRole.Format = ""
    Me.colRole.FormatInfo = Nothing
    Me.colRole.HeaderText = "Role"
    Me.colRole.MappingName = "Role"
    Me.colRole.Width = 140
    '
    'chkIsDirty
    '
    Me.chkIsDirty.Location = New System.Drawing.Point(-368, 136)
    Me.chkIsDirty.Name = "chkIsDirty"
    Me.chkIsDirty.Size = New System.Drawing.Size(105, 24)
    Me.chkIsDirty.TabIndex = 24
    Me.chkIsDirty.Text = "CheckBox1"
    '
    'ResourceEdit
    '
    Me.AcceptButton = Me.btnSave
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.CancelButton = Me.btnCancel
    Me.ClientSize = New System.Drawing.Size(704, 301)
    Me.Controls.Add(Me.chkIsDirty)
    Me.Controls.Add(Me.GroupBox1)
    Me.Controls.Add(Me.txtID)
    Me.Controls.Add(Me.txtFirstname)
    Me.Controls.Add(Me.txtLastname)
    Me.Controls.Add(Me.Label5)
    Me.Controls.Add(Me.Label2)
    Me.Controls.Add(Me.Label1)
    Me.Controls.Add(Me.btnCancel)
    Me.Controls.Add(Me.btnSave)
    Me.Name = "ResourceEdit"
    Me.Text = "ResourceEdit"
    Me.GroupBox1.ResumeLayout(False)
    Me.ResumeLayout(False)

  End Sub

#End Region

  Private mResource As Resource

  Public Property Resource() As Resource
    Get
      Return mResource
    End Get
    Set(ByVal Value As Resource)
      mResource = Value
    End Set
  End Property

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnCancel.Click

    mResource.CancelEdit()
    Hide()

  End Sub

  Private Sub btnSave_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnSave.Click

    Try
      Cursor.Current = Cursors.WaitCursor
      mResource.ApplyEdit()
      mResource = DirectCast(mResource.Save, Resource)
      Cursor.Current = Cursors.Default

    Catch ex As Exception
      Cursor.Current = Cursors.Default
      MsgBox(ex.ToString)
    End Try

    Hide()

  End Sub

  Private Sub ResourceEdit_Load(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load

    Me.Text = "Resource " & mResource.LastName & ", " & mResource.FirstName

    Dim Role As String
    For Each Role In Assignment.Roles
      mnuRoles.MenuItems.Add(Assignment.Roles.Item(Role))
      AddHandler mnuRoles.MenuItems(mnuRoles.MenuItems.Count - 1).Click, _
        AddressOf mnuRoles_Click
    Next

    If Thread.CurrentPrincipal.IsInRole("ProjectManager") OrElse _
        Thread.CurrentPrincipal.IsInRole("Supervisor") Then
      ' only project managers or supervisors can edit a resource
      mResource.BeginEdit()
      btnAssignProject.Enabled = True
      btnRemoveProject.Enabled = True

    Else
      btnAssignProject.Enabled = False
      btnRemoveProject.Enabled = False
    End If

    DataBind()

  End Sub

  Private Sub DataBind()

    If Thread.CurrentPrincipal.IsInRole("ProjectManager") OrElse _
        Thread.CurrentPrincipal.IsInRole("Supervisor") Then
      ' only project managers or supervisors can save a resource
      BindField(btnSave, "Enabled", mResource, "IsValid")

    Else
      btnSave.Enabled = False
    End If

    BindField(chkIsDirty, "Checked", mResource, "IsDirty")
    BindField(txtID, "Text", mResource, "ID")
    BindField(txtLastname, "Text", mResource, "LastName")
    BindField(txtFirstname, "Text", mResource, "FirstName")

    With dvProjects
      .SuspendLayout()
      .Clear()
      .AutoDiscover = False
      .Columns.Add("ID", "ProjectID", 0)
      .Columns.Add("Project", "ProjectName", 200)
      .Columns.Add("Assigned", "Assigned", 100)
      .Columns.Add("Role", "Role", 150)
      .DataSource = mResource.Assignments
      .ResumeLayout()
    End With

  End Sub

  Private Sub ResourceEdit_Closing(ByVal sender As Object, _
      ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

    mResource.CancelEdit()

  End Sub

  Private Sub btnAssignProject_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnAssignProject.Click

    Dim dlg As New ProjectSelect
    dlg.Text = "Assign to project"
    dlg.ShowDialog(Me)
    Dim Result As String = dlg.Result

    If Len(Result) > 0 Then
      dvProjects.SuspendLayout()
      dvProjects.DataSource = Nothing
      Dim ID As Guid = New Guid(Result)
      Try
        mResource.Assignments.AssignTo(ID)

      Catch ex As Exception
        MsgBox(ex.Message)

      Finally
        dvProjects.DataSource = mResource.Assignments
        dvProjects.ResumeLayout()
      End Try
    End If

  End Sub

  Private Sub btnRemoveProject_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnRemoveProject.Click

    Dim ID As Guid = New Guid(dvProjects.SelectedItems(0).Text)
    Dim Name As String = dvProjects.SelectedItems(0).SubItems(0).Text

    If MsgBox("Remove from project " & Name & "?", _
        MsgBoxStyle.YesNo, "Remove assignment") = MsgBoxResult.Yes Then
      dvProjects.SuspendLayout()
      dvProjects.DataSource = Nothing
      mResource.Assignments.Remove(ID)
      dvProjects.DataSource = mResource.Assignments
      dvProjects.ResumeLayout()
    End If

  End Sub

  Private Sub mnuRoles_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs)

    Dim Item As MenuItem = CType(sender, MenuItem)
    If dvProjects.SelectedItems.Count > 0 Then
      Dim ID As Guid = New Guid(dvProjects.SelectedItems(0).Text)

      dvProjects.SuspendLayout()
      dvProjects.DataSource = Nothing
      mResource.Assignments.Item(ID).Role = Item.Text
      dvProjects.DataSource = mResource.Assignments
      dvProjects.ResumeLayout()
    End If

  End Sub

  Private Sub dvProjects_DoubleClick(ByVal sender As Object, _
      ByVal e As System.EventArgs) Handles dvProjects.DoubleClick

    Dim ID As Guid = New Guid(dvProjects.SelectedItems(0).Text)
    Cursor.Current = Cursors.WaitCursor
    Dim frm As New ProjectEdit
    frm.MdiParent = Me.MdiParent
    frm.Project = Project.GetProject(ID)
    Cursor.Current = Cursors.Default
    frm.Show()

  End Sub

End Class
