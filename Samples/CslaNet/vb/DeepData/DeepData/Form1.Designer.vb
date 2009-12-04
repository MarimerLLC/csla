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
    Me.OrderListDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.OrderListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.LineItemsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.LineItemsDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DetailsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
    Me.DetailsDataGridView = New System.Windows.Forms.DataGridView
    Me.DataGridViewTextBoxColumn6 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn7 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn8 = New System.Windows.Forms.DataGridViewTextBoxColumn
    Me.DataGridViewTextBoxColumn9 = New System.Windows.Forms.DataGridViewTextBoxColumn
    CType(Me.OrderListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.OrderListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.LineItemsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.LineItemsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.DetailsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.DetailsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'OrderListDataGridView
    '
    Me.OrderListDataGridView.AutoGenerateColumns = False
    Me.OrderListDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2})
    Me.OrderListDataGridView.DataSource = Me.OrderListBindingSource
    Me.OrderListDataGridView.Location = New System.Drawing.Point(12, 12)
    Me.OrderListDataGridView.Name = "OrderListDataGridView"
    Me.OrderListDataGridView.Size = New System.Drawing.Size(245, 220)
    Me.OrderListDataGridView.TabIndex = 1
    '
    'DataGridViewTextBoxColumn1
    '
    Me.DataGridViewTextBoxColumn1.DataPropertyName = "Customer"
    Me.DataGridViewTextBoxColumn1.HeaderText = "Customer"
    Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
    Me.DataGridViewTextBoxColumn1.ReadOnly = True
    '
    'DataGridViewTextBoxColumn2
    '
    Me.DataGridViewTextBoxColumn2.DataPropertyName = "Id"
    Me.DataGridViewTextBoxColumn2.HeaderText = "Id"
    Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
    Me.DataGridViewTextBoxColumn2.ReadOnly = True
    '
    'OrderListBindingSource
    '
    Me.OrderListBindingSource.DataSource = GetType(DeepData.Library.OrderInfo)
    '
    'LineItemsBindingSource
    '
    Me.LineItemsBindingSource.DataMember = "LineItems"
    Me.LineItemsBindingSource.DataSource = Me.OrderListBindingSource
    '
    'LineItemsDataGridView
    '
    Me.LineItemsDataGridView.AutoGenerateColumns = False
    Me.LineItemsDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn4, Me.DataGridViewTextBoxColumn5})
    Me.LineItemsDataGridView.DataSource = Me.LineItemsBindingSource
    Me.LineItemsDataGridView.Location = New System.Drawing.Point(276, 12)
    Me.LineItemsDataGridView.Name = "LineItemsDataGridView"
    Me.LineItemsDataGridView.Size = New System.Drawing.Size(343, 220)
    Me.LineItemsDataGridView.TabIndex = 1
    '
    'DataGridViewTextBoxColumn3
    '
    Me.DataGridViewTextBoxColumn3.DataPropertyName = "Product"
    Me.DataGridViewTextBoxColumn3.HeaderText = "Product"
    Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
    Me.DataGridViewTextBoxColumn3.ReadOnly = True
    '
    'DataGridViewTextBoxColumn4
    '
    Me.DataGridViewTextBoxColumn4.DataPropertyName = "OrderId"
    Me.DataGridViewTextBoxColumn4.HeaderText = "OrderId"
    Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
    Me.DataGridViewTextBoxColumn4.ReadOnly = True
    '
    'DataGridViewTextBoxColumn5
    '
    Me.DataGridViewTextBoxColumn5.DataPropertyName = "Id"
    Me.DataGridViewTextBoxColumn5.HeaderText = "Id"
    Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
    Me.DataGridViewTextBoxColumn5.ReadOnly = True
    '
    'DetailsBindingSource
    '
    Me.DetailsBindingSource.DataMember = "Details"
    Me.DetailsBindingSource.DataSource = Me.LineItemsBindingSource
    '
    'DetailsDataGridView
    '
    Me.DetailsDataGridView.AutoGenerateColumns = False
    Me.DetailsDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn6, Me.DataGridViewTextBoxColumn7, Me.DataGridViewTextBoxColumn8, Me.DataGridViewTextBoxColumn9})
    Me.DetailsDataGridView.DataSource = Me.DetailsBindingSource
    Me.DetailsDataGridView.Location = New System.Drawing.Point(175, 249)
    Me.DetailsDataGridView.Name = "DetailsDataGridView"
    Me.DetailsDataGridView.Size = New System.Drawing.Size(444, 237)
    Me.DetailsDataGridView.TabIndex = 2
    '
    'DataGridViewTextBoxColumn6
    '
    Me.DataGridViewTextBoxColumn6.DataPropertyName = "Detail"
    Me.DataGridViewTextBoxColumn6.HeaderText = "Detail"
    Me.DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6"
    Me.DataGridViewTextBoxColumn6.ReadOnly = True
    '
    'DataGridViewTextBoxColumn7
    '
    Me.DataGridViewTextBoxColumn7.DataPropertyName = "OrderId"
    Me.DataGridViewTextBoxColumn7.HeaderText = "OrderId"
    Me.DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7"
    Me.DataGridViewTextBoxColumn7.ReadOnly = True
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
    Me.DataGridViewTextBoxColumn9.DataPropertyName = "LineId"
    Me.DataGridViewTextBoxColumn9.HeaderText = "LineId"
    Me.DataGridViewTextBoxColumn9.Name = "DataGridViewTextBoxColumn9"
    Me.DataGridViewTextBoxColumn9.ReadOnly = True
    '
    'Form1
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(639, 626)
    Me.Controls.Add(Me.DetailsDataGridView)
    Me.Controls.Add(Me.LineItemsDataGridView)
    Me.Controls.Add(Me.OrderListDataGridView)
    Me.Name = "Form1"
    Me.Text = "Form1"
    CType(Me.OrderListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.OrderListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.LineItemsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.LineItemsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.DetailsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.DetailsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents OrderListBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents OrderListDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents LineItemsBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents LineItemsDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DetailsBindingSource As System.Windows.Forms.BindingSource
  Friend WithEvents DetailsDataGridView As System.Windows.Forms.DataGridView
  Friend WithEvents DataGridViewTextBoxColumn6 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn7 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn8 As System.Windows.Forms.DataGridViewTextBoxColumn
  Friend WithEvents DataGridViewTextBoxColumn9 As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
