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
    Me.components = New System.ComponentModel.Container
    Dim IdLabel As System.Windows.Forms.Label
    Dim NameLabel As System.Windows.Forms.Label
    Dim StartedLabel As System.Windows.Forms.Label
    Dim EndedLabel As System.Windows.Forms.Label
    Dim DescriptionLabel As System.Windows.Forms.Label
    Dim IdLabel2 As System.Windows.Forms.Label
    Dim NameLabel1 As System.Windows.Forms.Label
    Dim IdLabel4 As System.Windows.Forms.Label
    Dim NameLabel3 As System.Windows.Forms.Label
    Me.ProjectInfoBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ProjectInfoDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.ResourceInfoBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ResourceInfoDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn6 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn7 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.RoleInfoBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.RoleInfoDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn8 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn9 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.Label1 = New System.Windows.Forms.Label
    Me.Label2 = New System.Windows.Forms.Label
    Me.Label3 = New System.Windows.Forms.Label
    Me.GroupBox1 = New System.Windows.Forms.GroupBox
    Me.ProjectResourcesDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn10 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn11 = New System.Windows.Forms.DataGridViewComboBoxColumn
    Me.ProjectResourcesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ProjectDetailBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ClearProjectButton = New System.Windows.Forms.Button
    Me.SaveProjectButton = New System.Windows.Forms.Button
    Me.DescriptionTextBox = New System.Windows.Forms.TextBox
    Me.EndedTextBox = New System.Windows.Forms.TextBox
    Me.StartedTextBox = New System.Windows.Forms.TextBox
    Me.NameTextBox = New System.Windows.Forms.TextBox
    Me.IdLabel1 = New System.Windows.Forms.Label
    Me.GroupBox2 = New System.Windows.Forms.GroupBox
    Me.AssignToProjectButton = New System.Windows.Forms.Button
    Me.NameLabel4 = New System.Windows.Forms.Label
    Me.ResourceIdLabel = New System.Windows.Forms.Label
    Me.Label5 = New System.Windows.Forms.Label
    Me.Label4 = New System.Windows.Forms.Label
    Me.NameLabel2 = New System.Windows.Forms.Label
    Me.ProjectIdLabel = New System.Windows.Forms.Label
    Me.Label6 = New System.Windows.Forms.Label
    Me.UsernameTextBox = New System.Windows.Forms.TextBox
    Me.Label7 = New System.Windows.Forms.Label
    Me.PasswordTextBox = New System.Windows.Forms.TextBox
    IdLabel = New System.Windows.Forms.Label
    NameLabel = New System.Windows.Forms.Label
    StartedLabel = New System.Windows.Forms.Label
    EndedLabel = New System.Windows.Forms.Label
    DescriptionLabel = New System.Windows.Forms.Label
    IdLabel2 = New System.Windows.Forms.Label
    NameLabel1 = New System.Windows.Forms.Label
    IdLabel4 = New System.Windows.Forms.Label
    NameLabel3 = New System.Windows.Forms.Label
    CType(Me.ProjectInfoBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ProjectInfoDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ResourceInfoBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ResourceInfoDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.RoleInfoBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.RoleInfoDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.GroupBox1.SuspendLayout()
    CType(Me.ProjectResourcesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ProjectResourcesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ProjectDetailBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.GroupBox2.SuspendLayout()
    Me.SuspendLayout()
    '
    'IdLabel
    '
    IdLabel.AutoSize = True
    IdLabel.Location = New System.Drawing.Point(7, 30)
    IdLabel.Name = "IdLabel"
    IdLabel.Size = New System.Drawing.Size(19, 13)
    IdLabel.TabIndex = 0
    IdLabel.Text = "Id:"
    '
    'NameLabel
    '
    NameLabel.AutoSize = True
    NameLabel.Location = New System.Drawing.Point(7, 59)
    NameLabel.Name = "NameLabel"
    NameLabel.Size = New System.Drawing.Size(38, 13)
    NameLabel.TabIndex = 2
    NameLabel.Text = "Name:"
    '
    'StartedLabel
    '
    StartedLabel.AutoSize = True
    StartedLabel.Location = New System.Drawing.Point(7, 88)
    StartedLabel.Name = "StartedLabel"
    StartedLabel.Size = New System.Drawing.Size(44, 13)
    StartedLabel.TabIndex = 4
    StartedLabel.Text = "Started:"
    '
    'EndedLabel
    '
    EndedLabel.AutoSize = True
    EndedLabel.Location = New System.Drawing.Point(7, 117)
    EndedLabel.Name = "EndedLabel"
    EndedLabel.Size = New System.Drawing.Size(41, 13)
    EndedLabel.TabIndex = 6
    EndedLabel.Text = "Ended:"
    '
    'DescriptionLabel
    '
    DescriptionLabel.AutoSize = True
    DescriptionLabel.Location = New System.Drawing.Point(7, 148)
    DescriptionLabel.Name = "DescriptionLabel"
    DescriptionLabel.Size = New System.Drawing.Size(63, 13)
    DescriptionLabel.TabIndex = 8
    DescriptionLabel.Text = "Description:"
    '
    'IdLabel2
    '
    IdLabel2.AutoSize = True
    IdLabel2.Location = New System.Drawing.Point(9, 44)
    IdLabel2.Name = "IdLabel2"
    IdLabel2.Size = New System.Drawing.Size(19, 13)
    IdLabel2.TabIndex = 0
    IdLabel2.Text = "Id:"
    '
    'NameLabel1
    '
    NameLabel1.AutoSize = True
    NameLabel1.Location = New System.Drawing.Point(7, 68)
    NameLabel1.Name = "NameLabel1"
    NameLabel1.Size = New System.Drawing.Size(38, 13)
    NameLabel1.TabIndex = 2
    NameLabel1.Text = "Name:"
    '
    'IdLabel4
    '
    IdLabel4.AutoSize = True
    IdLabel4.Location = New System.Drawing.Point(9, 130)
    IdLabel4.Name = "IdLabel4"
    IdLabel4.Size = New System.Drawing.Size(19, 13)
    IdLabel4.TabIndex = 5
    IdLabel4.Text = "Id:"
    '
    'NameLabel3
    '
    NameLabel3.AutoSize = True
    NameLabel3.Location = New System.Drawing.Point(9, 153)
    NameLabel3.Name = "NameLabel3"
    NameLabel3.Size = New System.Drawing.Size(38, 13)
    NameLabel3.TabIndex = 7
    NameLabel3.Text = "Name:"
    '
    'ProjectInfoBindingSource
    '
    Me.ProjectInfoBindingSource.DataSource = GetType(PTServiceClient.PTService.ProjectData)
    '
    'ProjectInfoDataGridView
    '
    Me.ProjectInfoDataGridView.AutoGenerateColumns = False
    Me.ProjectInfoDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2})
    Me.ProjectInfoDataGridView.DataSource = Me.ProjectInfoBindingSource
    Me.ProjectInfoDataGridView.Location = New System.Drawing.Point(12, 82)
    Me.ProjectInfoDataGridView.MultiSelect = False
    Me.ProjectInfoDataGridView.Name = "ProjectInfoDataGridView"
    Me.ProjectInfoDataGridView.ReadOnly = True
    Me.ProjectInfoDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
    Me.ProjectInfoDataGridView.Size = New System.Drawing.Size(277, 220)
    Me.ProjectInfoDataGridView.TabIndex = 1
    '
    'DataGridViewTextBoxColumn1
    '
    Me.DataGridViewTextBoxColumn1.DataPropertyName = "Id"
    Me.DataGridViewTextBoxColumn1.HeaderText = "Id"
    Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
    Me.DataGridViewTextBoxColumn1.ReadOnly = True
    '
    'DataGridViewTextBoxColumn2
    '
    Me.DataGridViewTextBoxColumn2.DataPropertyName = "Name"
    Me.DataGridViewTextBoxColumn2.HeaderText = "Name"
    Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
    Me.DataGridViewTextBoxColumn2.ReadOnly = True
    '
    'ResourceInfoBindingSource
    '
    Me.ResourceInfoBindingSource.DataSource = GetType(PTServiceClient.PTService.ResourceData)
    '
    'ResourceInfoDataGridView
    '
    Me.ResourceInfoDataGridView.AutoGenerateColumns = False
    Me.ResourceInfoDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn6, Me.DataGridViewTextBoxColumn7})
    Me.ResourceInfoDataGridView.DataSource = Me.ResourceInfoBindingSource
    Me.ResourceInfoDataGridView.Location = New System.Drawing.Point(295, 82)
    Me.ResourceInfoDataGridView.MultiSelect = False
    Me.ResourceInfoDataGridView.Name = "ResourceInfoDataGridView"
    Me.ResourceInfoDataGridView.ReadOnly = True
    Me.ResourceInfoDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
    Me.ResourceInfoDataGridView.Size = New System.Drawing.Size(300, 220)
    Me.ResourceInfoDataGridView.TabIndex = 1
    '
    'DataGridViewTextBoxColumn6
    '
    Me.DataGridViewTextBoxColumn6.DataPropertyName = "Id"
    Me.DataGridViewTextBoxColumn6.HeaderText = "Id"
    Me.DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6"
    Me.DataGridViewTextBoxColumn6.ReadOnly = True
    '
    'DataGridViewTextBoxColumn7
    '
    Me.DataGridViewTextBoxColumn7.DataPropertyName = "Name"
    Me.DataGridViewTextBoxColumn7.HeaderText = "Name"
    Me.DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7"
    Me.DataGridViewTextBoxColumn7.ReadOnly = True
    '
    'RoleInfoBindingSource
    '
    Me.RoleInfoBindingSource.DataSource = GetType(PTServiceClient.PTService.RoleData)
    '
    'RoleInfoDataGridView
    '
    Me.RoleInfoDataGridView.AutoGenerateColumns = False
    Me.RoleInfoDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn8, Me.DataGridViewTextBoxColumn9})
    Me.RoleInfoDataGridView.DataSource = Me.RoleInfoBindingSource
    Me.RoleInfoDataGridView.Location = New System.Drawing.Point(601, 82)
    Me.RoleInfoDataGridView.MultiSelect = False
    Me.RoleInfoDataGridView.Name = "RoleInfoDataGridView"
    Me.RoleInfoDataGridView.ReadOnly = True
    Me.RoleInfoDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
    Me.RoleInfoDataGridView.Size = New System.Drawing.Size(300, 220)
    Me.RoleInfoDataGridView.TabIndex = 2
    '
    'DataGridViewTextBoxColumn8
    '
    Me.DataGridViewTextBoxColumn8.DataPropertyName = "Id"
    Me.DataGridViewTextBoxColumn8.HeaderText = "Id"
    Me.DataGridViewTextBoxColumn8.Name = "DataGridViewTextBoxColumn8"
    Me.DataGridViewTextBoxColumn8.ReadOnly = True
    '
    'DataGridViewTextBoxColumn9
    '
    Me.DataGridViewTextBoxColumn9.DataPropertyName = "Name"
    Me.DataGridViewTextBoxColumn9.HeaderText = "Name"
    Me.DataGridViewTextBoxColumn9.Name = "DataGridViewTextBoxColumn9"
    Me.DataGridViewTextBoxColumn9.ReadOnly = True
    '
    'Label1
    '
    Me.Label1.AutoSize = True
    Me.Label1.Location = New System.Drawing.Point(12, 66)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(45, 13)
    Me.Label1.TabIndex = 3
    Me.Label1.Text = "Projects"
    '
    'Label2
    '
    Me.Label2.AutoSize = True
    Me.Label2.Location = New System.Drawing.Point(295, 66)
    Me.Label2.Name = "Label2"
    Me.Label2.Size = New System.Drawing.Size(204, 13)
    Me.Label2.TabIndex = 4
    Me.Label2.Text = "Resources (double-click to change name)"
    '
    'Label3
    '
    Me.Label3.AutoSize = True
    Me.Label3.Location = New System.Drawing.Point(598, 66)
    Me.Label3.Name = "Label3"
    Me.Label3.Size = New System.Drawing.Size(34, 13)
    Me.Label3.TabIndex = 5
    Me.Label3.Text = "Roles"
    '
    'GroupBox1
    '
    Me.GroupBox1.Controls.Add(Me.ProjectResourcesDataGridView)
    Me.GroupBox1.Controls.Add(Me.ClearProjectButton)
    Me.GroupBox1.Controls.Add(Me.SaveProjectButton)
    Me.GroupBox1.Controls.Add(DescriptionLabel)
    Me.GroupBox1.Controls.Add(Me.DescriptionTextBox)
    Me.GroupBox1.Controls.Add(EndedLabel)
    Me.GroupBox1.Controls.Add(Me.EndedTextBox)
    Me.GroupBox1.Controls.Add(StartedLabel)
    Me.GroupBox1.Controls.Add(Me.StartedTextBox)
    Me.GroupBox1.Controls.Add(NameLabel)
    Me.GroupBox1.Controls.Add(Me.NameTextBox)
    Me.GroupBox1.Controls.Add(IdLabel)
    Me.GroupBox1.Controls.Add(Me.IdLabel1)
    Me.GroupBox1.Location = New System.Drawing.Point(12, 308)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(583, 282)
    Me.GroupBox1.TabIndex = 6
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "Project"
    '
    'ProjectResourcesDataGridView
    '
    Me.ProjectResourcesDataGridView.AllowUserToAddRows = False
    Me.ProjectResourcesDataGridView.AllowUserToDeleteRows = False
    Me.ProjectResourcesDataGridView.AutoGenerateColumns = False
    Me.ProjectResourcesDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn4, Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn5, Me.DataGridViewTextBoxColumn10, Me.DataGridViewTextBoxColumn11})
    Me.ProjectResourcesDataGridView.DataSource = Me.ProjectResourcesBindingSource
    Me.ProjectResourcesDataGridView.Location = New System.Drawing.Point(257, 56)
    Me.ProjectResourcesDataGridView.MultiSelect = False
    Me.ProjectResourcesDataGridView.Name = "ProjectResourcesDataGridView"
    Me.ProjectResourcesDataGridView.ReadOnly = True
    Me.ProjectResourcesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
    Me.ProjectResourcesDataGridView.Size = New System.Drawing.Size(313, 220)
    Me.ProjectResourcesDataGridView.TabIndex = 12
    '
    'DataGridViewTextBoxColumn4
    '
    Me.DataGridViewTextBoxColumn4.DataPropertyName = "ResourceId"
    Me.DataGridViewTextBoxColumn4.HeaderText = "ResourceId"
    Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
    Me.DataGridViewTextBoxColumn4.ReadOnly = True
    Me.DataGridViewTextBoxColumn4.Visible = False
    '
    'DataGridViewTextBoxColumn3
    '
    Me.DataGridViewTextBoxColumn3.DataPropertyName = "LastName"
    Me.DataGridViewTextBoxColumn3.HeaderText = "LastName"
    Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
    Me.DataGridViewTextBoxColumn3.ReadOnly = True
    '
    'DataGridViewTextBoxColumn5
    '
    Me.DataGridViewTextBoxColumn5.DataPropertyName = "FirstName"
    Me.DataGridViewTextBoxColumn5.HeaderText = "FirstName"
    Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
    Me.DataGridViewTextBoxColumn5.ReadOnly = True
    '
    'DataGridViewTextBoxColumn10
    '
    Me.DataGridViewTextBoxColumn10.DataPropertyName = "Assigned"
    Me.DataGridViewTextBoxColumn10.HeaderText = "Assigned"
    Me.DataGridViewTextBoxColumn10.Name = "DataGridViewTextBoxColumn10"
    Me.DataGridViewTextBoxColumn10.ReadOnly = True
    '
    'DataGridViewTextBoxColumn11
    '
    Me.DataGridViewTextBoxColumn11.DataPropertyName = "Role"
    Me.DataGridViewTextBoxColumn11.DataSource = Me.RoleInfoBindingSource
    Me.DataGridViewTextBoxColumn11.DisplayMember = "Name"
    Me.DataGridViewTextBoxColumn11.HeaderText = "Role"
    Me.DataGridViewTextBoxColumn11.Name = "DataGridViewTextBoxColumn11"
    Me.DataGridViewTextBoxColumn11.ReadOnly = True
    Me.DataGridViewTextBoxColumn11.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
    Me.DataGridViewTextBoxColumn11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
    Me.DataGridViewTextBoxColumn11.ValueMember = "Id"
    '
    'ProjectResourcesBindingSource
    '
    Me.ProjectResourcesBindingSource.DataMember = "ProjectResources"
    Me.ProjectResourcesBindingSource.DataSource = Me.ProjectDetailBindingSource
    '
    'ProjectDetailBindingSource
    '
    Me.ProjectDetailBindingSource.DataSource = GetType(PTServiceClient.PTService.ProjectData)
    '
    'ClearProjectButton
    '
    Me.ClearProjectButton.Location = New System.Drawing.Point(95, 253)
    Me.ClearProjectButton.Name = "ClearProjectButton"
    Me.ClearProjectButton.Size = New System.Drawing.Size(75, 23)
    Me.ClearProjectButton.TabIndex = 11
    Me.ClearProjectButton.Text = "Clear"
    Me.ClearProjectButton.UseVisualStyleBackColor = True
    '
    'SaveProjectButton
    '
    Me.SaveProjectButton.Location = New System.Drawing.Point(176, 253)
    Me.SaveProjectButton.Name = "SaveProjectButton"
    Me.SaveProjectButton.Size = New System.Drawing.Size(75, 23)
    Me.SaveProjectButton.TabIndex = 10
    Me.SaveProjectButton.Text = "Save"
    Me.SaveProjectButton.UseVisualStyleBackColor = True
    '
    'DescriptionTextBox
    '
    Me.DescriptionTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectDetailBindingSource, "Description", True))
    Me.DescriptionTextBox.Location = New System.Drawing.Point(76, 145)
    Me.DescriptionTextBox.Multiline = True
    Me.DescriptionTextBox.Name = "DescriptionTextBox"
    Me.DescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
    Me.DescriptionTextBox.Size = New System.Drawing.Size(175, 102)
    Me.DescriptionTextBox.TabIndex = 9
    '
    'EndedTextBox
    '
    Me.EndedTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectDetailBindingSource, "Ended", True))
    Me.EndedTextBox.Location = New System.Drawing.Point(76, 114)
    Me.EndedTextBox.Name = "EndedTextBox"
    Me.EndedTextBox.Size = New System.Drawing.Size(175, 20)
    Me.EndedTextBox.TabIndex = 7
    '
    'StartedTextBox
    '
    Me.StartedTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectDetailBindingSource, "Started", True))
    Me.StartedTextBox.Location = New System.Drawing.Point(76, 85)
    Me.StartedTextBox.Name = "StartedTextBox"
    Me.StartedTextBox.Size = New System.Drawing.Size(175, 20)
    Me.StartedTextBox.TabIndex = 5
    '
    'NameTextBox
    '
    Me.NameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectDetailBindingSource, "Name", True))
    Me.NameTextBox.Location = New System.Drawing.Point(76, 56)
    Me.NameTextBox.Name = "NameTextBox"
    Me.NameTextBox.Size = New System.Drawing.Size(175, 20)
    Me.NameTextBox.TabIndex = 3
    '
    'IdLabel1
    '
    Me.IdLabel1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectDetailBindingSource, "Id", True))
    Me.IdLabel1.Location = New System.Drawing.Point(73, 30)
    Me.IdLabel1.Name = "IdLabel1"
    Me.IdLabel1.Size = New System.Drawing.Size(175, 23)
    Me.IdLabel1.TabIndex = 1
    '
    'GroupBox2
    '
    Me.GroupBox2.Controls.Add(Me.AssignToProjectButton)
    Me.GroupBox2.Controls.Add(NameLabel3)
    Me.GroupBox2.Controls.Add(Me.NameLabel4)
    Me.GroupBox2.Controls.Add(IdLabel4)
    Me.GroupBox2.Controls.Add(Me.ResourceIdLabel)
    Me.GroupBox2.Controls.Add(Me.Label5)
    Me.GroupBox2.Controls.Add(Me.Label4)
    Me.GroupBox2.Controls.Add(NameLabel1)
    Me.GroupBox2.Controls.Add(Me.NameLabel2)
    Me.GroupBox2.Controls.Add(IdLabel2)
    Me.GroupBox2.Controls.Add(Me.ProjectIdLabel)
    Me.GroupBox2.Location = New System.Drawing.Point(602, 308)
    Me.GroupBox2.Name = "GroupBox2"
    Me.GroupBox2.Size = New System.Drawing.Size(300, 282)
    Me.GroupBox2.TabIndex = 7
    Me.GroupBox2.TabStop = False
    Me.GroupBox2.Text = "Add assignment"
    '
    'AssignToProjectButton
    '
    Me.AssignToProjectButton.Location = New System.Drawing.Point(196, 205)
    Me.AssignToProjectButton.Name = "AssignToProjectButton"
    Me.AssignToProjectButton.Size = New System.Drawing.Size(75, 23)
    Me.AssignToProjectButton.TabIndex = 9
    Me.AssignToProjectButton.Text = "Assign"
    Me.AssignToProjectButton.UseVisualStyleBackColor = True
    '
    'NameLabel4
    '
    Me.NameLabel4.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ResourceInfoBindingSource, "Name", True))
    Me.NameLabel4.Location = New System.Drawing.Point(53, 153)
    Me.NameLabel4.Name = "NameLabel4"
    Me.NameLabel4.Size = New System.Drawing.Size(218, 23)
    Me.NameLabel4.TabIndex = 8
    '
    'ResourceIdLabel
    '
    Me.ResourceIdLabel.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ResourceInfoBindingSource, "Id", True))
    Me.ResourceIdLabel.Location = New System.Drawing.Point(53, 130)
    Me.ResourceIdLabel.Name = "ResourceIdLabel"
    Me.ResourceIdLabel.Size = New System.Drawing.Size(218, 23)
    Me.ResourceIdLabel.TabIndex = 6
    '
    'Label5
    '
    Me.Label5.AutoSize = True
    Me.Label5.Location = New System.Drawing.Point(9, 109)
    Me.Label5.Name = "Label5"
    Me.Label5.Size = New System.Drawing.Size(56, 13)
    Me.Label5.TabIndex = 4
    Me.Label5.Text = "Resource:"
    '
    'Label4
    '
    Me.Label4.AutoSize = True
    Me.Label4.Location = New System.Drawing.Point(9, 25)
    Me.Label4.Name = "Label4"
    Me.Label4.Size = New System.Drawing.Size(43, 13)
    Me.Label4.TabIndex = 4
    Me.Label4.Text = "Project:"
    '
    'NameLabel2
    '
    Me.NameLabel2.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectInfoBindingSource, "Name", True))
    Me.NameLabel2.Location = New System.Drawing.Point(53, 68)
    Me.NameLabel2.Name = "NameLabel2"
    Me.NameLabel2.Size = New System.Drawing.Size(227, 23)
    Me.NameLabel2.TabIndex = 3
    '
    'ProjectIdLabel
    '
    Me.ProjectIdLabel.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectInfoBindingSource, "Id", True))
    Me.ProjectIdLabel.Location = New System.Drawing.Point(53, 44)
    Me.ProjectIdLabel.Name = "ProjectIdLabel"
    Me.ProjectIdLabel.Size = New System.Drawing.Size(227, 23)
    Me.ProjectIdLabel.TabIndex = 1
    '
    'Label6
    '
    Me.Label6.AutoSize = True
    Me.Label6.Location = New System.Drawing.Point(12, 9)
    Me.Label6.Name = "Label6"
    Me.Label6.Size = New System.Drawing.Size(58, 13)
    Me.Label6.TabIndex = 8
    Me.Label6.Text = "Username:"
    '
    'UsernameTextBox
    '
    Me.UsernameTextBox.Location = New System.Drawing.Point(82, 6)
    Me.UsernameTextBox.Name = "UsernameTextBox"
    Me.UsernameTextBox.Size = New System.Drawing.Size(128, 20)
    Me.UsernameTextBox.TabIndex = 9
    '
    'Label7
    '
    Me.Label7.AutoSize = True
    Me.Label7.Location = New System.Drawing.Point(12, 31)
    Me.Label7.Name = "Label7"
    Me.Label7.Size = New System.Drawing.Size(56, 13)
    Me.Label7.TabIndex = 10
    Me.Label7.Text = "Password:"
    '
    'PasswordTextBox
    '
    Me.PasswordTextBox.Location = New System.Drawing.Point(82, 28)
    Me.PasswordTextBox.Name = "PasswordTextBox"
    Me.PasswordTextBox.Size = New System.Drawing.Size(128, 20)
    Me.PasswordTextBox.TabIndex = 11
    '
    'MainForm
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(914, 602)
    Me.Controls.Add(Me.PasswordTextBox)
    Me.Controls.Add(Me.Label7)
    Me.Controls.Add(Me.UsernameTextBox)
    Me.Controls.Add(Me.Label6)
    Me.Controls.Add(Me.GroupBox2)
    Me.Controls.Add(Me.GroupBox1)
    Me.Controls.Add(Me.Label3)
    Me.Controls.Add(Me.Label2)
    Me.Controls.Add(Me.Label1)
    Me.Controls.Add(Me.RoleInfoDataGridView)
    Me.Controls.Add(Me.ResourceInfoDataGridView)
    Me.Controls.Add(Me.ProjectInfoDataGridView)
    Me.Name = "MainForm"
    Me.Text = "Project Tracker Client"
    CType(Me.ProjectInfoBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ProjectInfoDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ResourceInfoBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ResourceInfoDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.RoleInfoBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.RoleInfoDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    Me.GroupBox1.ResumeLayout(False)
    Me.GroupBox1.PerformLayout()
    CType(Me.ProjectResourcesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ProjectResourcesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ProjectDetailBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    Me.GroupBox2.ResumeLayout(False)
    Me.GroupBox2.PerformLayout()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents ProjectInfoBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ProjectInfoDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents ResourceInfoBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ResourceInfoDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridViewTextBoxColumn6 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn7 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents RoleInfoBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents RoleInfoDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridViewTextBoxColumn8 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn9 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents Label2 As System.Windows.Forms.Label
  Friend WithEvents Label3 As System.Windows.Forms.Label
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents DescriptionTextBox As System.Windows.Forms.TextBox
  Friend WithEvents EndedTextBox As System.Windows.Forms.TextBox
  Friend WithEvents StartedTextBox As System.Windows.Forms.TextBox
  Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
  Friend WithEvents IdLabel1 As System.Windows.Forms.Label
  Friend WithEvents SaveProjectButton As System.Windows.Forms.Button
  Friend WithEvents ProjectDetailBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ClearProjectButton As System.Windows.Forms.Button
  Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
  Friend WithEvents NameLabel4 As System.Windows.Forms.Label
  Friend WithEvents ResourceIdLabel As System.Windows.Forms.Label
  Friend WithEvents Label4 As System.Windows.Forms.Label
  Friend WithEvents NameLabel2 As System.Windows.Forms.Label
  Friend WithEvents ProjectIdLabel As System.Windows.Forms.Label
  Friend WithEvents AssignToProjectButton As System.Windows.Forms.Button
  Friend WithEvents Label5 As System.Windows.Forms.Label
  Friend WithEvents ProjectResourcesDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents ProjectResourcesBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn10 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn11 As System.Windows.Forms.DataGridViewComboBoxColumn
  Friend WithEvents Label6 As System.Windows.Forms.Label
  Friend WithEvents UsernameTextBox As System.Windows.Forms.TextBox
  Friend WithEvents Label7 As System.Windows.Forms.Label
  Friend WithEvents PasswordTextBox As System.Windows.Forms.TextBox

End Class
