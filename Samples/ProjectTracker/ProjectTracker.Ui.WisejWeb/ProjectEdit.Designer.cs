namespace PTWisej
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
      Wisej.Web.Label DescriptionLabel;
      Wisej.Web.Label EndedLabel;
      Wisej.Web.Label IdLabel;
      Wisej.Web.Label NameLabel;
      Wisej.Web.Label StartedLabel;
      this.CloseButton = new Wisej.Web.Button();
      this.ApplyButton = new Wisej.Web.Button();
      this.Cancel_Button = new Wisej.Web.Button();
      this.OKButton = new Wisej.Web.Button();
      this.GroupBox1 = new Wisej.Web.GroupBox();
      this.UnassignButton = new Wisej.Web.Button();
      this.AssignButton = new Wisej.Web.Button();
      this.ResourcesDataGridView = new Wisej.Web.DataGridView();
      this.ResourceId = new Wisej.Web.DataGridViewTextBoxColumn();
      this.FullName = new Wisej.Web.DataGridViewLinkColumn();
      this.Role = new Wisej.Web.DataGridViewComboBoxColumn();
      this.RoleListBindingSource = new Wisej.Web.BindingSource(this.components);
      this.assignedDataGridViewTextBoxColumn = new Wisej.Web.DataGridViewTextBoxColumn();
      this.ResourcesBindingSource = new Wisej.Web.BindingSource(this.components);
      this.ProjectBindingSource = new Wisej.Web.BindingSource(this.components);
      this.DescriptionTextBox = new Wisej.Web.TextBox();
      this.EndedTextBox = new Wisej.Web.TextBox();
      this.IdLabel1 = new Wisej.Web.Label();
      this.NameTextBox = new Wisej.Web.TextBox();
      this.StartedTextBox = new Wisej.Web.TextBox();
      this.ErrorProvider1 = new Wisej.Web.ErrorProvider(this.components);
      this.ReadWriteAuthorization1 = new CslaContrib.WisejWeb.ReadWriteAuthorization(this.components);
      this.RefreshButton = new Wisej.Web.Button();
      DescriptionLabel = new Wisej.Web.Label();
      EndedLabel = new Wisej.Web.Label();
      IdLabel = new Wisej.Web.Label();
      NameLabel = new Wisej.Web.Label();
      StartedLabel = new Wisej.Web.Label();
      this.GroupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ResourcesDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.RoleListBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ResourcesBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ProjectBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // DescriptionLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(DescriptionLabel, false);
      DescriptionLabel.AutoSize = true;
      DescriptionLabel.Location = new System.Drawing.Point(12, 120);
      DescriptionLabel.Name = "DescriptionLabel";
      DescriptionLabel.Size = new System.Drawing.Size(65, 14);
      DescriptionLabel.TabIndex = 8;
      DescriptionLabel.Text = "Description:";
      // 
      // EndedLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(EndedLabel, false);
      EndedLabel.AutoSize = true;
      EndedLabel.Location = new System.Drawing.Point(12, 94);
      EndedLabel.Name = "EndedLabel";
      EndedLabel.Size = new System.Drawing.Size(41, 14);
      EndedLabel.TabIndex = 6;
      EndedLabel.Text = "Ended:";
      // 
      // IdLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(IdLabel, false);
      IdLabel.AutoSize = true;
      IdLabel.Location = new System.Drawing.Point(12, 13);
      IdLabel.Name = "IdLabel";
      IdLabel.Size = new System.Drawing.Size(21, 14);
      IdLabel.TabIndex = 0;
      IdLabel.Text = "Id:";
      // 
      // NameLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(NameLabel, false);
      NameLabel.AutoSize = true;
      NameLabel.Location = new System.Drawing.Point(12, 42);
      NameLabel.Name = "NameLabel";
      NameLabel.Size = new System.Drawing.Size(39, 14);
      NameLabel.TabIndex = 2;
      NameLabel.Text = "Name:";
      // 
      // StartedLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(StartedLabel, false);
      StartedLabel.AutoSize = true;
      StartedLabel.Location = new System.Drawing.Point(12, 68);
      StartedLabel.Name = "StartedLabel";
      StartedLabel.Size = new System.Drawing.Size(46, 14);
      StartedLabel.TabIndex = 4;
      StartedLabel.Text = "Started:";
      // 
      // CloseButton
      // 
      this.CloseButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.CloseButton, false);
      this.CloseButton.DialogResult = Wisej.Web.DialogResult.Cancel;
      this.CloseButton.Location = new System.Drawing.Point(814, 100);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new System.Drawing.Size(75, 23);
      this.CloseButton.TabIndex = 14;
      this.CloseButton.Text = "Close";
      this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
      // 
      // ApplyButton
      // 
      this.ApplyButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.ApplyButton, false);
      this.ApplyButton.DialogResult = Wisej.Web.DialogResult.Cancel;
      this.ApplyButton.Location = new System.Drawing.Point(814, 42);
      this.ApplyButton.Name = "ApplyButton";
      this.ApplyButton.Size = new System.Drawing.Size(75, 23);
      this.ApplyButton.TabIndex = 12;
      this.ApplyButton.Text = "Apply";
      this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
      // 
      // Cancel_Button
      // 
      this.Cancel_Button.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.Cancel_Button, false);
      this.Cancel_Button.DialogResult = Wisej.Web.DialogResult.Cancel;
      this.Cancel_Button.Location = new System.Drawing.Point(814, 71);
      this.Cancel_Button.Name = "Cancel_Button";
      this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
      this.Cancel_Button.TabIndex = 13;
      this.Cancel_Button.Text = "Cancel";
      this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
      // 
      // OKButton
      // 
      this.OKButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.OKButton, false);
      this.OKButton.Location = new System.Drawing.Point(814, 13);
      this.OKButton.Name = "OKButton";
      this.OKButton.Size = new System.Drawing.Size(75, 23);
      this.OKButton.TabIndex = 11;
      this.OKButton.Text = "OK";
      this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
      // 
      // GroupBox1
      // 
      this.GroupBox1.Anchor = ((Wisej.Web.AnchorStyles)((((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom) 
            | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.GroupBox1, false);
      this.GroupBox1.Controls.Add(this.UnassignButton);
      this.GroupBox1.Controls.Add(this.AssignButton);
      this.GroupBox1.Controls.Add(this.ResourcesDataGridView);
      this.GroupBox1.Location = new System.Drawing.Point(81, 231);
      this.GroupBox1.Name = "GroupBox1";
      this.GroupBox1.Size = new System.Drawing.Size(696, 210);
      this.GroupBox1.TabIndex = 10;
      this.GroupBox1.Text = "Resources";
      // 
      // UnassignButton
      // 
      this.UnassignButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.UnassignButton, false);
      this.UnassignButton.Location = new System.Drawing.Point(615, 48);
      this.UnassignButton.Name = "UnassignButton";
      this.UnassignButton.Size = new System.Drawing.Size(75, 23);
      this.UnassignButton.TabIndex = 2;
      this.UnassignButton.Text = "Unassign";
      this.UnassignButton.Click += new System.EventHandler(this.UnassignButton_Click);
      // 
      // AssignButton
      // 
      this.AssignButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.AssignButton, false);
      this.AssignButton.Location = new System.Drawing.Point(615, 19);
      this.AssignButton.Name = "AssignButton";
      this.AssignButton.Size = new System.Drawing.Size(75, 23);
      this.AssignButton.TabIndex = 1;
      this.AssignButton.Text = "Assign";
      this.AssignButton.Click += new System.EventHandler(this.AssignButton_Click);
      // 
      // ResourcesDataGridView
      // 
      this.ResourcesDataGridView.Anchor = ((Wisej.Web.AnchorStyles)((((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom) 
            | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.ResourcesDataGridView, true);
      this.ResourcesDataGridView.AutoGenerateColumns = false;
      this.ResourcesDataGridView.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.AllCells;
      this.ResourcesDataGridView.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.ResourceId,
            this.FullName,
            this.Role,
            this.assignedDataGridViewTextBoxColumn});
      this.ResourcesDataGridView.DataSource = this.ResourcesBindingSource;
      this.ResourcesDataGridView.Location = new System.Drawing.Point(6, 19);
      this.ResourcesDataGridView.MultiSelect = false;
      this.ResourcesDataGridView.Name = "ResourcesDataGridView";
      this.ResourcesDataGridView.RowHeadersVisible = false;
      this.ResourcesDataGridView.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
      this.ResourcesDataGridView.ShowColumnVisibilityMenu = false;
      this.ResourcesDataGridView.Size = new System.Drawing.Size(603, 185);
      this.ResourcesDataGridView.TabIndex = 0;
      this.ResourcesDataGridView.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.ResourcesDataGridView_CellContentClick);
      // 
      // ResourceId
      // 
      this.ResourceId.DataPropertyName = "ResourceId";
      this.ResourceId.HeaderText = "ResourceId";
      this.ResourceId.Name = "ResourceId";
      this.ResourceId.ReadOnly = true;
      this.ResourceId.ValueType = typeof(int);
      this.ResourceId.Visible = false;
      // 
      // FullName
      // 
      this.FullName.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
      this.FullName.DataPropertyName = "FullName";
      this.FullName.FillWeight = 300F;
      this.FullName.HeaderText = "Full Name";
      this.FullName.MinimumWidth = 200;
      this.FullName.Name = "FullName";
      this.FullName.ReadOnly = true;
      this.FullName.Resizable = Wisej.Web.DataGridViewTriState.True;
      this.FullName.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
      this.FullName.Text = "";
      this.FullName.ValueType = typeof(string);
      this.FullName.Width = 300;
      // 
      // Role
      // 
      this.Role.DataPropertyName = "Role";
      this.Role.DataSource = this.RoleListBindingSource;
      this.Role.DisplayMember = "Value";
      this.Role.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
      this.Role.FillWeight = 200F;
      this.Role.HeaderText = "Role";
      this.Role.MinimumWidth = 100;
      this.Role.Name = "Role";
      this.Role.ValueMember = "Key";
      this.Role.ValueType = typeof(int);
      this.Role.Width = 200;
      // 
      // RoleListBindingSource
      // 
      this.RoleListBindingSource.DataSource = typeof(ProjectTracker.Library.RoleList);
      this.RoleListBindingSource.RefreshValueOnChange = true;
      // 
      // assignedDataGridViewTextBoxColumn
      // 
      this.assignedDataGridViewTextBoxColumn.DataPropertyName = "Assigned";
      this.assignedDataGridViewTextBoxColumn.HeaderText = "Assigned";
      this.assignedDataGridViewTextBoxColumn.MinimumWidth = 50;
      this.assignedDataGridViewTextBoxColumn.Name = "assignedDataGridViewTextBoxColumn";
      this.assignedDataGridViewTextBoxColumn.ReadOnly = true;
      this.assignedDataGridViewTextBoxColumn.ValueType = typeof(System.DateTime);
      // 
      // ResourcesBindingSource
      // 
      this.ResourcesBindingSource.DataMember = "Resources";
      this.ResourcesBindingSource.DataSource = this.ProjectBindingSource;
      // 
      // ProjectBindingSource
      // 
      this.ProjectBindingSource.DataSource = typeof(ProjectTracker.Library.ProjectEdit);
      this.ProjectBindingSource.RefreshValueOnChange = true;
      // 
      // DescriptionTextBox
      // 
      this.DescriptionTextBox.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.DescriptionTextBox, true);
      this.DescriptionTextBox.DataBindings.Add(new Wisej.Web.Binding("Text", this.ProjectBindingSource, "Description", true));
      this.DescriptionTextBox.Location = new System.Drawing.Point(81, 117);
      this.DescriptionTextBox.Multiline = true;
      this.DescriptionTextBox.Name = "DescriptionTextBox";
      this.DescriptionTextBox.Size = new System.Drawing.Size(696, 108);
      this.DescriptionTextBox.TabIndex = 9;
      // 
      // EndedTextBox
      // 
      this.EndedTextBox.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.EndedTextBox, true);
      this.EndedTextBox.DataBindings.Add(new Wisej.Web.Binding("Text", this.ProjectBindingSource, "Ended", true));
      this.EndedTextBox.Location = new System.Drawing.Point(81, 91);
      this.EndedTextBox.Name = "EndedTextBox";
      this.EndedTextBox.Size = new System.Drawing.Size(696, 20);
      this.EndedTextBox.TabIndex = 7;
      // 
      // IdLabel1
      // 
      this.IdLabel1.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.IdLabel1, false);
      this.IdLabel1.DataBindings.Add(new Wisej.Web.Binding("Text", this.ProjectBindingSource, "Id", true));
      this.IdLabel1.Location = new System.Drawing.Point(81, 13);
      this.IdLabel1.Name = "IdLabel1";
      this.IdLabel1.Size = new System.Drawing.Size(696, 23);
      this.IdLabel1.TabIndex = 1;
      // 
      // NameTextBox
      // 
      this.NameTextBox.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.NameTextBox, true);
      this.NameTextBox.DataBindings.Add(new Wisej.Web.Binding("Text", this.ProjectBindingSource, "Name", true));
      this.NameTextBox.Location = new System.Drawing.Point(81, 39);
      this.NameTextBox.Name = "NameTextBox";
      this.NameTextBox.Size = new System.Drawing.Size(696, 20);
      this.NameTextBox.TabIndex = 3;
      // 
      // StartedTextBox
      // 
      this.StartedTextBox.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.StartedTextBox, true);
      this.StartedTextBox.DataBindings.Add(new Wisej.Web.Binding("Text", this.ProjectBindingSource, "Started", true));
      this.StartedTextBox.Location = new System.Drawing.Point(81, 65);
      this.StartedTextBox.Name = "StartedTextBox";
      this.StartedTextBox.Size = new System.Drawing.Size(696, 20);
      this.StartedTextBox.TabIndex = 5;
      // 
      // ErrorProvider1
      // 
      this.ErrorProvider1.BlinkStyle = Wisej.Web.ErrorBlinkStyle.NeverBlink;
      this.ErrorProvider1.ContainerControl = this;
      this.ErrorProvider1.DataSource = this.ProjectBindingSource;
      // 
      // RefreshButton
      // 
      this.RefreshButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.RefreshButton, false);
      this.RefreshButton.Location = new System.Drawing.Point(814, 129);
      this.RefreshButton.Name = "RefreshButton";
      this.RefreshButton.Size = new System.Drawing.Size(75, 23);
      this.RefreshButton.TabIndex = 15;
      this.RefreshButton.Text = "Refresh";
      this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
      // 
      // ProjectEdit
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(this, false);
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
      this.Size = new System.Drawing.Size(901, 470);
      this.CurrentPrincipalChanged += new System.EventHandler(this.ProjectEdit_CurrentPrincipalChanged);
      this.Load += new System.EventHandler(this.ProjectEdit_Load);
      this.GroupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ResourcesDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.RoleListBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ResourcesBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ProjectBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    internal Wisej.Web.Button CloseButton;
    internal Wisej.Web.Button ApplyButton;
    internal Wisej.Web.Button Cancel_Button;
    internal Wisej.Web.Button OKButton;
    internal Wisej.Web.GroupBox GroupBox1;
    internal Wisej.Web.Button UnassignButton;
    internal Wisej.Web.Button AssignButton;
    internal Wisej.Web.DataGridView ResourcesDataGridView;
    internal Wisej.Web.TextBox DescriptionTextBox;
    internal Wisej.Web.TextBox EndedTextBox;
    internal Wisej.Web.Label IdLabel1;
    internal Wisej.Web.TextBox NameTextBox;
    internal Wisej.Web.TextBox StartedTextBox;
    internal CslaContrib.WisejWeb.ReadWriteAuthorization ReadWriteAuthorization1;
    internal Wisej.Web.ErrorProvider ErrorProvider1;
    internal Wisej.Web.BindingSource ProjectBindingSource;
    internal Wisej.Web.BindingSource ResourcesBindingSource;
    internal Wisej.Web.BindingSource RoleListBindingSource;
    private Wisej.Web.DataGridViewTextBoxColumn ResourceId;
    private Wisej.Web.DataGridViewLinkColumn FullName;
    private Wisej.Web.DataGridViewComboBoxColumn Role;
    private Wisej.Web.DataGridViewTextBoxColumn assignedDataGridViewTextBoxColumn;
    internal Wisej.Web.Button RefreshButton;
  }
}
