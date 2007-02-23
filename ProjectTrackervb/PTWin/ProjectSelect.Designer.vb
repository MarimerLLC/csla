<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProjectSelect
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
    Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
    Me.OK_Button = New System.Windows.Forms.Button
    Me.Cancel_Button = New System.Windows.Forms.Button
    Me.ProjectListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.ProjectListListBox = New System.Windows.Forms.ListBox
    Me.NameLabel = New System.Windows.Forms.Label
    Me.NameTextBox = New System.Windows.Forms.TextBox
    Me.GetListButton = New System.Windows.Forms.Button
    Me.TableLayoutPanel1.SuspendLayout()
    CType(Me.ProjectListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'TableLayoutPanel1
    '
    Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.TableLayoutPanel1.ColumnCount = 2
    Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
    Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
    Me.TableLayoutPanel1.Location = New System.Drawing.Point(285, 282)
    Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
    Me.TableLayoutPanel1.RowCount = 1
    Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
    Me.TableLayoutPanel1.TabIndex = 0
    '
    'OK_Button
    '
    Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
    Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.OK_Button.Location = New System.Drawing.Point(3, 3)
    Me.OK_Button.Name = "OK_Button"
    Me.OK_Button.Size = New System.Drawing.Size(67, 23)
    Me.OK_Button.TabIndex = 0
    Me.OK_Button.Text = "OK"
    '
    'Cancel_Button
    '
    Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
    Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
    Me.Cancel_Button.Name = "Cancel_Button"
    Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
    Me.Cancel_Button.TabIndex = 1
    Me.Cancel_Button.Text = "Cancel"
    '
    'ProjectListBindingSource
    '
    Me.ProjectListBindingSource.DataSource = GetType(ProjectTracker.Library.ProjectList)
    '
    'ProjectListListBox
    '
    Me.ProjectListListBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ProjectListListBox.DataSource = Me.ProjectListBindingSource
    Me.ProjectListListBox.DisplayMember = "Name"
    Me.ProjectListListBox.Location = New System.Drawing.Point(12, 38)
    Me.ProjectListListBox.Name = "ProjectListListBox"
    Me.ProjectListListBox.Size = New System.Drawing.Size(419, 238)
    Me.ProjectListListBox.TabIndex = 2
    Me.ProjectListListBox.ValueMember = "ID"
    '
    'NameLabel
    '
    Me.NameLabel.AutoSize = True
    Me.NameLabel.Location = New System.Drawing.Point(12, 9)
    Me.NameLabel.Name = "NameLabel"
    Me.NameLabel.Size = New System.Drawing.Size(38, 13)
    Me.NameLabel.TabIndex = 3
    Me.NameLabel.Text = "Name:"
    '
    'NameTextBox
    '
    Me.NameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.NameTextBox.Location = New System.Drawing.Point(83, 6)
    Me.NameTextBox.Name = "NameTextBox"
    Me.NameTextBox.Size = New System.Drawing.Size(267, 20)
    Me.NameTextBox.TabIndex = 4
    '
    'GetListButton
    '
    Me.GetListButton.Location = New System.Drawing.Point(356, 6)
    Me.GetListButton.Name = "GetListButton"
    Me.GetListButton.Size = New System.Drawing.Size(75, 23)
    Me.GetListButton.TabIndex = 5
    Me.GetListButton.Text = "Get list"
    Me.GetListButton.UseVisualStyleBackColor = True
    '
    'ProjectSelect
    '
    Me.AcceptButton = Me.OK_Button
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.CancelButton = Me.Cancel_Button
    Me.ClientSize = New System.Drawing.Size(443, 323)
    Me.Controls.Add(Me.GetListButton)
    Me.Controls.Add(Me.NameTextBox)
    Me.Controls.Add(Me.NameLabel)
    Me.Controls.Add(Me.ProjectListListBox)
    Me.Controls.Add(Me.TableLayoutPanel1)
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
    Me.MaximizeBox = False
    Me.MinimizeBox = False
    Me.Name = "ProjectSelect"
    Me.ShowInTaskbar = False
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "ProjectSelect"
    Me.TableLayoutPanel1.ResumeLayout(False)
    CType(Me.ProjectListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
  Friend WithEvents Cancel_Button As System.Windows.Forms.Button
  Friend WithEvents ProjectListBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ProjectListListBox As System.Windows.Forms.ListBox
  Friend WithEvents NameLabel As System.Windows.Forms.Label
  Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
  Friend WithEvents GetListButton As System.Windows.Forms.Button

End Class
