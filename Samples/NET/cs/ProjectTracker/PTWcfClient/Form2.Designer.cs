namespace PTWcfClient
{
  partial class Form2
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
      this.roleDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.roleDataDataGridView = new System.Windows.Forms.DataGridView();
      this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.roleDataBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.roleDataDataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // roleDataBindingSource
      // 
      this.roleDataBindingSource.DataSource = typeof(PTWcfClient.PTWcfService.RoleData);
      // 
      // roleDataDataGridView
      // 
      this.roleDataDataGridView.AutoGenerateColumns = false;
      this.roleDataDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.roleDataDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
      this.roleDataDataGridView.DataSource = this.roleDataBindingSource;
      this.roleDataDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.roleDataDataGridView.Location = new System.Drawing.Point(0, 0);
      this.roleDataDataGridView.Name = "roleDataDataGridView";
      this.roleDataDataGridView.Size = new System.Drawing.Size(584, 512);
      this.roleDataDataGridView.TabIndex = 1;
      // 
      // dataGridViewTextBoxColumn3
      // 
      this.dataGridViewTextBoxColumn3.DataPropertyName = "Key";
      this.dataGridViewTextBoxColumn3.HeaderText = "Key";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      // 
      // dataGridViewTextBoxColumn4
      // 
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Value";
      this.dataGridViewTextBoxColumn4.HeaderText = "Value";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(584, 512);
      this.Controls.Add(this.roleDataDataGridView);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(6);
      this.Name = "Form2";
      this.Text = "Form2";
      this.Load += new System.EventHandler(this.Form2_Load);
      ((System.ComponentModel.ISupportInitialize)(this.roleDataBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.roleDataDataGridView)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private System.Windows.Forms.BindingSource roleDataBindingSource;
    private System.Windows.Forms.DataGridView roleDataDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
  }
}