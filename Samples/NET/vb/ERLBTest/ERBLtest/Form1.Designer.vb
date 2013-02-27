<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
    Me.ChildListBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
    Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton
    Me.ChildListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel
    Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton
    Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton
    Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton
    Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator
    Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox
    Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator
    Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton
    Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton
    Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator
    Me.ChildListBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton
    Me.ChildListDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridView1 = New System.Windows.Forms.DataGridView
    Me.Button1 = New System.Windows.Forms.Button
    Me.Button2 = New System.Windows.Forms.Button
    Me.Val1 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.Val2 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
    CType(Me.ChildListBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.ChildListBindingNavigator.SuspendLayout()
    CType(Me.ChildListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.ChildListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'ChildListBindingNavigator
    '
    Me.ChildListBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItem
    Me.ChildListBindingNavigator.BindingSource = Me.ChildListBindingSource
    Me.ChildListBindingNavigator.CountItem = Me.BindingNavigatorCountItem
    Me.ChildListBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
    Me.ChildListBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.ChildListBindingNavigatorSaveItem})
    Me.ChildListBindingNavigator.Location = New System.Drawing.Point(0, 0)
    Me.ChildListBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
    Me.ChildListBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
    Me.ChildListBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
    Me.ChildListBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
    Me.ChildListBindingNavigator.Name = "ChildListBindingNavigator"
    Me.ChildListBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
    Me.ChildListBindingNavigator.Size = New System.Drawing.Size(731, 25)
    Me.ChildListBindingNavigator.TabIndex = 0
    Me.ChildListBindingNavigator.Text = "BindingNavigator1"
    '
    'BindingNavigatorAddNewItem
    '
    Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.BindingNavigatorAddNewItem.Image = CType(resources.GetObject("BindingNavigatorAddNewItem.Image"), System.Drawing.Image)
    Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
    Me.BindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = True
    Me.BindingNavigatorAddNewItem.Size = New System.Drawing.Size(23, 22)
    Me.BindingNavigatorAddNewItem.Text = "Add new"
    '
    'ChildListBindingSource
    '
    Me.ChildListBindingSource.DataSource = GetType(ERBLtest.Child)
    '
    'BindingNavigatorCountItem
    '
    Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
    Me.BindingNavigatorCountItem.Size = New System.Drawing.Size(36, 22)
    Me.BindingNavigatorCountItem.Text = "of {0}"
    Me.BindingNavigatorCountItem.ToolTipText = "Total number of items"
    '
    'BindingNavigatorDeleteItem
    '
    Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.BindingNavigatorDeleteItem.Image = CType(resources.GetObject("BindingNavigatorDeleteItem.Image"), System.Drawing.Image)
    Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
    Me.BindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True
    Me.BindingNavigatorDeleteItem.Size = New System.Drawing.Size(23, 22)
    Me.BindingNavigatorDeleteItem.Text = "Delete"
    '
    'BindingNavigatorMoveFirstItem
    '
    Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.BindingNavigatorMoveFirstItem.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem.Image"), System.Drawing.Image)
    Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
    Me.BindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True
    Me.BindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(23, 22)
    Me.BindingNavigatorMoveFirstItem.Text = "Move first"
    '
    'BindingNavigatorMovePreviousItem
    '
    Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.BindingNavigatorMovePreviousItem.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem.Image"), System.Drawing.Image)
    Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
    Me.BindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True
    Me.BindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(23, 22)
    Me.BindingNavigatorMovePreviousItem.Text = "Move previous"
    '
    'BindingNavigatorSeparator
    '
    Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
    Me.BindingNavigatorSeparator.Size = New System.Drawing.Size(6, 25)
    '
    'BindingNavigatorPositionItem
    '
    Me.BindingNavigatorPositionItem.AccessibleName = "Position"
    Me.BindingNavigatorPositionItem.AutoSize = False
    Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
    Me.BindingNavigatorPositionItem.Size = New System.Drawing.Size(50, 21)
    Me.BindingNavigatorPositionItem.Text = "0"
    Me.BindingNavigatorPositionItem.ToolTipText = "Current position"
    '
    'BindingNavigatorSeparator1
    '
    Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
    Me.BindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 25)
    '
    'BindingNavigatorMoveNextItem
    '
    Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.BindingNavigatorMoveNextItem.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem.Image"), System.Drawing.Image)
    Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
    Me.BindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True
    Me.BindingNavigatorMoveNextItem.Size = New System.Drawing.Size(23, 22)
    Me.BindingNavigatorMoveNextItem.Text = "Move next"
    '
    'BindingNavigatorMoveLastItem
    '
    Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.BindingNavigatorMoveLastItem.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem.Image"), System.Drawing.Image)
    Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
    Me.BindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True
    Me.BindingNavigatorMoveLastItem.Size = New System.Drawing.Size(23, 22)
    Me.BindingNavigatorMoveLastItem.Text = "Move last"
    '
    'BindingNavigatorSeparator2
    '
    Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
    Me.BindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 25)
    '
    'ChildListBindingNavigatorSaveItem
    '
    Me.ChildListBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.ChildListBindingNavigatorSaveItem.Enabled = False
    Me.ChildListBindingNavigatorSaveItem.Image = CType(resources.GetObject("ChildListBindingNavigatorSaveItem.Image"), System.Drawing.Image)
    Me.ChildListBindingNavigatorSaveItem.Name = "ChildListBindingNavigatorSaveItem"
    Me.ChildListBindingNavigatorSaveItem.Size = New System.Drawing.Size(23, 22)
    Me.ChildListBindingNavigatorSaveItem.Text = "Save Data"
    '
    'ChildListDataGridView
    '
    Me.ChildListDataGridView.AutoGenerateColumns = False
    Me.ChildListDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Val1, Me.Val2, Me.DataGridViewTextBoxColumn1})
    Me.ChildListDataGridView.DataSource = Me.ChildListBindingSource
    Me.ChildListDataGridView.Location = New System.Drawing.Point(12, 28)
    Me.ChildListDataGridView.Name = "ChildListDataGridView"
    Me.ChildListDataGridView.Size = New System.Drawing.Size(296, 408)
    Me.ChildListDataGridView.TabIndex = 1
    '
    'DataGridView1
    '
    Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
    Me.DataGridView1.Location = New System.Drawing.Point(314, 28)
    Me.DataGridView1.Name = "DataGridView1"
    Me.DataGridView1.Size = New System.Drawing.Size(301, 408)
    Me.DataGridView1.TabIndex = 2
    '
    'Button1
    '
    Me.Button1.Location = New System.Drawing.Point(621, 28)
    Me.Button1.Name = "Button1"
    Me.Button1.Size = New System.Drawing.Size(75, 23)
    Me.Button1.TabIndex = 3
    Me.Button1.Text = "Remove S"
    Me.Button1.UseVisualStyleBackColor = True
    '
    'Button2
    '
    Me.Button2.Location = New System.Drawing.Point(621, 57)
    Me.Button2.Name = "Button2"
    Me.Button2.Size = New System.Drawing.Size(75, 23)
    Me.Button2.TabIndex = 4
    Me.Button2.Text = "Add"
    Me.Button2.UseVisualStyleBackColor = True
    '
    'Val1
    '
    Me.Val1.DataPropertyName = "Val1"
    Me.Val1.HeaderText = "Val1"
    Me.Val1.Name = "Val1"
    '
    'Val2
    '
    Me.Val2.DataPropertyName = "Val2"
    Me.Val2.HeaderText = "Val2"
    Me.Val2.Name = "Val2"
    '
    'DataGridViewTextBoxColumn1
    '
    Me.DataGridViewTextBoxColumn1.DataPropertyName = "Data"
    Me.DataGridViewTextBoxColumn1.HeaderText = "Data"
    Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
    '
    'Form1
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(731, 448)
    Me.Controls.Add(Me.Button2)
    Me.Controls.Add(Me.Button1)
    Me.Controls.Add(Me.DataGridView1)
    Me.Controls.Add(Me.ChildListDataGridView)
    Me.Controls.Add(Me.ChildListBindingNavigator)
    Me.Name = "Form1"
    Me.Text = "Form1"
    CType(Me.ChildListBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ChildListBindingNavigator.ResumeLayout(False)
    Me.ChildListBindingNavigator.PerformLayout()
    CType(Me.ChildListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.ChildListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents ChildListBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents ChildListBindingNavigator As System.Windows.Forms.BindingNavigator
  Friend WithEvents BindingNavigatorAddNewItem As System.Windows.Forms.ToolStripButton
  Friend WithEvents BindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
  Friend WithEvents BindingNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
  Friend WithEvents BindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
  Friend WithEvents BindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
  Friend WithEvents BindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
  Friend WithEvents BindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
  Friend WithEvents BindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
  Friend WithEvents BindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
  Friend WithEvents BindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
  Friend WithEvents BindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
  Friend WithEvents ChildListBindingNavigatorSaveItem As System.Windows.Forms.ToolStripButton
  Friend WithEvents ChildListDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
  Friend WithEvents Button1 As System.Windows.Forms.Button
  Friend WithEvents Button2 As System.Windows.Forms.Button
  Friend WithEvents Val1 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents Val2 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
