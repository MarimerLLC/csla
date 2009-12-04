<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
    Me.components = New System.ComponentModel.Container
    Me.ProjectDataBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ProjectDataDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn
    CType(Me.ProjectDataBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ProjectDataDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'ProjectDataBindingSource
    '
    Me.ProjectDataBindingSource.DataSource = GetType(PTWcfClient.PTWcfService.ProjectData)
    '
    'ProjectDataDataGridView
    '
    Me.ProjectDataDataGridView.AllowUserToAddRows = False
    Me.ProjectDataDataGridView.AllowUserToDeleteRows = False
    Me.ProjectDataDataGridView.AutoGenerateColumns = False
    Me.ProjectDataDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
    Me.ProjectDataDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
    Me.ProjectDataDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn4})
    Me.ProjectDataDataGridView.DataSource = Me.ProjectDataBindingSource
    Me.ProjectDataDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
    Me.ProjectDataDataGridView.Location = New System.Drawing.Point(0, 0)
    Me.ProjectDataDataGridView.Name = "ProjectDataDataGridView"
    Me.ProjectDataDataGridView.ReadOnly = True
    Me.ProjectDataDataGridView.Size = New System.Drawing.Size(836, 477)
    Me.ProjectDataDataGridView.TabIndex = 1
    '
    'DataGridViewTextBoxColumn3
    '
    Me.DataGridViewTextBoxColumn3.DataPropertyName = "Id"
    Me.DataGridViewTextBoxColumn3.HeaderText = "Id"
    Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
    Me.DataGridViewTextBoxColumn3.ReadOnly = True
    Me.DataGridViewTextBoxColumn3.Width = 54
    '
    'DataGridViewTextBoxColumn4
    '
    Me.DataGridViewTextBoxColumn4.DataPropertyName = "Name"
    Me.DataGridViewTextBoxColumn4.HeaderText = "Name"
    Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
    Me.DataGridViewTextBoxColumn4.ReadOnly = True
    Me.DataGridViewTextBoxColumn4.Width = 93
    '
    'Form1
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(836, 477)
    Me.Controls.Add(Me.ProjectDataDataGridView)
    Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.Margin = New System.Windows.Forms.Padding(6)
    Me.Name = "Form1"
    Me.Text = "Form1"
    CType(Me.ProjectDataBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ProjectDataDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents ProjectDataBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ProjectDataDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
