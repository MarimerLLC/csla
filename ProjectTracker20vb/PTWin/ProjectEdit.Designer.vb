<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProjectEdit

  'UserControl overrides dispose to clean up the component list.
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
    Dim DescriptionLabel As System.Windows.Forms.Label
    Dim EndedLabel As System.Windows.Forms.Label
    Dim IdLabel As System.Windows.Forms.Label
    Dim NameLabel As System.Windows.Forms.Label
    Dim StartedLabel As System.Windows.Forms.Label
    Me.ErrorProvider1 = New System.Windows.Forms.ErrorProvider(Me.components)
    Me.ProjectBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.BindingSourceRefresh1 = New Csla.Windows.BindingSourceRefresh(Me.components)
    Me.RoleListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ResourcesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.CloseButton = New System.Windows.Forms.Button
    Me.ApplyButton = New System.Windows.Forms.Button
    Me.Cancel_Button = New System.Windows.Forms.Button
    Me.OKButton = New System.Windows.Forms.Button
    Me.UnassignButton = New System.Windows.Forms.Button
    Me.GroupBox1 = New System.Windows.Forms.GroupBox
    Me.AssignButton = New System.Windows.Forms.Button
    Me.ResourcesDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.Column1 = New System.Windows.Forms.DataGridViewLinkColumn
    Me.Role = New System.Windows.Forms.DataGridViewComboBoxColumn
    Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DescriptionTextBox = New System.Windows.Forms.TextBox
    Me.EndedTextBox = New System.Windows.Forms.TextBox
    Me.IdLabel1 = New System.Windows.Forms.Label
    Me.NameTextBox = New System.Windows.Forms.TextBox
    Me.StartedTextBox = New System.Windows.Forms.TextBox
    Me.ReadWriteAuthorization1 = New Csla.Windows.ReadWriteAuthorization(Me.components)
    DescriptionLabel = New System.Windows.Forms.Label
    EndedLabel = New System.Windows.Forms.Label
    IdLabel = New System.Windows.Forms.Label
    NameLabel = New System.Windows.Forms.Label
    StartedLabel = New System.Windows.Forms.Label
    CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ProjectBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.RoleListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ResourcesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.GroupBox1.SuspendLayout()
    CType(Me.ResourcesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'DescriptionLabel
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(DescriptionLabel, False)
    DescriptionLabel.AutoSize = True
    DescriptionLabel.Location = New System.Drawing.Point(10, 120)
    DescriptionLabel.Name = "DescriptionLabel"
    DescriptionLabel.Size = New System.Drawing.Size(63, 13)
    DescriptionLabel.TabIndex = 16
    DescriptionLabel.Text = "Description:"
    '
    'EndedLabel
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(EndedLabel, False)
    EndedLabel.AutoSize = True
    EndedLabel.Location = New System.Drawing.Point(10, 94)
    EndedLabel.Name = "EndedLabel"
    EndedLabel.Size = New System.Drawing.Size(41, 13)
    EndedLabel.TabIndex = 18
    EndedLabel.Text = "Ended:"
    '
    'IdLabel
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(IdLabel, False)
    IdLabel.AutoSize = True
    IdLabel.Location = New System.Drawing.Point(10, 13)
    IdLabel.Name = "IdLabel"
    IdLabel.Size = New System.Drawing.Size(19, 13)
    IdLabel.TabIndex = 20
    IdLabel.Text = "Id:"
    '
    'NameLabel
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(NameLabel, False)
    NameLabel.AutoSize = True
    NameLabel.Location = New System.Drawing.Point(10, 42)
    NameLabel.Name = "NameLabel"
    NameLabel.Size = New System.Drawing.Size(38, 13)
    NameLabel.TabIndex = 22
    NameLabel.Text = "Name:"
    '
    'StartedLabel
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(StartedLabel, False)
    StartedLabel.AutoSize = True
    StartedLabel.Location = New System.Drawing.Point(10, 68)
    StartedLabel.Name = "StartedLabel"
    StartedLabel.Size = New System.Drawing.Size(44, 13)
    StartedLabel.TabIndex = 24
    StartedLabel.Text = "Started:"
    '
    'ErrorProvider1
    '
    Me.ErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
    Me.ErrorProvider1.ContainerControl = Me
    Me.ErrorProvider1.DataSource = Me.ProjectBindingSource
    '
    'ProjectBindingSource
    '
    Me.ProjectBindingSource.DataSource = GetType(ProjectTracker.Library.Project)
    Me.BindingSourceRefresh1.SetReadValuesOnChange(Me.ProjectBindingSource, True)
    '
    'RoleListBindingSource
    '
    Me.RoleListBindingSource.DataSource = GetType(ProjectTracker.Library.RoleList)
    Me.BindingSourceRefresh1.SetReadValuesOnChange(Me.RoleListBindingSource, False)
    '
    'ResourcesBindingSource
    '
    Me.ResourcesBindingSource.DataMember = "Resources"
    Me.ResourcesBindingSource.DataSource = Me.ProjectBindingSource
    Me.BindingSourceRefresh1.SetReadValuesOnChange(Me.ResourcesBindingSource, False)
    '
    'CloseButton
    '
    Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.CloseButton, False)
    Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.CloseButton.Location = New System.Drawing.Point(576, 100)
    Me.CloseButton.Name = "CloseButton"
    Me.CloseButton.Size = New System.Drawing.Size(75, 23)
    Me.CloseButton.TabIndex = 30
    Me.CloseButton.Text = "Close"
    Me.CloseButton.UseVisualStyleBackColor = True
    '
    'ApplyButton
    '
    Me.ApplyButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.ApplyButton, False)
    Me.ApplyButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.ApplyButton.Location = New System.Drawing.Point(576, 42)
    Me.ApplyButton.Name = "ApplyButton"
    Me.ApplyButton.Size = New System.Drawing.Size(75, 23)
    Me.ApplyButton.TabIndex = 29
    Me.ApplyButton.Text = "Apply"
    Me.ApplyButton.UseVisualStyleBackColor = True
    '
    'Cancel_Button
    '
    Me.Cancel_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.Cancel_Button, False)
    Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Cancel_Button.Location = New System.Drawing.Point(576, 71)
    Me.Cancel_Button.Name = "Cancel_Button"
    Me.Cancel_Button.Size = New System.Drawing.Size(75, 23)
    Me.Cancel_Button.TabIndex = 28
    Me.Cancel_Button.Text = "Cancel"
    Me.Cancel_Button.UseVisualStyleBackColor = True
    '
    'OKButton
    '
    Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.OKButton, False)
    Me.OKButton.Location = New System.Drawing.Point(576, 13)
    Me.OKButton.Name = "OKButton"
    Me.OKButton.Size = New System.Drawing.Size(75, 23)
    Me.OKButton.TabIndex = 27
    Me.OKButton.Text = "OK"
    Me.OKButton.UseVisualStyleBackColor = True
    '
    'UnassignButton
    '
    Me.UnassignButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.UnassignButton, False)
    Me.UnassignButton.Location = New System.Drawing.Point(379, 48)
    Me.UnassignButton.Name = "UnassignButton"
    Me.UnassignButton.Size = New System.Drawing.Size(75, 23)
    Me.UnassignButton.TabIndex = 12
    Me.UnassignButton.Text = "Unassign"
    Me.UnassignButton.UseVisualStyleBackColor = True
    '
    'GroupBox1
    '
    Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.GroupBox1, False)
    Me.GroupBox1.Controls.Add(Me.UnassignButton)
    Me.GroupBox1.Controls.Add(Me.AssignButton)
    Me.GroupBox1.Controls.Add(Me.ResourcesDataGridView)
    Me.GroupBox1.Location = New System.Drawing.Point(79, 231)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(460, 210)
    Me.GroupBox1.TabIndex = 26
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "Resources"
    '
    'AssignButton
    '
    Me.AssignButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.AssignButton, False)
    Me.AssignButton.Location = New System.Drawing.Point(379, 19)
    Me.AssignButton.Name = "AssignButton"
    Me.AssignButton.Size = New System.Drawing.Size(75, 23)
    Me.AssignButton.TabIndex = 11
    Me.AssignButton.Text = "Assign"
    Me.AssignButton.UseVisualStyleBackColor = True
    '
    'ResourcesDataGridView
    '
    Me.ResourcesDataGridView.AllowUserToAddRows = False
    Me.ResourcesDataGridView.AllowUserToDeleteRows = False
    Me.ResourcesDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.ResourcesDataGridView, False)
    Me.ResourcesDataGridView.AutoGenerateColumns = False
    Me.ResourcesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
    Me.ResourcesDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn2, Me.Column1, Me.Role, Me.DataGridViewTextBoxColumn4})
    Me.ResourcesDataGridView.DataSource = Me.ResourcesBindingSource
    Me.ResourcesDataGridView.Location = New System.Drawing.Point(6, 19)
    Me.ResourcesDataGridView.MultiSelect = False
    Me.ResourcesDataGridView.Name = "ResourcesDataGridView"
    Me.ResourcesDataGridView.RowHeadersVisible = False
    Me.ResourcesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
    Me.ResourcesDataGridView.Size = New System.Drawing.Size(367, 185)
    Me.ResourcesDataGridView.TabIndex = 10
    '
    'DataGridViewTextBoxColumn2
    '
    Me.DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
    Me.DataGridViewTextBoxColumn2.DataPropertyName = "ResourceId"
    Me.DataGridViewTextBoxColumn2.HeaderText = "ResourceId"
    Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
    Me.DataGridViewTextBoxColumn2.ReadOnly = True
    Me.DataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
    Me.DataGridViewTextBoxColumn2.Visible = False
    Me.DataGridViewTextBoxColumn2.Width = 5
    '
    'Column1
    '
    Me.Column1.DataPropertyName = "FullName"
    Me.Column1.HeaderText = "Name"
    Me.Column1.Name = "Column1"
    Me.Column1.ReadOnly = True
    Me.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
    Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
    Me.Column1.Width = 60
    '
    'Role
    '
    Me.Role.DataPropertyName = "Role"
    Me.Role.DataSource = Me.RoleListBindingSource
    Me.Role.DisplayMember = "Value"
    Me.Role.HeaderText = "Role"
    Me.Role.Name = "Role"
    Me.Role.ValueMember = "Key"
    Me.Role.Width = 35
    '
    'DataGridViewTextBoxColumn4
    '
    Me.DataGridViewTextBoxColumn4.DataPropertyName = "Assigned"
    Me.DataGridViewTextBoxColumn4.HeaderText = "Assigned"
    Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
    Me.DataGridViewTextBoxColumn4.ReadOnly = True
    Me.DataGridViewTextBoxColumn4.Width = 75
    '
    'DescriptionTextBox
    '
    Me.DescriptionTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.DescriptionTextBox, True)
    Me.DescriptionTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Description", True))
    Me.DescriptionTextBox.Location = New System.Drawing.Point(79, 117)
    Me.DescriptionTextBox.Multiline = True
    Me.DescriptionTextBox.Name = "DescriptionTextBox"
    Me.DescriptionTextBox.Size = New System.Drawing.Size(460, 108)
    Me.DescriptionTextBox.TabIndex = 17
    '
    'EndedTextBox
    '
    Me.EndedTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.EndedTextBox, True)
    Me.EndedTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Ended", True))
    Me.EndedTextBox.Location = New System.Drawing.Point(79, 91)
    Me.EndedTextBox.Name = "EndedTextBox"
    Me.EndedTextBox.Size = New System.Drawing.Size(460, 20)
    Me.EndedTextBox.TabIndex = 19
    '
    'IdLabel1
    '
    Me.IdLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.IdLabel1, True)
    Me.IdLabel1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Id", True))
    Me.IdLabel1.Location = New System.Drawing.Point(79, 13)
    Me.IdLabel1.Name = "IdLabel1"
    Me.IdLabel1.Size = New System.Drawing.Size(460, 23)
    Me.IdLabel1.TabIndex = 21
    '
    'NameTextBox
    '
    Me.NameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.NameTextBox, True)
    Me.NameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Name", True))
    Me.NameTextBox.Location = New System.Drawing.Point(79, 39)
    Me.NameTextBox.Name = "NameTextBox"
    Me.NameTextBox.Size = New System.Drawing.Size(460, 20)
    Me.NameTextBox.TabIndex = 23
    '
    'StartedTextBox
    '
    Me.StartedTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.StartedTextBox, True)
    Me.StartedTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Started", True))
    Me.StartedTextBox.Location = New System.Drawing.Point(79, 65)
    Me.StartedTextBox.Name = "StartedTextBox"
    Me.StartedTextBox.Size = New System.Drawing.Size(460, 20)
    Me.StartedTextBox.TabIndex = 25
    '
    'ProjectEdit
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me, False)
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.Controls.Add(Me.CloseButton)
    Me.Controls.Add(Me.ApplyButton)
    Me.Controls.Add(Me.Cancel_Button)
    Me.Controls.Add(Me.OKButton)
    Me.Controls.Add(Me.GroupBox1)
    Me.Controls.Add(DescriptionLabel)
    Me.Controls.Add(Me.DescriptionTextBox)
    Me.Controls.Add(EndedLabel)
    Me.Controls.Add(IdLabel)
    Me.Controls.Add(NameLabel)
    Me.Controls.Add(StartedLabel)
    Me.Controls.Add(Me.EndedTextBox)
    Me.Controls.Add(Me.IdLabel1)
    Me.Controls.Add(Me.NameTextBox)
    Me.Controls.Add(Me.StartedTextBox)
    Me.Name = "ProjectEdit"
    Me.Size = New System.Drawing.Size(665, 454)
    CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ProjectBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.RoleListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ResourcesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    Me.GroupBox1.ResumeLayout(False)
    CType(Me.ResourcesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents ErrorProvider1 As System.Windows.Forms.ErrorProvider
  Friend WithEvents CloseButton As System.Windows.Forms.Button
  Friend WithEvents ApplyButton As System.Windows.Forms.Button
  Friend WithEvents Cancel_Button As System.Windows.Forms.Button
  Friend WithEvents OKButton As System.Windows.Forms.Button
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents UnassignButton As System.Windows.Forms.Button
  Friend WithEvents AssignButton As System.Windows.Forms.Button
  Friend WithEvents ResourcesDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents RoleListBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents BindingSourceRefresh1 As Csla.Windows.BindingSourceRefresh
  Friend WithEvents ResourcesBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ProjectBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents DescriptionTextBox As System.Windows.Forms.TextBox
  Friend WithEvents EndedTextBox As System.Windows.Forms.TextBox
  Friend WithEvents IdLabel1 As System.Windows.Forms.Label
  Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
  Friend WithEvents StartedTextBox As System.Windows.Forms.TextBox
  Friend WithEvents ReadWriteAuthorization1 As Csla.Windows.ReadWriteAuthorization
  Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents Column1 As System.Windows.Forms.DataGridViewLinkColumn
  Friend WithEvents Role As System.Windows.Forms.DataGridViewComboBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
