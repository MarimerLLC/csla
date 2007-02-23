<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
  Inherits System.Windows.Forms.Form

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing AndAlso components IsNot Nothing Then
      components.Dispose()
    End If
    MyBase.Dispose(disposing)
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
    Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
    Me.ProjectsToolStripDropDownButton = New System.Windows.Forms.ToolStripDropDownButton
    Me.NewProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.EditProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.DeleteProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.ResourcesToolStripDropDownButton = New System.Windows.Forms.ToolStripDropDownButton
    Me.NewResourceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.EditResourceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.DeleteResourceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.AdminToolStripDropDownButton = New System.Windows.Forms.ToolStripDropDownButton
    Me.EditRolesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
    Me.LoginToolStripLabel = New System.Windows.Forms.ToolStripLabel
    Me.LoginToolStripButton = New System.Windows.Forms.ToolStripButton
    Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
    Me.DocumentsToolStripDropDownButton = New System.Windows.Forms.ToolStripDropDownButton
    Me.Panel1 = New System.Windows.Forms.Panel
    Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
    Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel
    Me.ToolStrip1.SuspendLayout()
    Me.StatusStrip1.SuspendLayout()
    Me.SuspendLayout()
    '
    'ToolStrip1
    '
    Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
    Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ProjectsToolStripDropDownButton, Me.ResourcesToolStripDropDownButton, Me.AdminToolStripDropDownButton, Me.ToolStripSeparator1, Me.LoginToolStripLabel, Me.LoginToolStripButton, Me.ToolStripSeparator2, Me.DocumentsToolStripDropDownButton})
    Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
    Me.ToolStrip1.Name = "ToolStrip1"
    Me.ToolStrip1.Size = New System.Drawing.Size(827, 25)
    Me.ToolStrip1.TabIndex = 1
    '
    'ProjectsToolStripDropDownButton
    '
    Me.ProjectsToolStripDropDownButton.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewProjectToolStripMenuItem, Me.EditProjectToolStripMenuItem, Me.DeleteProjectToolStripMenuItem})
    Me.ProjectsToolStripDropDownButton.Image = CType(resources.GetObject("ProjectsToolStripDropDownButton.Image"), System.Drawing.Image)
    Me.ProjectsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.ProjectsToolStripDropDownButton.Name = "ProjectsToolStripDropDownButton"
    Me.ProjectsToolStripDropDownButton.Size = New System.Drawing.Size(75, 22)
    Me.ProjectsToolStripDropDownButton.Text = "Projects"
    '
    'NewProjectToolStripMenuItem
    '
    Me.NewProjectToolStripMenuItem.Name = "NewProjectToolStripMenuItem"
    Me.NewProjectToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
    Me.NewProjectToolStripMenuItem.Text = "New project"
    '
    'EditProjectToolStripMenuItem
    '
    Me.EditProjectToolStripMenuItem.Name = "EditProjectToolStripMenuItem"
    Me.EditProjectToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
    Me.EditProjectToolStripMenuItem.Text = "Edit project"
    '
    'DeleteProjectToolStripMenuItem
    '
    Me.DeleteProjectToolStripMenuItem.Name = "DeleteProjectToolStripMenuItem"
    Me.DeleteProjectToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
    Me.DeleteProjectToolStripMenuItem.Text = "Delete project"
    '
    'ResourcesToolStripDropDownButton
    '
    Me.ResourcesToolStripDropDownButton.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewResourceToolStripMenuItem, Me.EditResourceToolStripMenuItem, Me.DeleteResourceToolStripMenuItem})
    Me.ResourcesToolStripDropDownButton.Image = CType(resources.GetObject("ResourcesToolStripDropDownButton.Image"), System.Drawing.Image)
    Me.ResourcesToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.ResourcesToolStripDropDownButton.Name = "ResourcesToolStripDropDownButton"
    Me.ResourcesToolStripDropDownButton.Size = New System.Drawing.Size(86, 22)
    Me.ResourcesToolStripDropDownButton.Text = "Resources"
    '
    'NewResourceToolStripMenuItem
    '
    Me.NewResourceToolStripMenuItem.Name = "NewResourceToolStripMenuItem"
    Me.NewResourceToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
    Me.NewResourceToolStripMenuItem.Text = "New resource"
    '
    'EditResourceToolStripMenuItem
    '
    Me.EditResourceToolStripMenuItem.Name = "EditResourceToolStripMenuItem"
    Me.EditResourceToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
    Me.EditResourceToolStripMenuItem.Text = "Edit resource"
    '
    'DeleteResourceToolStripMenuItem
    '
    Me.DeleteResourceToolStripMenuItem.Name = "DeleteResourceToolStripMenuItem"
    Me.DeleteResourceToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
    Me.DeleteResourceToolStripMenuItem.Text = "Delete resource"
    '
    'AdminToolStripDropDownButton
    '
    Me.AdminToolStripDropDownButton.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditRolesToolStripMenuItem})
    Me.AdminToolStripDropDownButton.Image = CType(resources.GetObject("AdminToolStripDropDownButton.Image"), System.Drawing.Image)
    Me.AdminToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.AdminToolStripDropDownButton.Name = "AdminToolStripDropDownButton"
    Me.AdminToolStripDropDownButton.Size = New System.Drawing.Size(65, 22)
    Me.AdminToolStripDropDownButton.Text = "Admin"
    '
    'EditRolesToolStripMenuItem
    '
    Me.EditRolesToolStripMenuItem.Name = "EditRolesToolStripMenuItem"
    Me.EditRolesToolStripMenuItem.Size = New System.Drawing.Size(118, 22)
    Me.EditRolesToolStripMenuItem.Text = "Edit roles"
    '
    'ToolStripSeparator1
    '
    Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
    Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
    '
    'LoginToolStripLabel
    '
    Me.LoginToolStripLabel.Name = "LoginToolStripLabel"
    Me.LoginToolStripLabel.Size = New System.Drawing.Size(70, 22)
    Me.LoginToolStripLabel.Text = "Not logged in"
    '
    'LoginToolStripButton
    '
    Me.LoginToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
    Me.LoginToolStripButton.Image = CType(resources.GetObject("LoginToolStripButton.Image"), System.Drawing.Image)
    Me.LoginToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.LoginToolStripButton.Name = "LoginToolStripButton"
    Me.LoginToolStripButton.Size = New System.Drawing.Size(36, 22)
    Me.LoginToolStripButton.Text = "Login"
    '
    'ToolStripSeparator2
    '
    Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
    Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
    '
    'DocumentsToolStripDropDownButton
    '
    Me.DocumentsToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
    Me.DocumentsToolStripDropDownButton.Image = CType(resources.GetObject("DocumentsToolStripDropDownButton.Image"), System.Drawing.Image)
    Me.DocumentsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.DocumentsToolStripDropDownButton.Name = "DocumentsToolStripDropDownButton"
    Me.DocumentsToolStripDropDownButton.Size = New System.Drawing.Size(73, 22)
    Me.DocumentsToolStripDropDownButton.Text = "Documents"
    '
    'Panel1
    '
    Me.Panel1.BackColor = System.Drawing.SystemColors.ControlDark
    Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
    Me.Panel1.Location = New System.Drawing.Point(0, 25)
    Me.Panel1.Name = "Panel1"
    Me.Panel1.Size = New System.Drawing.Size(827, 371)
    Me.Panel1.TabIndex = 2
    '
    'StatusStrip1
    '
    Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel})
    Me.StatusStrip1.Location = New System.Drawing.Point(0, 396)
    Me.StatusStrip1.Name = "StatusStrip1"
    Me.StatusStrip1.Size = New System.Drawing.Size(827, 22)
    Me.StatusStrip1.TabIndex = 3
    Me.StatusStrip1.Text = "StatusStrip1"
    '
    'StatusLabel
    '
    Me.StatusLabel.Name = "StatusLabel"
    Me.StatusLabel.Size = New System.Drawing.Size(0, 17)
    '
    'MainForm
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(827, 418)
    Me.Controls.Add(Me.Panel1)
    Me.Controls.Add(Me.StatusStrip1)
    Me.Controls.Add(Me.ToolStrip1)
    Me.Name = "MainForm"
    Me.Text = "Project Tracker"
    Me.ToolStrip1.ResumeLayout(False)
    Me.ToolStrip1.PerformLayout()
    Me.StatusStrip1.ResumeLayout(False)
    Me.StatusStrip1.PerformLayout()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
  Friend WithEvents ProjectsToolStripDropDownButton As System.Windows.Forms.ToolStripDropDownButton
  Friend WithEvents NewProjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents EditProjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents DeleteProjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents ResourcesToolStripDropDownButton As System.Windows.Forms.ToolStripDropDownButton
  Friend WithEvents NewResourceToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents EditResourceToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents DeleteResourceToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
  Friend WithEvents LoginToolStripLabel As System.Windows.Forms.ToolStripLabel
  Friend WithEvents LoginToolStripButton As System.Windows.Forms.ToolStripButton
  Friend WithEvents Panel1 As System.Windows.Forms.Panel
  Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
  Friend WithEvents DocumentsToolStripDropDownButton As System.Windows.Forms.ToolStripDropDownButton
  Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
  Friend WithEvents StatusLabel As System.Windows.Forms.ToolStripStatusLabel
  Friend WithEvents AdminToolStripDropDownButton As System.Windows.Forms.ToolStripDropDownButton
  Friend WithEvents EditRolesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
