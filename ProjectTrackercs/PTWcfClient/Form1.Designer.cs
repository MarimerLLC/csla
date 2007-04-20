namespace PTWcfClient
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
      this.projectDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.projectDataDataGridView = new System.Windows.Forms.DataGridView();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.projectDataBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.projectDataDataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // projectDataBindingSource
      // 
      this.projectDataBindingSource.DataSource = typeof(PTWcfClient.PTWcfService.ProjectData);
      // 
      // projectDataDataGridView
      // 
      this.projectDataDataGridView.AllowUserToAddRows = false;
      this.projectDataDataGridView.AllowUserToDeleteRows = false;
      this.projectDataDataGridView.AutoGenerateColumns = false;
      this.projectDataDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.projectDataDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.projectDataDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
      this.projectDataDataGridView.DataSource = this.projectDataBindingSource;
      this.projectDataDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.projectDataDataGridView.Location = new System.Drawing.Point(0, 0);
      this.projectDataDataGridView.Margin = new System.Windows.Forms.Padding(6);
      this.projectDataDataGridView.MultiSelect = false;
      this.projectDataDataGridView.Name = "projectDataDataGridView";
      this.projectDataDataGridView.ReadOnly = true;
      this.projectDataDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.projectDataDataGridView.Size = new System.Drawing.Size(773, 435);
      this.projectDataDataGridView.TabIndex = 1;
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Id";
      this.dataGridViewTextBoxColumn1.HeaderText = "Id";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Width = 54;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.DataPropertyName = "Name";
      this.dataGridViewTextBoxColumn2.HeaderText = "Name";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 93;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(773, 435);
      this.Controls.Add(this.projectDataDataGridView);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(6);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.projectDataBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.projectDataDataGridView)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.BindingSource projectDataBindingSource;
    private System.Windows.Forms.DataGridView projectDataDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
  }
}

