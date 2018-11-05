namespace PTWin
{
  partial class ResourceEdit
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
      System.Windows.Forms.Label FirstNameLabel;
      System.Windows.Forms.Label IdLabel;
      System.Windows.Forms.Label LastNameLabel;
      this.CloseButton = new System.Windows.Forms.Button();
      this.ApplyButton = new System.Windows.Forms.Button();
      this.Cancel_Button = new System.Windows.Forms.Button();
      this.OKButton = new System.Windows.Forms.Button();
      this.GroupBox1 = new System.Windows.Forms.GroupBox();
      this.AssignmentsDataGridView = new System.Windows.Forms.DataGridView();
      this.RoleListBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.AssignmentsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.ResourceBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.UnassignButton = new System.Windows.Forms.Button();
      this.AssignButton = new System.Windows.Forms.Button();
      this.FirstNameTextBox = new System.Windows.Forms.TextBox();
      this.IdLabel1 = new System.Windows.Forms.Label();
      this.LastNameTextBox = new System.Windows.Forms.TextBox();
      this.ErrorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.ReadWriteAuthorization1 = new Csla.Windows.ReadWriteAuthorization(this.components);
      this.RefreshButton = new System.Windows.Forms.Button();
      this.bindingSourceRefresh1 = new Csla.Windows.BindingSourceRefresh(this.components);
      this.projectIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.projectNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewLinkColumn();
      this.assignedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Role = new System.Windows.Forms.DataGridViewComboBoxColumn();
      FirstNameLabel = new System.Windows.Forms.Label();
      IdLabel = new System.Windows.Forms.Label();
      LastNameLabel = new System.Windows.Forms.Label();
      this.GroupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.AssignmentsDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.RoleListBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.AssignmentsBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ResourceBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).BeginInit();
      this.SuspendLayout();
      // 
      // FirstNameLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(FirstNameLabel, false);
      FirstNameLabel.AutoSize = true;
      FirstNameLabel.Location = new System.Drawing.Point(13, 42);
      FirstNameLabel.Name = "FirstNameLabel";
      FirstNameLabel.Size = new System.Drawing.Size(60, 13);
      FirstNameLabel.TabIndex = 2;
      FirstNameLabel.Text = "First Name:";
      // 
      // IdLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(IdLabel, false);
      IdLabel.AutoSize = true;
      IdLabel.Location = new System.Drawing.Point(13, 13);
      IdLabel.Name = "IdLabel";
      IdLabel.Size = new System.Drawing.Size(19, 13);
      IdLabel.TabIndex = 0;
      IdLabel.Text = "Id:";
      // 
      // LastNameLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(LastNameLabel, false);
      LastNameLabel.AutoSize = true;
      LastNameLabel.Location = new System.Drawing.Point(13, 68);
      LastNameLabel.Name = "LastNameLabel";
      LastNameLabel.Size = new System.Drawing.Size(61, 13);
      LastNameLabel.TabIndex = 4;
      LastNameLabel.Text = "Last Name:";
      // 
      // CloseButton
      // 
      this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.CloseButton, false);
      this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CloseButton.Location = new System.Drawing.Point(501, 100);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new System.Drawing.Size(75, 23);
      this.CloseButton.TabIndex = 10;
      this.CloseButton.Text = "Close";
      this.CloseButton.UseVisualStyleBackColor = true;
      this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
      // 
      // ApplyButton
      // 
      this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.ApplyButton, false);
      this.ApplyButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.ApplyButton.Location = new System.Drawing.Point(501, 42);
      this.ApplyButton.Name = "ApplyButton";
      this.ApplyButton.Size = new System.Drawing.Size(75, 23);
      this.ApplyButton.TabIndex = 8;
      this.ApplyButton.Text = "Apply";
      this.ApplyButton.UseVisualStyleBackColor = true;
      this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
      // 
      // Cancel_Button
      // 
      this.Cancel_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.Cancel_Button, false);
      this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel_Button.Location = new System.Drawing.Point(501, 71);
      this.Cancel_Button.Name = "Cancel_Button";
      this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
      this.Cancel_Button.TabIndex = 9;
      this.Cancel_Button.Text = "Cancel";
      this.Cancel_Button.UseVisualStyleBackColor = true;
      this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
      // 
      // OKButton
      // 
      this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.OKButton, false);
      this.OKButton.Location = new System.Drawing.Point(501, 13);
      this.OKButton.Name = "OKButton";
      this.OKButton.Size = new System.Drawing.Size(75, 23);
      this.OKButton.TabIndex = 7;
      this.OKButton.Text = "OK";
      this.OKButton.UseVisualStyleBackColor = true;
      this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
      // 
      // GroupBox1
      // 
      this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.GroupBox1, false);
      this.GroupBox1.Controls.Add(this.AssignmentsDataGridView);
      this.GroupBox1.Controls.Add(this.UnassignButton);
      this.GroupBox1.Controls.Add(this.AssignButton);
      this.GroupBox1.Location = new System.Drawing.Point(16, 91);
      this.GroupBox1.Name = "GroupBox1";
      this.GroupBox1.Size = new System.Drawing.Size(449, 310);
      this.GroupBox1.TabIndex = 6;
      this.GroupBox1.TabStop = false;
      this.GroupBox1.Text = "Assigned projects";
      // 
      // AssignmentsDataGridView
      // 
      this.AssignmentsDataGridView.AllowUserToAddRows = false;
      this.AssignmentsDataGridView.AllowUserToDeleteRows = false;
      this.AssignmentsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.AssignmentsDataGridView, false);
      this.AssignmentsDataGridView.AutoGenerateColumns = false;
      this.AssignmentsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.AssignmentsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.projectIdDataGridViewTextBoxColumn,
            this.projectNameDataGridViewTextBoxColumn,
            this.assignedDataGridViewTextBoxColumn,
            this.Role});
      this.AssignmentsDataGridView.DataSource = this.AssignmentsBindingSource;
      this.AssignmentsDataGridView.Location = new System.Drawing.Point(6, 19);
      this.AssignmentsDataGridView.MultiSelect = false;
      this.AssignmentsDataGridView.Name = "AssignmentsDataGridView";
      this.AssignmentsDataGridView.RowHeadersVisible = false;
      this.AssignmentsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.AssignmentsDataGridView.Size = new System.Drawing.Size(356, 285);
      this.AssignmentsDataGridView.TabIndex = 0;
      this.AssignmentsDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AssignmentsDataGridView_CellContentClick);
      // 
      // RoleListBindingSource
      // 
      this.RoleListBindingSource.DataSource = typeof(ProjectTracker.Library.RoleList);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.RoleListBindingSource, false);
      // 
      // AssignmentsBindingSource
      // 
      this.AssignmentsBindingSource.DataMember = "Assignments";
      this.AssignmentsBindingSource.DataSource = this.ResourceBindingSource;
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.AssignmentsBindingSource, false);
      // 
      // ResourceBindingSource
      // 
      this.ResourceBindingSource.DataSource = typeof(ProjectTracker.Library.ResourceEdit);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.ResourceBindingSource, true);
      // 
      // UnassignButton
      // 
      this.UnassignButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.UnassignButton, false);
      this.UnassignButton.Location = new System.Drawing.Point(368, 48);
      this.UnassignButton.Name = "UnassignButton";
      this.UnassignButton.Size = new System.Drawing.Size(75, 23);
      this.UnassignButton.TabIndex = 2;
      this.UnassignButton.Text = "Unassign";
      this.UnassignButton.UseVisualStyleBackColor = true;
      this.UnassignButton.Click += new System.EventHandler(this.UnassignButton_Click);
      // 
      // AssignButton
      // 
      this.AssignButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.AssignButton, false);
      this.AssignButton.Location = new System.Drawing.Point(368, 19);
      this.AssignButton.Name = "AssignButton";
      this.AssignButton.Size = new System.Drawing.Size(75, 23);
      this.AssignButton.TabIndex = 1;
      this.AssignButton.Text = "Assign";
      this.AssignButton.UseVisualStyleBackColor = true;
      this.AssignButton.Click += new System.EventHandler(this.AssignButton_Click);
      // 
      // FirstNameTextBox
      // 
      this.FirstNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.FirstNameTextBox, true);
      this.FirstNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ResourceBindingSource, "FirstName", true));
      this.FirstNameTextBox.Location = new System.Drawing.Point(80, 39);
      this.FirstNameTextBox.Name = "FirstNameTextBox";
      this.FirstNameTextBox.Size = new System.Drawing.Size(385, 20);
      this.FirstNameTextBox.TabIndex = 3;
      // 
      // IdLabel1
      // 
      this.IdLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.IdLabel1, true);
      this.IdLabel1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ResourceBindingSource, "Id", true));
      this.IdLabel1.Location = new System.Drawing.Point(80, 13);
      this.IdLabel1.Name = "IdLabel1";
      this.IdLabel1.Size = new System.Drawing.Size(385, 23);
      this.IdLabel1.TabIndex = 1;
      // 
      // LastNameTextBox
      // 
      this.LastNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.LastNameTextBox, true);
      this.LastNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ResourceBindingSource, "LastName", true));
      this.LastNameTextBox.Location = new System.Drawing.Point(80, 65);
      this.LastNameTextBox.Name = "LastNameTextBox";
      this.LastNameTextBox.Size = new System.Drawing.Size(385, 20);
      this.LastNameTextBox.TabIndex = 5;
      // 
      // ErrorProvider1
      // 
      this.ErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.ErrorProvider1.ContainerControl = this;
      this.ErrorProvider1.DataSource = this.ResourceBindingSource;
      // 
      // RefreshButton
      // 
      this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.RefreshButton, false);
      this.RefreshButton.Location = new System.Drawing.Point(501, 129);
      this.RefreshButton.Name = "RefreshButton";
      this.RefreshButton.Size = new System.Drawing.Size(75, 23);
      this.RefreshButton.TabIndex = 11;
      this.RefreshButton.Text = "Refresh";
      this.RefreshButton.UseVisualStyleBackColor = true;
      this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
      // 
      // bindingSourceRefresh1
      // 
      this.bindingSourceRefresh1.Host = this;
      // 
      // projectIdDataGridViewTextBoxColumn
      // 
      this.projectIdDataGridViewTextBoxColumn.DataPropertyName = "ProjectId";
      this.projectIdDataGridViewTextBoxColumn.HeaderText = "ProjectId";
      this.projectIdDataGridViewTextBoxColumn.Name = "projectIdDataGridViewTextBoxColumn";
      this.projectIdDataGridViewTextBoxColumn.ReadOnly = true;
      this.projectIdDataGridViewTextBoxColumn.Visible = false;
      this.projectIdDataGridViewTextBoxColumn.Width = 74;
      // 
      // projectNameDataGridViewTextBoxColumn
      // 
      this.projectNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.projectNameDataGridViewTextBoxColumn.DataPropertyName = "ProjectName";
      this.projectNameDataGridViewTextBoxColumn.FillWeight = 200F;
      this.projectNameDataGridViewTextBoxColumn.HeaderText = "Project Name";
      this.projectNameDataGridViewTextBoxColumn.Name = "projectNameDataGridViewTextBoxColumn";
      this.projectNameDataGridViewTextBoxColumn.ReadOnly = true;
      this.projectNameDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.projectNameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
      // 
      // assignedDataGridViewTextBoxColumn
      // 
      this.assignedDataGridViewTextBoxColumn.DataPropertyName = "Assigned";
      this.assignedDataGridViewTextBoxColumn.HeaderText = "Assigned";
      this.assignedDataGridViewTextBoxColumn.Name = "assignedDataGridViewTextBoxColumn";
      this.assignedDataGridViewTextBoxColumn.ReadOnly = true;
      this.assignedDataGridViewTextBoxColumn.Width = 75;
      // 
      // Role
      // 
      this.Role.DataPropertyName = "Role";
      this.Role.DataSource = this.RoleListBindingSource;
      this.Role.DisplayMember = "Value";
      this.Role.FillWeight = 200F;
      this.Role.HeaderText = "Role";
      this.Role.Name = "Role";
      this.Role.ValueMember = "Key";
      this.Role.Width = 35;
      // 
      // ResourceEdit
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(this, false);
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.RefreshButton);
      this.Controls.Add(this.CloseButton);
      this.Controls.Add(this.ApplyButton);
      this.Controls.Add(this.Cancel_Button);
      this.Controls.Add(this.OKButton);
      this.Controls.Add(this.GroupBox1);
      this.Controls.Add(FirstNameLabel);
      this.Controls.Add(this.FirstNameTextBox);
      this.Controls.Add(IdLabel);
      this.Controls.Add(this.IdLabel1);
      this.Controls.Add(LastNameLabel);
      this.Controls.Add(this.LastNameTextBox);
      this.Name = "ResourceEdit";
      this.Size = new System.Drawing.Size(588, 415);
      this.Load += new System.EventHandler(this.ResourceEdit_Load);
      this.GroupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.AssignmentsDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.RoleListBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.AssignmentsBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ResourceBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Button CloseButton;
    internal System.Windows.Forms.Button ApplyButton;
    internal System.Windows.Forms.Button Cancel_Button;
    internal System.Windows.Forms.Button OKButton;
    internal System.Windows.Forms.GroupBox GroupBox1;
    internal System.Windows.Forms.Button UnassignButton;
    internal System.Windows.Forms.Button AssignButton;
    internal System.Windows.Forms.TextBox FirstNameTextBox;
    internal System.Windows.Forms.Label IdLabel1;
    internal System.Windows.Forms.TextBox LastNameTextBox;
    internal Csla.Windows.ReadWriteAuthorization ReadWriteAuthorization1;
    internal System.Windows.Forms.DataGridView AssignmentsDataGridView;
    internal System.Windows.Forms.BindingSource RoleListBindingSource;
    internal System.Windows.Forms.BindingSource AssignmentsBindingSource;
    internal System.Windows.Forms.BindingSource ResourceBindingSource;
    internal System.Windows.Forms.ErrorProvider ErrorProvider1;
    private Csla.Windows.BindingSourceRefresh bindingSourceRefresh1;
    internal System.Windows.Forms.Button RefreshButton;
    private System.Windows.Forms.DataGridViewTextBoxColumn projectIdDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewLinkColumn projectNameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn assignedDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewComboBoxColumn Role;
  }
}
