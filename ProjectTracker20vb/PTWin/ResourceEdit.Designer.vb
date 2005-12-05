<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class ResourceEdit

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
    Dim FirstNameLabel As System.Windows.Forms.Label
    Dim IdLabel As System.Windows.Forms.Label
    Dim LastNameLabel As System.Windows.Forms.Label
    Me.RoleListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ResourceBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.FirstNameTextBox = New System.Windows.Forms.TextBox
    Me.IdLabel1 = New System.Windows.Forms.Label
    Me.LastNameTextBox = New System.Windows.Forms.TextBox
    Me.GroupBox1 = New System.Windows.Forms.GroupBox
    Me.UnassignButton = New System.Windows.Forms.Button
    Me.AssignButton = New System.Windows.Forms.Button
    Me.AssignmentsDataGridView = New System.Windows.Forms.DataGridView
    Me.AssignmentsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ErrorProvider1 = New System.Windows.Forms.ErrorProvider(Me.components)
    Me.BindingSourceRefresh1 = New Csla.Windows.BindingSourceRefresh(Me.components)
    Me.ReadWriteAuthorization1 = New Csla.Windows.ReadWriteAuthorization(Me.components)
    Me.CloseButton = New System.Windows.Forms.Button
    Me.ApplyButton = New System.Windows.Forms.Button
    Me.Cancel_Button = New System.Windows.Forms.Button
    Me.OKButton = New System.Windows.Forms.Button
    Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewLinkColumn
    Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.Role = New System.Windows.Forms.DataGridViewComboBoxColumn
    FirstNameLabel = New System.Windows.Forms.Label
    IdLabel = New System.Windows.Forms.Label
    LastNameLabel = New System.Windows.Forms.Label
    CType(Me.RoleListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ResourceBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.GroupBox1.SuspendLayout()
    CType(Me.AssignmentsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.AssignmentsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'FirstNameLabel
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(FirstNameLabel, False)
    FirstNameLabel.AutoSize = True
    FirstNameLabel.Location = New System.Drawing.Point(11, 42)
    FirstNameLabel.Name = "FirstNameLabel"
    FirstNameLabel.Size = New System.Drawing.Size(60, 13)
    FirstNameLabel.TabIndex = 0
    FirstNameLabel.Text = "First Name:"
    '
    'IdLabel
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(IdLabel, False)
    IdLabel.AutoSize = True
    IdLabel.Location = New System.Drawing.Point(11, 13)
    IdLabel.Name = "IdLabel"
    IdLabel.Size = New System.Drawing.Size(19, 13)
    IdLabel.TabIndex = 2
    IdLabel.Text = "Id:"
    '
    'LastNameLabel
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(LastNameLabel, False)
    LastNameLabel.AutoSize = True
    LastNameLabel.Location = New System.Drawing.Point(11, 68)
    LastNameLabel.Name = "LastNameLabel"
    LastNameLabel.Size = New System.Drawing.Size(61, 13)
    LastNameLabel.TabIndex = 4
    LastNameLabel.Text = "Last Name:"
    '
    'RoleListBindingSource
    '
    Me.RoleListBindingSource.DataSource = GetType(ProjectTracker.Library.RoleList)
    Me.BindingSourceRefresh1.SetReadValuesOnChange(Me.RoleListBindingSource, False)
    '
    'ResourceBindingSource
    '
    Me.ResourceBindingSource.DataSource = GetType(ProjectTracker.Library.Resource)
    Me.BindingSourceRefresh1.SetReadValuesOnChange(Me.ResourceBindingSource, True)
    '
    'FirstNameTextBox
    '
    Me.FirstNameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.FirstNameTextBox, True)
    Me.FirstNameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ResourceBindingSource, "FirstName", True))
    Me.FirstNameTextBox.Location = New System.Drawing.Point(78, 39)
    Me.FirstNameTextBox.Name = "FirstNameTextBox"
    Me.FirstNameTextBox.Size = New System.Drawing.Size(385, 20)
    Me.FirstNameTextBox.TabIndex = 1
    '
    'IdLabel1
    '
    Me.IdLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.IdLabel1, False)
    Me.IdLabel1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ResourceBindingSource, "Id", True))
    Me.IdLabel1.Location = New System.Drawing.Point(78, 13)
    Me.IdLabel1.Name = "IdLabel1"
    Me.IdLabel1.Size = New System.Drawing.Size(385, 23)
    Me.IdLabel1.TabIndex = 3
    '
    'LastNameTextBox
    '
    Me.LastNameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.LastNameTextBox, True)
    Me.LastNameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ResourceBindingSource, "LastName", True))
    Me.LastNameTextBox.Location = New System.Drawing.Point(78, 65)
    Me.LastNameTextBox.Name = "LastNameTextBox"
    Me.LastNameTextBox.Size = New System.Drawing.Size(385, 20)
    Me.LastNameTextBox.TabIndex = 5
    '
    'GroupBox1
    '
    Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.GroupBox1, False)
    Me.GroupBox1.Controls.Add(Me.UnassignButton)
    Me.GroupBox1.Controls.Add(Me.AssignButton)
    Me.GroupBox1.Controls.Add(Me.AssignmentsDataGridView)
    Me.GroupBox1.Location = New System.Drawing.Point(14, 91)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(449, 310)
    Me.GroupBox1.TabIndex = 6
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "Assigned projects"
    '
    'UnassignButton
    '
    Me.UnassignButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.UnassignButton, False)
    Me.UnassignButton.Location = New System.Drawing.Point(368, 48)
    Me.UnassignButton.Name = "UnassignButton"
    Me.UnassignButton.Size = New System.Drawing.Size(75, 23)
    Me.UnassignButton.TabIndex = 14
    Me.UnassignButton.Text = "Unassign"
    Me.UnassignButton.UseVisualStyleBackColor = True
    '
    'AssignButton
    '
    Me.AssignButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.AssignButton, False)
    Me.AssignButton.Location = New System.Drawing.Point(368, 19)
    Me.AssignButton.Name = "AssignButton"
    Me.AssignButton.Size = New System.Drawing.Size(75, 23)
    Me.AssignButton.TabIndex = 13
    Me.AssignButton.Text = "Assign"
    Me.AssignButton.UseVisualStyleBackColor = True
    '
    'AssignmentsDataGridView
    '
    Me.AssignmentsDataGridView.AllowUserToAddRows = False
    Me.AssignmentsDataGridView.AllowUserToDeleteRows = False
    Me.AssignmentsDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.AssignmentsDataGridView, False)
    Me.AssignmentsDataGridView.AutoGenerateColumns = False
    Me.AssignmentsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
    Me.AssignmentsDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn1, Me.Role})
    Me.AssignmentsDataGridView.DataSource = Me.AssignmentsBindingSource
    Me.AssignmentsDataGridView.Location = New System.Drawing.Point(6, 19)
    Me.AssignmentsDataGridView.MultiSelect = False
    Me.AssignmentsDataGridView.Name = "AssignmentsDataGridView"
    Me.AssignmentsDataGridView.RowHeadersVisible = False
    Me.AssignmentsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
    Me.AssignmentsDataGridView.Size = New System.Drawing.Size(356, 285)
    Me.AssignmentsDataGridView.TabIndex = 0
    '
    'AssignmentsBindingSource
    '
    Me.AssignmentsBindingSource.DataMember = "Assignments"
    Me.AssignmentsBindingSource.DataSource = Me.ResourceBindingSource
    Me.BindingSourceRefresh1.SetReadValuesOnChange(Me.AssignmentsBindingSource, False)
    '
    'ErrorProvider1
    '
    Me.ErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
    Me.ErrorProvider1.ContainerControl = Me
    Me.ErrorProvider1.DataSource = Me.ResourceBindingSource
    '
    'CloseButton
    '
    Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.CloseButton, False)
    Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.CloseButton.Location = New System.Drawing.Point(499, 100)
    Me.CloseButton.Name = "CloseButton"
    Me.CloseButton.Size = New System.Drawing.Size(75, 23)
    Me.CloseButton.TabIndex = 34
    Me.CloseButton.Text = "Close"
    Me.CloseButton.UseVisualStyleBackColor = True
    '
    'ApplyButton
    '
    Me.ApplyButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.ApplyButton, False)
    Me.ApplyButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.ApplyButton.Location = New System.Drawing.Point(499, 42)
    Me.ApplyButton.Name = "ApplyButton"
    Me.ApplyButton.Size = New System.Drawing.Size(75, 23)
    Me.ApplyButton.TabIndex = 33
    Me.ApplyButton.Text = "Apply"
    Me.ApplyButton.UseVisualStyleBackColor = True
    '
    'Cancel_Button
    '
    Me.Cancel_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.Cancel_Button, False)
    Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Cancel_Button.Location = New System.Drawing.Point(499, 71)
    Me.Cancel_Button.Name = "Cancel_Button"
    Me.Cancel_Button.Size = New System.Drawing.Size(75, 23)
    Me.Cancel_Button.TabIndex = 32
    Me.Cancel_Button.Text = "Cancel"
    Me.Cancel_Button.UseVisualStyleBackColor = True
    '
    'OKButton
    '
    Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me.OKButton, False)
    Me.OKButton.Location = New System.Drawing.Point(499, 13)
    Me.OKButton.Name = "OKButton"
    Me.OKButton.Size = New System.Drawing.Size(75, 23)
    Me.OKButton.TabIndex = 31
    Me.OKButton.Text = "OK"
    Me.OKButton.UseVisualStyleBackColor = True
    '
    'DataGridViewTextBoxColumn3
    '
    Me.DataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
    Me.DataGridViewTextBoxColumn3.DataPropertyName = "ProjectId"
    Me.DataGridViewTextBoxColumn3.HeaderText = "ProjectId"
    Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
    Me.DataGridViewTextBoxColumn3.ReadOnly = True
    Me.DataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
    Me.DataGridViewTextBoxColumn3.Visible = False
    Me.DataGridViewTextBoxColumn3.Width = 5
    '
    'DataGridViewTextBoxColumn2
    '
    Me.DataGridViewTextBoxColumn2.DataPropertyName = "ProjectName"
    Me.DataGridViewTextBoxColumn2.HeaderText = "Project Name"
    Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
    Me.DataGridViewTextBoxColumn2.ReadOnly = True
    Me.DataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
    Me.DataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
    Me.DataGridViewTextBoxColumn2.Width = 96
    '
    'DataGridViewTextBoxColumn1
    '
    Me.DataGridViewTextBoxColumn1.DataPropertyName = "Assigned"
    Me.DataGridViewTextBoxColumn1.HeaderText = "Assigned"
    Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
    Me.DataGridViewTextBoxColumn1.ReadOnly = True
    Me.DataGridViewTextBoxColumn1.Width = 75
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
    'ResourceEdit
    '
    Me.ReadWriteAuthorization1.SetApplyAuthorization(Me, False)
    Me.Controls.Add(Me.CloseButton)
    Me.Controls.Add(Me.ApplyButton)
    Me.Controls.Add(Me.Cancel_Button)
    Me.Controls.Add(Me.OKButton)
    Me.Controls.Add(Me.GroupBox1)
    Me.Controls.Add(FirstNameLabel)
    Me.Controls.Add(Me.FirstNameTextBox)
    Me.Controls.Add(IdLabel)
    Me.Controls.Add(Me.IdLabel1)
    Me.Controls.Add(LastNameLabel)
    Me.Controls.Add(Me.LastNameTextBox)
    Me.Name = "ResourceEdit"
    Me.Size = New System.Drawing.Size(588, 415)
    CType(Me.RoleListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ResourceBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    Me.GroupBox1.ResumeLayout(False)
    CType(Me.AssignmentsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.AssignmentsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents RoleListBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ResourceBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents FirstNameTextBox As System.Windows.Forms.TextBox
  Friend WithEvents IdLabel1 As System.Windows.Forms.Label
  Friend WithEvents LastNameTextBox As System.Windows.Forms.TextBox
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents AssignmentsDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents AssignmentsBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ErrorProvider1 As System.Windows.Forms.ErrorProvider
  Friend WithEvents BindingSourceRefresh1 As Csla.Windows.BindingSourceRefresh
  Friend WithEvents ReadWriteAuthorization1 As Csla.Windows.ReadWriteAuthorization
  Friend WithEvents CloseButton As System.Windows.Forms.Button
  Friend WithEvents ApplyButton As System.Windows.Forms.Button
  Friend WithEvents Cancel_Button As System.Windows.Forms.Button
  Friend WithEvents OKButton As System.Windows.Forms.Button
  Friend WithEvents UnassignButton As System.Windows.Forms.Button
  Friend WithEvents AssignButton As System.Windows.Forms.Button
  Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewLinkColumn
  Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents Role As System.Windows.Forms.DataGridViewComboBoxColumn

End Class
