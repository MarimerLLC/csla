namespace PTWisej
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
      Wisej.Web.Label FirstNameLabel;
      Wisej.Web.Label IdLabel;
      Wisej.Web.Label LastNameLabel;
      this.CloseButton = new Wisej.Web.Button();
      this.ApplyButton = new Wisej.Web.Button();
      this.Cancel_Button = new Wisej.Web.Button();
      this.OKButton = new Wisej.Web.Button();
      this.GroupBox1 = new Wisej.Web.GroupBox();
      this.AssignmentsDataGridView = new Wisej.Web.DataGridView();
      this.projectIdDataGridViewTextBoxColumn = new Wisej.Web.DataGridViewTextBoxColumn();
      this.projectNameDataGridViewTextBoxColumn = new Wisej.Web.DataGridViewLinkColumn();
      this.assignedDataGridViewTextBoxColumn = new Wisej.Web.DataGridViewTextBoxColumn();
      this.Role = new Wisej.Web.DataGridViewComboBoxColumn();
      this.RoleListBindingSource = new Wisej.Web.BindingSource(this.components);
      this.AssignmentsBindingSource = new Wisej.Web.BindingSource(this.components);
      this.ResourceBindingSource = new Wisej.Web.BindingSource(this.components);
      this.UnassignButton = new Wisej.Web.Button();
      this.AssignButton = new Wisej.Web.Button();
      this.FirstNameTextBox = new Wisej.Web.TextBox();
      this.IdLabel1 = new Wisej.Web.Label();
      this.LastNameTextBox = new Wisej.Web.TextBox();
      this.ErrorProvider1 = new Wisej.Web.ErrorProvider(this.components);
      this.ReadWriteAuthorization1 = new CslaContrib.WisejWeb.ReadWriteAuthorization(this.components);
      this.RefreshButton = new Wisej.Web.Button();
      FirstNameLabel = new Wisej.Web.Label();
      IdLabel = new Wisej.Web.Label();
      LastNameLabel = new Wisej.Web.Label();
      this.GroupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.AssignmentsDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.RoleListBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.AssignmentsBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ResourceBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // FirstNameLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(FirstNameLabel, false);
      FirstNameLabel.AutoSize = true;
      FirstNameLabel.Location = new System.Drawing.Point(13, 42);
      FirstNameLabel.Name = "FirstNameLabel";
      FirstNameLabel.Size = new System.Drawing.Size(63, 14);
      FirstNameLabel.TabIndex = 2;
      FirstNameLabel.Text = "First Name:";
      // 
      // IdLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(IdLabel, false);
      IdLabel.AutoSize = true;
      IdLabel.Location = new System.Drawing.Point(13, 13);
      IdLabel.Name = "IdLabel";
      IdLabel.Size = new System.Drawing.Size(21, 14);
      IdLabel.TabIndex = 0;
      IdLabel.Text = "Id:";
      // 
      // LastNameLabel
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(LastNameLabel, false);
      LastNameLabel.AutoSize = true;
      LastNameLabel.Location = new System.Drawing.Point(13, 68);
      LastNameLabel.Name = "LastNameLabel";
      LastNameLabel.Size = new System.Drawing.Size(62, 14);
      LastNameLabel.TabIndex = 4;
      LastNameLabel.Text = "Last Name:";
      // 
      // CloseButton
      // 
      this.CloseButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.CloseButton, false);
      this.CloseButton.DialogResult = Wisej.Web.DialogResult.Cancel;
      this.CloseButton.Location = new System.Drawing.Point(814, 100);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new System.Drawing.Size(75, 23);
      this.CloseButton.TabIndex = 10;
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
      this.ApplyButton.TabIndex = 8;
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
      this.Cancel_Button.TabIndex = 9;
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
      this.OKButton.TabIndex = 7;
      this.OKButton.Text = "OK";
      this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
      // 
      // GroupBox1
      // 
      this.GroupBox1.Anchor = ((Wisej.Web.AnchorStyles)((((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom) 
            | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.GroupBox1, false);
      this.GroupBox1.Controls.Add(this.AssignmentsDataGridView);
      this.GroupBox1.Controls.Add(this.UnassignButton);
      this.GroupBox1.Controls.Add(this.AssignButton);
      this.GroupBox1.Location = new System.Drawing.Point(16, 91);
      this.GroupBox1.Name = "GroupBox1";
      this.GroupBox1.Size = new System.Drawing.Size(762, 310);
      this.GroupBox1.TabIndex = 6;
      this.GroupBox1.Text = "Assigned projects";
      // 
      // AssignmentsDataGridView
      // 
      this.AssignmentsDataGridView.Anchor = ((Wisej.Web.AnchorStyles)((((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom) 
            | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.AssignmentsDataGridView, true);
      this.AssignmentsDataGridView.AutoGenerateColumns = false;
      this.AssignmentsDataGridView.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.AllCells;
      this.AssignmentsDataGridView.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.projectIdDataGridViewTextBoxColumn,
            this.projectNameDataGridViewTextBoxColumn,
            this.assignedDataGridViewTextBoxColumn,
            this.Role});
      this.AssignmentsDataGridView.DataSource = this.AssignmentsBindingSource;
      this.AssignmentsDataGridView.Location = new System.Drawing.Point(6, 19);
      this.AssignmentsDataGridView.MultiSelect = false;
      this.AssignmentsDataGridView.Name = "AssignmentsDataGridView";
      this.AssignmentsDataGridView.RowHeadersVisible = false;
      this.AssignmentsDataGridView.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
      this.AssignmentsDataGridView.ShowColumnVisibilityMenu = false;
      this.AssignmentsDataGridView.Size = new System.Drawing.Size(669, 285);
      this.AssignmentsDataGridView.TabIndex = 0;
      this.AssignmentsDataGridView.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.AssignmentsDataGridView_CellContentClick);
      // 
      // projectIdDataGridViewTextBoxColumn
      // 
      this.projectIdDataGridViewTextBoxColumn.DataPropertyName = "ProjectId";
      this.projectIdDataGridViewTextBoxColumn.HeaderText = "ProjectId";
      this.projectIdDataGridViewTextBoxColumn.Name = "projectIdDataGridViewTextBoxColumn";
      this.projectIdDataGridViewTextBoxColumn.ReadOnly = true;
      this.projectIdDataGridViewTextBoxColumn.ValueType = typeof(int);
      this.projectIdDataGridViewTextBoxColumn.Visible = false;
      this.projectIdDataGridViewTextBoxColumn.Width = 74;
      // 
      // projectNameDataGridViewTextBoxColumn
      // 
      this.projectNameDataGridViewTextBoxColumn.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
      this.projectNameDataGridViewTextBoxColumn.DataPropertyName = "ProjectName";
      this.projectNameDataGridViewTextBoxColumn.FillWeight = 200F;
      this.projectNameDataGridViewTextBoxColumn.HeaderText = "Project Name";
      this.projectNameDataGridViewTextBoxColumn.MinimumWidth = 100;
      this.projectNameDataGridViewTextBoxColumn.Name = "projectNameDataGridViewTextBoxColumn";
      this.projectNameDataGridViewTextBoxColumn.ReadOnly = true;
      this.projectNameDataGridViewTextBoxColumn.Resizable = Wisej.Web.DataGridViewTriState.True;
      this.projectNameDataGridViewTextBoxColumn.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
      this.projectNameDataGridViewTextBoxColumn.Text = "";
      this.projectNameDataGridViewTextBoxColumn.ValueType = typeof(string);
      this.projectNameDataGridViewTextBoxColumn.Width = 200;
      // 
      // assignedDataGridViewTextBoxColumn
      // 
      this.assignedDataGridViewTextBoxColumn.DataPropertyName = "Assigned";
      this.assignedDataGridViewTextBoxColumn.HeaderText = "Assigned";
      this.assignedDataGridViewTextBoxColumn.MinimumWidth = 50;
      this.assignedDataGridViewTextBoxColumn.Name = "assignedDataGridViewTextBoxColumn";
      this.assignedDataGridViewTextBoxColumn.ReadOnly = true;
      this.assignedDataGridViewTextBoxColumn.ValueType = typeof(string);
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
      // 
      // AssignmentsBindingSource
      // 
      this.AssignmentsBindingSource.DataMember = "Assignments";
      this.AssignmentsBindingSource.DataSource = this.ResourceBindingSource;
      // 
      // ResourceBindingSource
      // 
      this.ResourceBindingSource.DataSource = typeof(ProjectTracker.Library.ResourceEdit);
      this.ResourceBindingSource.RefreshValueOnChange = true;
      // 
      // UnassignButton
      // 
      this.UnassignButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.UnassignButton, false);
      this.UnassignButton.Location = new System.Drawing.Point(681, 48);
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
      this.AssignButton.Location = new System.Drawing.Point(681, 19);
      this.AssignButton.Name = "AssignButton";
      this.AssignButton.Size = new System.Drawing.Size(75, 23);
      this.AssignButton.TabIndex = 1;
      this.AssignButton.Text = "Assign";
      this.AssignButton.Click += new System.EventHandler(this.AssignButton_Click);
      // 
      // FirstNameTextBox
      // 
      this.FirstNameTextBox.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.FirstNameTextBox, true);
      this.FirstNameTextBox.DataBindings.Add(new Wisej.Web.Binding("Text", this.ResourceBindingSource, "FirstName", true));
      this.FirstNameTextBox.Location = new System.Drawing.Point(80, 39);
      this.FirstNameTextBox.Name = "FirstNameTextBox";
      this.FirstNameTextBox.Size = new System.Drawing.Size(698, 20);
      this.FirstNameTextBox.TabIndex = 3;
      // 
      // IdLabel1
      // 
      this.IdLabel1.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.IdLabel1, true);
      this.IdLabel1.DataBindings.Add(new Wisej.Web.Binding("Text", this.ResourceBindingSource, "Id", true));
      this.IdLabel1.Location = new System.Drawing.Point(80, 13);
      this.IdLabel1.Name = "IdLabel1";
      this.IdLabel1.Size = new System.Drawing.Size(698, 23);
      this.IdLabel1.TabIndex = 1;
      // 
      // LastNameTextBox
      // 
      this.LastNameTextBox.Anchor = ((Wisej.Web.AnchorStyles)(((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.LastNameTextBox, true);
      this.LastNameTextBox.DataBindings.Add(new Wisej.Web.Binding("Text", this.ResourceBindingSource, "LastName", true));
      this.LastNameTextBox.Location = new System.Drawing.Point(80, 65);
      this.LastNameTextBox.Name = "LastNameTextBox";
      this.LastNameTextBox.Size = new System.Drawing.Size(698, 20);
      this.LastNameTextBox.TabIndex = 5;
      // 
      // ErrorProvider1
      // 
      this.ErrorProvider1.BlinkStyle = Wisej.Web.ErrorBlinkStyle.NeverBlink;
      this.ErrorProvider1.ContainerControl = this;
      this.ErrorProvider1.DataSource = this.ResourceBindingSource;
      // 
      // RefreshButton
      // 
      this.RefreshButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
      this.ReadWriteAuthorization1.SetApplyAuthorization(this.RefreshButton, false);
      this.RefreshButton.Location = new System.Drawing.Point(814, 129);
      this.RefreshButton.Name = "RefreshButton";
      this.RefreshButton.Size = new System.Drawing.Size(75, 23);
      this.RefreshButton.TabIndex = 11;
      this.RefreshButton.Text = "Refresh";
      this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
      // 
      // ResourceEdit
      // 
      this.ReadWriteAuthorization1.SetApplyAuthorization(this, false);
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
      this.Size = new System.Drawing.Size(901, 431);
      this.Load += new System.EventHandler(this.ResourceEdit_Load);
      this.GroupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.AssignmentsDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.RoleListBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.AssignmentsBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ResourceBindingSource)).EndInit();
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
    internal Wisej.Web.TextBox FirstNameTextBox;
    internal Wisej.Web.Label IdLabel1;
    internal Wisej.Web.TextBox LastNameTextBox;
    internal CslaContrib.WisejWeb.ReadWriteAuthorization ReadWriteAuthorization1;
    internal Wisej.Web.DataGridView AssignmentsDataGridView;
    internal Wisej.Web.BindingSource RoleListBindingSource;
    internal Wisej.Web.BindingSource AssignmentsBindingSource;
    internal Wisej.Web.BindingSource ResourceBindingSource;
    internal Wisej.Web.ErrorProvider ErrorProvider1;
    private Wisej.Web.DataGridViewTextBoxColumn projectIdDataGridViewTextBoxColumn;
    private Wisej.Web.DataGridViewLinkColumn projectNameDataGridViewTextBoxColumn;
    private Wisej.Web.DataGridViewTextBoxColumn assignedDataGridViewTextBoxColumn;
    private Wisej.Web.DataGridViewComboBoxColumn Role;
    internal Wisej.Web.Button RefreshButton;
  }
}
