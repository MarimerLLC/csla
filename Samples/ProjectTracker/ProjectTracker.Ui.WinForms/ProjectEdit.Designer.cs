namespace PTWin
{
  partial class ProjectEdit
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
      System.Windows.Forms.Label DescriptionLabel;
      System.Windows.Forms.Label EndedLabel;
      System.Windows.Forms.Label IdLabel;
      System.Windows.Forms.Label NameLabel;
      System.Windows.Forms.Label StartedLabel;
      this.CloseButton = new System.Windows.Forms.Button();
      this.ApplyButton = new System.Windows.Forms.Button();
      this.Cancel_Button = new System.Windows.Forms.Button();
      this.OKButton = new System.Windows.Forms.Button();
      this.GroupBox1 = new System.Windows.Forms.GroupBox();
      this.UnassignButton = new System.Windows.Forms.Button();
      this.AssignButton = new System.Windows.Forms.Button();
      this.ResourcesDataGridView = new System.Windows.Forms.DataGridView();
      this.ResourceId = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.FullName = new System.Windows.Forms.DataGridViewLinkColumn();
      this.Role = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.RoleListBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.assignedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ResourcesBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.ProjectBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.DescriptionTextBox = new System.Windows.Forms.TextBox();
      this.EndedTextBox = new System.Windows.Forms.TextBox();
      this.IdLabel1 = new System.Windows.Forms.Label();
      this.NameTextBox = new System.Windows.Forms.TextBox();
      this.StartedTextBox = new System.Windows.Forms.TextBox();
      this.ErrorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.ReadWriteAuthorization1 = new Csla.Windows.ReadWriteAuthorization(this.components);
      this.RefreshButton = new System.Windows.Forms.Button();
      this.bindingSourceRefresh1 = new Csla.Windows.BindingSourceRefresh(this.components);
      DescriptionLabel = new System.Windows.Forms.Label();
      EndedLabel = new System.Windows.Forms.Label();
      IdLabel = new System.Windows.Forms.Label();
      NameLabel = new System.Windows.Forms.Label();
      StartedLabel = new System.Windows.Forms.Label();
      this.GroupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ResourcesDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.RoleListBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ResourcesBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ProjectBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).BeginInit();
      this.SuspendLayout();
      // 
      // DescriptionLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(DescriptionLabel, false);
      DescriptionLabel.AutoSize = true;
      DescriptionLabel.Location = new System.Drawing.Point(12, 120);
      DescriptionLabel.Name = "DescriptionLabel";
      DescriptionLabel.Size = new System.Drawing.Size(63, 13);
      DescriptionLabel.TabIndex = 8;
      DescriptionLabel.Text = "Description:";
      // 
      // EndedLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(EndedLabel, false);
      EndedLabel.AutoSize = true;
      EndedLabel.Location = new System.Drawing.Point(12, 94);
      EndedLabel.Name = "EndedLabel";
      EndedLabel.Size = new System.Drawing.Size(41, 13);
      EndedLabel.TabIndex = 6;
      EndedLabel.Text = "Ended:";
      // 
      // IdLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(IdLabel, false);
      IdLabel.AutoSize = true;
      IdLabel.Location = new System.Drawing.Point(12, 13);
      IdLabel.Name = "IdLabel";
      IdLabel.Size = new System.Drawing.Size(19, 13);
      IdLabel.TabIndex = 0;
      IdLabel.Text = "Id:";
      // 
      // NameLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(NameLabel, false);
      NameLabel.AutoSize = true;
      NameLabel.Location = new System.Drawing.Point(12, 42);
      NameLabel.Name = "NameLabel";
      NameLabel.Size = new System.Drawing.Size(38, 13);
      NameLabel.TabIndex = 2;
      NameLabel.Text = "Name:";
      // 
      // StartedLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(StartedLabel, false);
      StartedLabel.AutoSize = true;
      StartedLabel.Location = new System.Drawing.Point(12, 68);
      StartedLabel.Name = "StartedLabel";
      StartedLabel.Size = new System.Drawing.Size(44, 13);
      StartedLabel.TabIndex = 4;
      StartedLabel.Text = "Started:";
      // 
      // CloseButton
      // 
      this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.CloseButton, false);
      this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CloseButton.Location = new System.Drawing.Point(578, 100);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new System.Drawing.Size(75, 23);
      this.CloseButton.TabIndex = 14;
      this.CloseButton.Text = "Close";
      this.CloseButton.UseVisualStyleBackColor = true;
      this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
      // 
      // ApplyButton
      // 
      this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.ApplyButton, false);
      this.ApplyButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.ApplyButton.Location = new System.Drawing.Point(578, 42);
      this.ApplyButton.Name = "ApplyButton";
      this.ApplyButton.Size = new System.Drawing.Size(75, 23);
      this.ApplyButton.TabIndex = 12;
      this.ApplyButton.Text = "Apply";
      this.ApplyButton.UseVisualStyleBackColor = true;
      this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
      // 
      // Cancel_Button
      // 
      this.Cancel_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.Cancel_Button, false);
      this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel_Button.Location = new System.Drawing.Point(578, 71);
      this.Cancel_Button.Name = "Cancel_Button";
      this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
      this.Cancel_Button.TabIndex = 13;
      this.Cancel_Button.Text = "Cancel";
      this.Cancel_Button.UseVisualStyleBackColor = true;
      this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
      // 
      // OKButton
      // 
      this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.OKButton, false);
      this.OKButton.Location = new System.Drawing.Point(578, 13);
      this.OKButton.Name = "OKButton";
      this.OKButton.Size = new System.Drawing.Size(75, 23);
      this.OKButton.TabIndex = 11;
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
      this.GroupBox1.Controls.Add(this.UnassignButton);
      this.GroupBox1.Controls.Add(this.AssignButton);
      this.GroupBox1.Controls.Add(this.ResourcesDataGridView);
      this.GroupBox1.Location = new System.Drawing.Point(81, 231);
      this.GroupBox1.Name = "GroupBox1";
      this.GroupBox1.Size = new System.Drawing.Size(460, 210);
      this.GroupBox1.TabIndex = 10;
      this.GroupBox1.TabStop = false;
      this.GroupBox1.Text = "Resources";
      // 
      // UnassignButton
      // 
      this.UnassignButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.UnassignButton, false);
      this.UnassignButton.Location = new System.Drawing.Point(379, 48);
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
      this.AssignButton.Location = new System.Drawing.Point(379, 19);
      this.AssignButton.Name = "AssignButton";
      this.AssignButton.Size = new System.Drawing.Size(75, 23);
      this.AssignButton.TabIndex = 1;
      this.AssignButton.Text = "Assign";
      this.AssignButton.UseVisualStyleBackColor = true;
      this.AssignButton.Click += new System.EventHandler(this.AssignButton_Click);
      // 
      // ResourcesDataGridView
      // 
      this.ResourcesDataGridView.AllowUserToAddRows = false;
      this.ResourcesDataGridView.AllowUserToDeleteRows = false;
      this.ResourcesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.ResourcesDataGridView, false);
      this.ResourcesDataGridView.AutoGenerateColumns = false;
      this.ResourcesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.ResourcesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ResourceId,
            this.FullName,
            this.Role,
            this.assignedDataGridViewTextBoxColumn});
      this.ResourcesDataGridView.DataSource = this.ResourcesBindingSource;
      this.ResourcesDataGridView.Location = new System.Drawing.Point(6, 19);
      this.ResourcesDataGridView.MultiSelect = false;
      this.ResourcesDataGridView.Name = "ResourcesDataGridView";
      this.ResourcesDataGridView.RowHeadersVisible = false;
      this.ResourcesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.ResourcesDataGridView.Size = new System.Drawing.Size(367, 185);
      this.ResourcesDataGridView.TabIndex = 0;
      this.ResourcesDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResourcesDataGridView_CellContentClick);
      // 
      // ResourceId
      // 
      this.ResourceId.DataPropertyName = "ResourceId";
      this.ResourceId.HeaderText = "ResourceId";
      this.ResourceId.Name = "ResourceId";
      this.ResourceId.ReadOnly = true;
      this.ResourceId.Visible = false;
      this.ResourceId.Width = 87;
      // 
      // FullName
      // 
      this.FullName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.FullName.DataPropertyName = "FullName";
      this.FullName.FillWeight = 300F;
      this.FullName.HeaderText = "Full Name";
      this.FullName.Name = "FullName";
      this.FullName.ReadOnly = true;
      this.FullName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.FullName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
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
      // RoleListBindingSource
      // 
      this.RoleListBindingSource.DataSource = typeof(ProjectTracker.Library.RoleList);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.RoleListBindingSource, false);
      // 
      // assignedDataGridViewTextBoxColumn
      // 
      this.assignedDataGridViewTextBoxColumn.DataPropertyName = "Assigned";
      this.assignedDataGridViewTextBoxColumn.HeaderText = "Assigned";
      this.assignedDataGridViewTextBoxColumn.Name = "assignedDataGridViewTextBoxColumn";
      this.assignedDataGridViewTextBoxColumn.ReadOnly = true;
      this.assignedDataGridViewTextBoxColumn.Width = 75;
      // 
      // ResourcesBindingSource
      // 
      this.ResourcesBindingSource.DataMember = "Resources";
      this.ResourcesBindingSource.DataSource = this.ProjectBindingSource;
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.ResourcesBindingSource, false);
      // 
      // ProjectBindingSource
      // 
      this.ProjectBindingSource.DataSource = typeof(ProjectTracker.Library.ProjectEdit);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.ProjectBindingSource, true);
      // 
      // DescriptionTextBox
      // 
      this.DescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.DescriptionTextBox, true);
      this.DescriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "Description", true));
      this.DescriptionTextBox.Location = new System.Drawing.Point(81, 117);
      this.DescriptionTextBox.Multiline = true;
      this.DescriptionTextBox.Name = "DescriptionTextBox";
      this.DescriptionTextBox.Size = new System.Drawing.Size(460, 108);
      this.DescriptionTextBox.TabIndex = 9;
      // 
      // EndedTextBox
      // 
      this.EndedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.EndedTextBox, true);
      this.EndedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "Ended", true));
      this.EndedTextBox.Location = new System.Drawing.Point(81, 91);
      this.EndedTextBox.Name = "EndedTextBox";
      this.EndedTextBox.Size = new System.Drawing.Size(460, 20);
      this.EndedTextBox.TabIndex = 7;
      // 
      // IdLabel1
      // 
      this.IdLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.IdLabel1, false);
      this.IdLabel1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "Id", true));
      this.IdLabel1.Location = new System.Drawing.Point(81, 13);
      this.IdLabel1.Name = "IdLabel1";
      this.IdLabel1.Size = new System.Drawing.Size(460, 23);
      this.IdLabel1.TabIndex = 1;
      // 
      // NameTextBox
      // 
      this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.NameTextBox, true);
      this.NameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "Name", true));
      this.NameTextBox.Location = new System.Drawing.Point(81, 39);
      this.NameTextBox.Name = "NameTextBox";
      this.NameTextBox.Size = new System.Drawing.Size(460, 20);
      this.NameTextBox.TabIndex = 3;
      // 
      // StartedTextBox
      // 
      this.StartedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.StartedTextBox, true);
      this.StartedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "Started", true));
      this.StartedTextBox.Location = new System.Drawing.Point(81, 65);
      this.StartedTextBox.Name = "StartedTextBox";
      this.StartedTextBox.Size = new System.Drawing.Size(460, 20);
      this.StartedTextBox.TabIndex = 5;
      // 
      // ErrorProvider1
      // 
      this.ErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.ErrorProvider1.ContainerControl = this;
      this.ErrorProvider1.DataSource = this.ProjectBindingSource;
      // 
      // RefreshButton
      // 
      this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.RefreshButton, false);
      this.RefreshButton.Location = new System.Drawing.Point(578, 129);
      this.RefreshButton.Name = "RefreshButton";
      this.RefreshButton.Size = new System.Drawing.Size(75, 23);
      this.RefreshButton.TabIndex = 15;
      this.RefreshButton.Text = "Refresh";
      this.RefreshButton.UseVisualStyleBackColor = true;
      this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
      // 
      // bindingSourceRefresh1
      // 
      this.bindingSourceRefresh1.Host = this;
      // 
      // ProjectEdit
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
      this.Controls.Add(DescriptionLabel);
      this.Controls.Add(this.DescriptionTextBox);
      this.Controls.Add(EndedLabel);
      this.Controls.Add(IdLabel);
      this.Controls.Add(NameLabel);
      this.Controls.Add(StartedLabel);
      this.Controls.Add(this.EndedTextBox);
      this.Controls.Add(this.IdLabel1);
      this.Controls.Add(this.NameTextBox);
      this.Controls.Add(this.StartedTextBox);
      this.Name = "ProjectEdit";
      this.Size = new System.Drawing.Size(665, 454);
      this.CurrentPrincipalChanged += new System.EventHandler(this.ProjectEdit_CurrentPrincipalChanged);
      this.Load += new System.EventHandler(this.ProjectEdit_Load);
      this.GroupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ResourcesDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.RoleListBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ResourcesBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ProjectBindingSource)).EndInit();
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
    internal System.Windows.Forms.DataGridView ResourcesDataGridView;
    internal System.Windows.Forms.TextBox DescriptionTextBox;
    internal System.Windows.Forms.TextBox EndedTextBox;
    internal System.Windows.Forms.Label IdLabel1;
    internal System.Windows.Forms.TextBox NameTextBox;
    internal System.Windows.Forms.TextBox StartedTextBox;
    internal Csla.Windows.ReadWriteAuthorization ReadWriteAuthorization1;
    internal System.Windows.Forms.ErrorProvider ErrorProvider1;
    internal System.Windows.Forms.BindingSource ProjectBindingSource;
    internal System.Windows.Forms.BindingSource ResourcesBindingSource;
    internal System.Windows.Forms.BindingSource RoleListBindingSource;
    private Csla.Windows.BindingSourceRefresh bindingSourceRefresh1;
    internal System.Windows.Forms.Button RefreshButton;
    private System.Windows.Forms.DataGridViewTextBoxColumn ResourceId;
    private System.Windows.Forms.DataGridViewLinkColumn FullName;
    private System.Windows.Forms.DataGridViewComboBoxColumn Role;
    private System.Windows.Forms.DataGridViewTextBoxColumn assignedDataGridViewTextBoxColumn;
  }
}
