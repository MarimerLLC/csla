namespace WindowsUI
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.Label customerNameLabel;
      System.Windows.Forms.Label idLabel;
      this.orderBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.customerNameTextBox = new System.Windows.Forms.TextBox();
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.lineItemsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.lineItemsDataGridView = new System.Windows.Forms.DataGridView();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cslaActionExtender1 = new Csla.Windows.CslaActionExtender(this.components);
      this.SaveButton = new System.Windows.Forms.Button();
      this.CancelButton1 = new System.Windows.Forms.Button();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.bindingSourceRefresh1 = new Csla.Windows.BindingSourceRefresh(this.components);
      customerNameLabel = new System.Windows.Forms.Label();
      idLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.lineItemsBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.lineItemsDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).BeginInit();
      this.SuspendLayout();
      // 
      // customerNameLabel
      // 
      customerNameLabel.AutoSize = true;
      customerNameLabel.Location = new System.Drawing.Point(26, 88);
      customerNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      customerNameLabel.Name = "customerNameLabel";
      customerNameLabel.Size = new System.Drawing.Size(128, 20);
      customerNameLabel.TabIndex = 1;
      customerNameLabel.Text = "Customer Name:";
      // 
      // idLabel
      // 
      idLabel.AutoSize = true;
      idLabel.Location = new System.Drawing.Point(26, 48);
      idLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      idLabel.Name = "idLabel";
      idLabel.Size = new System.Drawing.Size(27, 20);
      idLabel.TabIndex = 3;
      idLabel.Text = "Id:";
      // 
      // orderBindingSource
      // 
      this.orderBindingSource.DataSource = typeof(BusinessLibrary.Order);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.orderBindingSource, true);
      // 
      // customerNameTextBox
      // 
      this.customerNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "CustomerName", true));
      this.customerNameTextBox.Location = new System.Drawing.Point(162, 83);
      this.customerNameTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.customerNameTextBox.Name = "customerNameTextBox";
      this.customerNameTextBox.Size = new System.Drawing.Size(148, 26);
      this.customerNameTextBox.TabIndex = 2;
      // 
      // idTextBox
      // 
      this.idTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "Id", true));
      this.idTextBox.Location = new System.Drawing.Point(162, 43);
      this.idTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.ReadOnly = true;
      this.idTextBox.Size = new System.Drawing.Size(148, 26);
      this.idTextBox.TabIndex = 4;
      // 
      // lineItemsBindingSource
      // 
      this.lineItemsBindingSource.DataMember = "LineItems";
      this.lineItemsBindingSource.DataSource = this.orderBindingSource;
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.lineItemsBindingSource, false);
      // 
      // lineItemsDataGridView
      // 
      this.lineItemsDataGridView.AutoGenerateColumns = false;
      this.lineItemsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.lineItemsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
      this.lineItemsDataGridView.DataSource = this.lineItemsBindingSource;
      this.lineItemsDataGridView.Location = new System.Drawing.Point(30, 137);
      this.lineItemsDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.lineItemsDataGridView.Name = "lineItemsDataGridView";
      this.lineItemsDataGridView.RowHeadersWidth = 62;
      this.lineItemsDataGridView.Size = new System.Drawing.Size(450, 338);
      this.lineItemsDataGridView.TabIndex = 5;
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Id";
      this.dataGridViewTextBoxColumn1.HeaderText = "Id";
      this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.Width = 150;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.DataPropertyName = "Name";
      this.dataGridViewTextBoxColumn2.HeaderText = "Name";
      this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.Width = 150;
      // 
      // cslaActionExtender1
      // 
      this.cslaActionExtender1.DataSource = this.orderBindingSource;
      this.cslaActionExtender1.ObjectIsValidMessage = "Object is valid";
      // 
      // SaveButton
      // 
      this.cslaActionExtender1.SetActionType(this.SaveButton, Csla.Windows.CslaFormAction.Save);
      this.SaveButton.Location = new System.Drawing.Point(540, 43);
      this.SaveButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.SaveButton.Name = "SaveButton";
      this.SaveButton.Size = new System.Drawing.Size(112, 35);
      this.SaveButton.TabIndex = 6;
      this.SaveButton.Text = "Save";
      this.SaveButton.UseVisualStyleBackColor = true;
      // 
      // CancelButton1
      // 
      this.cslaActionExtender1.SetActionType(this.CancelButton1, Csla.Windows.CslaFormAction.Cancel);
      this.CancelButton1.Location = new System.Drawing.Point(540, 88);
      this.CancelButton1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.CancelButton1.Name = "CancelButton1";
      this.CancelButton1.Size = new System.Drawing.Size(112, 35);
      this.CancelButton1.TabIndex = 7;
      this.CancelButton1.Text = "Cancel";
      this.CancelButton1.UseVisualStyleBackColor = true;
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      this.errorProvider1.DataSource = this.orderBindingSource;
      // 
      // bindingSourceRefresh1
      // 
      this.bindingSourceRefresh1.Host = this;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1046, 714);
      this.Controls.Add(this.CancelButton1);
      this.Controls.Add(this.SaveButton);
      this.Controls.Add(this.lineItemsDataGridView);
      this.Controls.Add(customerNameLabel);
      this.Controls.Add(this.customerNameTextBox);
      this.Controls.Add(idLabel);
      this.Controls.Add(this.idTextBox);
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "Form1";
      this.Text = "Order entry";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.lineItemsBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.lineItemsDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource orderBindingSource;
    private System.Windows.Forms.TextBox customerNameTextBox;
    private System.Windows.Forms.TextBox idTextBox;
    private System.Windows.Forms.BindingSource lineItemsBindingSource;
    private System.Windows.Forms.DataGridView lineItemsDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private Csla.Windows.CslaActionExtender cslaActionExtender1;
    private System.Windows.Forms.Button SaveButton;
    private System.Windows.Forms.Button CancelButton1;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private Csla.Windows.BindingSourceRefresh bindingSourceRefresh1;
  }
}

