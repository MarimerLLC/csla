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
    Dim DescriptionLabel As System.Windows.Forms.Label
    Dim EndedLabel As System.Windows.Forms.Label
    Dim IdLabel As System.Windows.Forms.Label
    Dim NameLabel As System.Windows.Forms.Label
    Dim StartedLabel As System.Windows.Forms.Label
    Me.ProjectListDataGridView = New System.Windows.Forms.DataGridView
    Me.GroupBox1 = New System.Windows.Forms.GroupBox
    Me.DescriptionTextBox = New System.Windows.Forms.TextBox
    Me.EndedTextBox = New System.Windows.Forms.TextBox
    Me.IdLabel1 = New System.Windows.Forms.Label
    Me.NameTextBox = New System.Windows.Forms.TextBox
    Me.StartedTextBox = New System.Windows.Forms.TextBox
    Me.ResourcesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ResourcesDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn6 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn7 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.ProjectBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ProjectListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn
    DescriptionLabel = New System.Windows.Forms.Label
    EndedLabel = New System.Windows.Forms.Label
    IdLabel = New System.Windows.Forms.Label
    NameLabel = New System.Windows.Forms.Label
    StartedLabel = New System.Windows.Forms.Label
    CType(Me.ProjectListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.GroupBox1.SuspendLayout()
    CType(Me.ResourcesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ResourcesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ProjectBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ProjectListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'ProjectListDataGridView
    '
    Me.ProjectListDataGridView.AllowUserToAddRows = False
    Me.ProjectListDataGridView.AllowUserToDeleteRows = False
    Me.ProjectListDataGridView.AutoGenerateColumns = False
    Me.ProjectListDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
    Me.ProjectListDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2})
    Me.ProjectListDataGridView.DataSource = Me.ProjectListBindingSource
    Me.ProjectListDataGridView.Location = New System.Drawing.Point(12, 12)
    Me.ProjectListDataGridView.MultiSelect = False
    Me.ProjectListDataGridView.Name = "ProjectListDataGridView"
    Me.ProjectListDataGridView.ReadOnly = True
    Me.ProjectListDataGridView.RowHeadersVisible = False
    Me.ProjectListDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
    Me.ProjectListDataGridView.Size = New System.Drawing.Size(300, 467)
    Me.ProjectListDataGridView.TabIndex = 1
    '
    'GroupBox1
    '
    Me.GroupBox1.Controls.Add(Me.ResourcesDataGridView)
    Me.GroupBox1.Controls.Add(DescriptionLabel)
    Me.GroupBox1.Controls.Add(Me.DescriptionTextBox)
    Me.GroupBox1.Controls.Add(EndedLabel)
    Me.GroupBox1.Controls.Add(Me.EndedTextBox)
    Me.GroupBox1.Controls.Add(IdLabel)
    Me.GroupBox1.Controls.Add(Me.IdLabel1)
    Me.GroupBox1.Controls.Add(NameLabel)
    Me.GroupBox1.Controls.Add(Me.NameTextBox)
    Me.GroupBox1.Controls.Add(StartedLabel)
    Me.GroupBox1.Controls.Add(Me.StartedTextBox)
    Me.GroupBox1.Location = New System.Drawing.Point(318, 12)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(538, 522)
    Me.GroupBox1.TabIndex = 2
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "Project"
    '
    'DescriptionLabel
    '
    DescriptionLabel.AutoSize = True
    DescriptionLabel.Location = New System.Drawing.Point(27, 132)
    DescriptionLabel.Name = "DescriptionLabel"
    DescriptionLabel.Size = New System.Drawing.Size(63, 13)
    DescriptionLabel.TabIndex = 0
    DescriptionLabel.Text = "Description:"
    '
    'DescriptionTextBox
    '
    Me.DescriptionTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Description", True))
    Me.DescriptionTextBox.Location = New System.Drawing.Point(96, 129)
    Me.DescriptionTextBox.Multiline = True
    Me.DescriptionTextBox.Name = "DescriptionTextBox"
    Me.DescriptionTextBox.Size = New System.Drawing.Size(422, 109)
    Me.DescriptionTextBox.TabIndex = 1
    '
    'EndedLabel
    '
    EndedLabel.AutoSize = True
    EndedLabel.Location = New System.Drawing.Point(27, 106)
    EndedLabel.Name = "EndedLabel"
    EndedLabel.Size = New System.Drawing.Size(41, 13)
    EndedLabel.TabIndex = 2
    EndedLabel.Text = "Ended:"
    '
    'EndedTextBox
    '
    Me.EndedTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Ended", True))
    Me.EndedTextBox.Location = New System.Drawing.Point(96, 103)
    Me.EndedTextBox.Name = "EndedTextBox"
    Me.EndedTextBox.Size = New System.Drawing.Size(422, 20)
    Me.EndedTextBox.TabIndex = 3
    '
    'IdLabel
    '
    IdLabel.AutoSize = True
    IdLabel.Location = New System.Drawing.Point(27, 25)
    IdLabel.Name = "IdLabel"
    IdLabel.Size = New System.Drawing.Size(19, 13)
    IdLabel.TabIndex = 4
    IdLabel.Text = "Id:"
    '
    'IdLabel1
    '
    Me.IdLabel1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Id", True))
    Me.IdLabel1.Location = New System.Drawing.Point(93, 25)
    Me.IdLabel1.Name = "IdLabel1"
    Me.IdLabel1.Size = New System.Drawing.Size(422, 23)
    Me.IdLabel1.TabIndex = 5
    '
    'NameLabel
    '
    NameLabel.AutoSize = True
    NameLabel.Location = New System.Drawing.Point(27, 54)
    NameLabel.Name = "NameLabel"
    NameLabel.Size = New System.Drawing.Size(38, 13)
    NameLabel.TabIndex = 6
    NameLabel.Text = "Name:"
    '
    'NameTextBox
    '
    Me.NameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Name", True))
    Me.NameTextBox.Location = New System.Drawing.Point(96, 51)
    Me.NameTextBox.Name = "NameTextBox"
    Me.NameTextBox.Size = New System.Drawing.Size(422, 20)
    Me.NameTextBox.TabIndex = 7
    '
    'StartedLabel
    '
    StartedLabel.AutoSize = True
    StartedLabel.Location = New System.Drawing.Point(27, 80)
    StartedLabel.Name = "StartedLabel"
    StartedLabel.Size = New System.Drawing.Size(44, 13)
    StartedLabel.TabIndex = 8
    StartedLabel.Text = "Started:"
    '
    'StartedTextBox
    '
    Me.StartedTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ProjectBindingSource, "Started", True))
    Me.StartedTextBox.Location = New System.Drawing.Point(96, 77)
    Me.StartedTextBox.Name = "StartedTextBox"
    Me.StartedTextBox.Size = New System.Drawing.Size(422, 20)
    Me.StartedTextBox.TabIndex = 9
    '
    'ResourcesBindingSource
    '
    Me.ResourcesBindingSource.DataMember = "Resources"
    Me.ResourcesBindingSource.DataSource = Me.ProjectBindingSource
    '
    'ResourcesDataGridView
    '
    Me.ResourcesDataGridView.AutoGenerateColumns = False
    Me.ResourcesDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn4, Me.DataGridViewTextBoxColumn5, Me.DataGridViewTextBoxColumn6, Me.DataGridViewTextBoxColumn7})
    Me.ResourcesDataGridView.DataSource = Me.ResourcesBindingSource
    Me.ResourcesDataGridView.Location = New System.Drawing.Point(6, 247)
    Me.ResourcesDataGridView.Name = "ResourcesDataGridView"
    Me.ResourcesDataGridView.Size = New System.Drawing.Size(509, 220)
    Me.ResourcesDataGridView.TabIndex = 10
    '
    'DataGridViewTextBoxColumn3
    '
    Me.DataGridViewTextBoxColumn3.DataPropertyName = "LastName"
    Me.DataGridViewTextBoxColumn3.HeaderText = "LastName"
    Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
    '
    'DataGridViewTextBoxColumn4
    '
    Me.DataGridViewTextBoxColumn4.DataPropertyName = "ResourceId"
    Me.DataGridViewTextBoxColumn4.HeaderText = "ResourceId"
    Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
    '
    'DataGridViewTextBoxColumn5
    '
    Me.DataGridViewTextBoxColumn5.DataPropertyName = "FirstName"
    Me.DataGridViewTextBoxColumn5.HeaderText = "FirstName"
    Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
    '
    'DataGridViewTextBoxColumn6
    '
    Me.DataGridViewTextBoxColumn6.DataPropertyName = "Assigned"
    Me.DataGridViewTextBoxColumn6.HeaderText = "Assigned"
    Me.DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6"
    Me.DataGridViewTextBoxColumn6.ReadOnly = True
    '
    'DataGridViewTextBoxColumn7
    '
    Me.DataGridViewTextBoxColumn7.DataPropertyName = "Role"
    Me.DataGridViewTextBoxColumn7.HeaderText = "Role"
    Me.DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7"
    '
    'ProjectBindingSource
    '
    Me.ProjectBindingSource.DataSource = GetType(ProjectTracker.Library.Project)
    '
    'ProjectListBindingSource
    '
    Me.ProjectListBindingSource.DataSource = GetType(ProjectTracker.Library.ProjectList)
    '
    'DataGridViewTextBoxColumn1
    '
    Me.DataGridViewTextBoxColumn1.DataPropertyName = "ID"
    Me.DataGridViewTextBoxColumn1.HeaderText = "ID"
    Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
    Me.DataGridViewTextBoxColumn1.ReadOnly = True
    Me.DataGridViewTextBoxColumn1.Visible = False
    Me.DataGridViewTextBoxColumn1.Width = 43
    '
    'DataGridViewTextBoxColumn2
    '
    Me.DataGridViewTextBoxColumn2.DataPropertyName = "Name"
    Me.DataGridViewTextBoxColumn2.HeaderText = "Name"
    Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
    Me.DataGridViewTextBoxColumn2.ReadOnly = True
    Me.DataGridViewTextBoxColumn2.Width = 60
    '
    'MainForm
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(856, 536)
    Me.Controls.Add(Me.GroupBox1)
    Me.Controls.Add(Me.ProjectListDataGridView)
    Me.Name = "MainForm"
    Me.Text = "MainForm"
    CType(Me.ProjectListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    Me.GroupBox1.ResumeLayout(False)
    Me.GroupBox1.PerformLayout()
    CType(Me.ResourcesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ResourcesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ProjectBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ProjectListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents ProjectListBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ProjectListDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents ResourcesDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn6 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn7 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents ResourcesBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ProjectBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents DescriptionTextBox As System.Windows.Forms.TextBox
  Friend WithEvents EndedTextBox As System.Windows.Forms.TextBox
  Friend WithEvents IdLabel1 As System.Windows.Forms.Label
  Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
  Friend WithEvents StartedTextBox As System.Windows.Forms.TextBox
  Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
