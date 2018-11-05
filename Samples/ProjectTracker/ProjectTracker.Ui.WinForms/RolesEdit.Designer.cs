namespace PTWin
{
  partial class RolesEdit
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.CancelButton = new System.Windows.Forms.Button();
      this.SaveButton = new System.Windows.Forms.Button();
      this.rolesBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.RolesDataGridView = new System.Windows.Forms.DataGridView();
      this.DataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.DataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.rolesBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.RolesDataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // CancelButton
      // 
      this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.CancelButton.Location = new System.Drawing.Point(453, 42);
      this.CancelButton.Name = "CancelButton";
      this.CancelButton.Size = new System.Drawing.Size(75, 23);
      this.CancelButton.TabIndex = 2;
      this.CancelButton.Text = "Cancel";
      this.CancelButton.UseVisualStyleBackColor = true;
      this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
      // 
      // SaveButton
      // 
      this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.SaveButton.Location = new System.Drawing.Point(453, 13);
      this.SaveButton.Name = "SaveButton";
      this.SaveButton.Size = new System.Drawing.Size(75, 23);
      this.SaveButton.TabIndex = 1;
      this.SaveButton.Text = "Save";
      this.SaveButton.UseVisualStyleBackColor = true;
      this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
      // 
      // rolesBindingSource
      // 
      this.rolesBindingSource.DataSource = typeof(ProjectTracker.Library.Admin.RoleEditBindingList);
      // 
      // RolesDataGridView
      // 
      this.RolesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RolesDataGridView.AutoGenerateColumns = false;
      this.RolesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DataGridViewTextBoxColumn1,
            this.DataGridViewTextBoxColumn2});
      this.RolesDataGridView.DataSource = this.rolesBindingSource;
      this.RolesDataGridView.Location = new System.Drawing.Point(12, 13);
      this.RolesDataGridView.MultiSelect = false;
      this.RolesDataGridView.Name = "RolesDataGridView";
      this.RolesDataGridView.Size = new System.Drawing.Size(435, 323);
      this.RolesDataGridView.TabIndex = 0;
      // 
      // DataGridViewTextBoxColumn1
      // 
      this.DataGridViewTextBoxColumn1.DataPropertyName = "Id";
      this.DataGridViewTextBoxColumn1.HeaderText = "Id";
      this.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1";
      this.DataGridViewTextBoxColumn1.ReadOnly = true;
      // 
      // DataGridViewTextBoxColumn2
      // 
      this.DataGridViewTextBoxColumn2.DataPropertyName = "Name";
      this.DataGridViewTextBoxColumn2.FillWeight = 200F;
      this.DataGridViewTextBoxColumn2.HeaderText = "Name";
      this.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2";
      this.DataGridViewTextBoxColumn2.Width = 200;
      // 
      // RolesEdit
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.RolesDataGridView);
      this.Controls.Add(this.CancelButton);
      this.Controls.Add(this.SaveButton);
      this.Name = "RolesEdit";
      this.Size = new System.Drawing.Size(541, 348);
      this.CurrentPrincipalChanged += new System.EventHandler(this.RolesEdit_CurrentPrincipalChanged);
      this.Load += new System.EventHandler(this.RolesEdit_Load);
      ((System.ComponentModel.ISupportInitialize)(this.rolesBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.RolesDataGridView)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.Button CancelButton;
    internal System.Windows.Forms.Button SaveButton;
    internal System.Windows.Forms.BindingSource rolesBindingSource;
    internal System.Windows.Forms.DataGridView RolesDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn2;
  }
}
