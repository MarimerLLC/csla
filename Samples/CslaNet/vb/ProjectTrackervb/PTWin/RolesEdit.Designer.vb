<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RolesEdit

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
    Me.RolesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.RolesDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.SaveButton = New System.Windows.Forms.Button
    Me.CancelButton = New System.Windows.Forms.Button
    CType(Me.RolesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.RolesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'RolesBindingSource
    '
    Me.RolesBindingSource.DataSource = GetType(ProjectTracker.Library.Admin.Roles)
    '
    'RolesDataGridView
    '
    Me.RolesDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.RolesDataGridView.AutoGenerateColumns = False
    Me.RolesDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2})
    Me.RolesDataGridView.DataSource = Me.RolesBindingSource
    Me.RolesDataGridView.Location = New System.Drawing.Point(12, 13)
    Me.RolesDataGridView.MultiSelect = False
    Me.RolesDataGridView.Name = "RolesDataGridView"
    Me.RolesDataGridView.Size = New System.Drawing.Size(435, 323)
    Me.RolesDataGridView.TabIndex = 1
    '
    'DataGridViewTextBoxColumn1
    '
    Me.DataGridViewTextBoxColumn1.DataPropertyName = "Id"
    Me.DataGridViewTextBoxColumn1.HeaderText = "Id"
    Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
    '
    'DataGridViewTextBoxColumn2
    '
    Me.DataGridViewTextBoxColumn2.DataPropertyName = "Name"
    Me.DataGridViewTextBoxColumn2.HeaderText = "Name"
    Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
    '
    'SaveButton
    '
    Me.SaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.SaveButton.Location = New System.Drawing.Point(453, 13)
    Me.SaveButton.Name = "SaveButton"
    Me.SaveButton.Size = New System.Drawing.Size(75, 23)
    Me.SaveButton.TabIndex = 2
    Me.SaveButton.Text = "Save"
    Me.SaveButton.UseVisualStyleBackColor = True
    '
    'CancelButton
    '
    Me.CancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.CancelButton.Location = New System.Drawing.Point(453, 42)
    Me.CancelButton.Name = "CancelButton"
    Me.CancelButton.Size = New System.Drawing.Size(75, 23)
    Me.CancelButton.TabIndex = 3
    Me.CancelButton.Text = "Cancel"
    Me.CancelButton.UseVisualStyleBackColor = True
    '
    'RolesEdit
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.Controls.Add(Me.CancelButton)
    Me.Controls.Add(Me.SaveButton)
    Me.Controls.Add(Me.RolesDataGridView)
    Me.Name = "RolesEdit"
    Me.Size = New System.Drawing.Size(541, 348)
    CType(Me.RolesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.RolesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents RolesBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents RolesDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents SaveButton As System.Windows.Forms.Button
  Friend WithEvents CancelButton As System.Windows.Forms.Button

End Class
